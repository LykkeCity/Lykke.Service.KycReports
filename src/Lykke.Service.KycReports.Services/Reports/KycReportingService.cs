using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Common.Log;

using Lykke.Service.PersonalData.Contract;

using Newtonsoft.Json;
using Lykke.Service.KycReports.Core.Domain.Reports;
using Lykke.Service.KycReports.AzureRepositories.Reports;
using Lykke.Service.Kyc.Abstractions.Domain.Verification;
using Lykke.Service.Kyc.Abstractions.Services;
using Lykke.Service.Kyc.Abstractions.Services.Models;
using Lykke.Service.PersonalData.Contract.Models;
using Lykke.Service.ClientAccount.Client;
using Lykke.Service.ClientAccount.Client.AutorestClient.Models;
using Lykke.Service.ClientAccount.Client.Models;
using Common;

namespace Lykke.Service.KycReports.Services.Reports
{
    public class KycReportingService : IKycReportingService
    {
        private const string _lykkeWalletPartnerName = "Lykke Wallet";
        private const string _displayDateFormat = "dd-MM-yyyy";
        private const string _boChanger = "BackOffice: ";

        private readonly IKycReportsRepository _reportRepository;
        private readonly IPersonalDataService _personalDataService;
        private readonly ILog _log;
        private readonly IKycStatusService _kycStatusService;
        private readonly IClientAccountClient _clientAccountService;
        private readonly IPartnersClient _partnersService;

        public KycReportingService(
            IKycReportsRepository reportRepository,
            ILog log,
            IPersonalDataService personalDataService,
            IClientAccountClient clientAccountService,
            IPartnersClient partnersService,
            IKycStatusService kycStatusService)
        {
            _reportRepository = reportRepository;
            _personalDataService = personalDataService;
            _log = log;
            _kycStatusService = kycStatusService;
            _clientAccountService = clientAccountService;
            _partnersService = partnersService;
        }

        public async Task<string> GetKycOfficerStatsJsonAsync(DateTime dateFrom, DateTime dateTo)
        {
            var jsonReportRows = (await GetKycOfficerStatsDataJsonRows(dateFrom, dateTo)).ToArray();

            var reportRows = jsonReportRows
                .Select(JsonConvert.DeserializeObject<KycOfficerStatsDataReport>)
                .OrderByDescending(row => row.ReportDay)
                .ThenBy(row => row.KycOfficer);

            jsonReportRows = reportRows.Select(JsonConvert.SerializeObject).ToArray();

            return $"'[\r\n{string.Join(", \r\n", jsonReportRows)}\r\n]'";
        }

        public async Task<string> GetKycOfficersPerformanceJsonAsync(DateTime dateFrom, DateTime dateTo)
        {
            var jsonReportRows = (await GetKycOfficersPerformanceJsonRows(dateFrom, dateTo)).ToArray();

            var reportRows = jsonReportRows
                .Select(JsonConvert.DeserializeObject<KycOfficersPerformanceRow>)
                .OrderByDescending(row => row.ReportDay)
                .ThenBy(row => row.KycOfficer)
                .ThenBy(row => row.Operation.ToString())
                .ThenBy(row => row.ClientEmail);

            jsonReportRows = reportRows.Select(JsonConvert.SerializeObject).ToArray();

            return $"'[\r\n{string.Join(", \r\n", jsonReportRows)}\r\n]'";
        }

        public async Task<string> GetKycReportDailyLeadershipDataJsonAsync(DateTime dateFrom, DateTime dateTo) {
            var jsonReportRows = await GetKycReportDailyLeadershipJsonRows(dateFrom, dateTo);
            return $"'[\r\n{string.Join(", \r\n", jsonReportRows)}\r\n]'";
        }

        private async Task<IEnumerable<string>> GetKycReportDailyLeadershipJsonRows(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                return new List<string>();
           
            var today = DateTime.UtcNow;
            if (endDate >= today)
                endDate = new DateTime(today.Year, today.Month, today.Day);


            var reportRowIds = KycReportDailyLeadership.GenerateRowids(startDate, endDate);
            var jsonReport = await _reportRepository.GetJsonData(KycReportType.KycReportDailyLeadership, true, reportRowIds);

            return jsonReport;
        }

        private async Task<IEnumerable<string>> GetKycOfficerStatsDataJsonRows(DateTime startDate, DateTime endDate)
        {
            startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day);

            var today = DateTime.Today;
            if (endDate > today)
                endDate = today;

            if (endDate < startDate)
                return new List<string>();


            await GenerateKycOfficerStats(startDate, true);
            var jsonReport = await _reportRepository.GetKycOfficerStatsJsonRows(startDate, endDate);

            return jsonReport.Where(jRow => !jRow.Contains(KycOfficerStatsDataReport.EmptyDayKycOfficer));
        }

        private async Task<bool> GenerateKycOfficerStats(DateTime from, bool isExcludeExistedRows)
        {
            var startDate = from;
            var endDate = DateTime.Today.AddDays(-1);

            var datesArray = Enumerable.Range(0, 1 + endDate.Subtract(startDate).Days)
                  .Select(offset => startDate.AddDays(offset))
                  .ToArray();

            List<KycStatusLogRecord> auditLogEntities = null;

            var isGeneratedOk = true;

            foreach (var startOfDay in datesArray)
            {
                try
                {
                    if (isExcludeExistedRows)
                    {
                        var existedJsonRows = await _reportRepository.GetKycOfficerStatsJsonRows(startOfDay, startOfDay);
                        if (existedJsonRows.Any())
                            continue;
                    }

                    if (auditLogEntities == null)
                    {
                        auditLogEntities = await GetKycStatusLogRecords(startDate, endDate);
                    }

                    var itemsTodayGroups =
                        auditLogEntities.Where(i => i.Date >= startOfDay && i.Date < startOfDay.AddDays(1))
                            .GroupBy(i => (KycOfficer: i.KycOfficer, PartnerName: i.PartnerName))
                            .ToList();

                    if (!itemsTodayGroups.Any())
                    {
                        var reportRow = new KycOfficerStatsDataReport()
                        {
                            ReportDay = startOfDay,
                            KycOfficer = KycOfficerStatsDataReport.EmptyDayKycOfficer,
                            PartnerName = string.Empty,
                            OnBoardedCount = -1,
                            DeclinedCount = -1,
                            ToResubmitCount = -1
                        };

                        await _reportRepository.InsertRow(reportRow);
                    }

                    foreach (var itemsToday in itemsTodayGroups)
                    {
                        var kycOfficer = itemsToday.Key.KycOfficer;
                        var partnerName = itemsToday.Key.PartnerName;

                        int onBoardedCount = 0;
                        int declinedCount = 0;
                        int toResubmitCount = 0;

                        foreach (var i in itemsToday)
                        {
                            if ((i.StatusCurrent == KycStatus.ReviewDone || i.StatusCurrent == KycStatus.Ok) && IsPending(i))
                            {
                                onBoardedCount++;
                            }
                            if ((i.StatusCurrent == KycStatus.RestrictedArea || i.StatusCurrent == KycStatus.Rejected) && IsPending(i))
                            {
                                declinedCount++;
                            }
                            if (i.StatusCurrent == KycStatus.NeedToFillData && IsPending(i))
                            {
                                toResubmitCount++;
                            }
                        }

                        var reportRow = new KycOfficerStatsDataReport()
                        {
                            ReportDay = startOfDay,
                            KycOfficer = kycOfficer,
                            PartnerName = partnerName,
                            OnBoardedCount = onBoardedCount,
                            DeclinedCount = declinedCount,
                            ToResubmitCount = toResubmitCount
                        };

                        await _reportRepository.InsertRow(reportRow);
                    }
                }
                catch (Exception ex)
                {
                    await _log.WriteErrorAsync("SrvReports", "GenerateKycOfficerStats (insert new rows)", startOfDay.ToString(), ex);
                    isGeneratedOk = false;
                }
            }

            return isGeneratedOk;
        }

        public async Task<bool> RebuildKycOfficerStats()
        {
            var startDate = DateTime.Today.AddDays(-14);
            return await GenerateKycOfficerStats(startDate, false);
        }
        
        private async Task<IEnumerable<string>> GetKycOfficersPerformanceJsonRows(DateTime startDate, DateTime endDate)
        {
            startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day);

            //var yesterday = DateTime.Today.AddDays(-1);
            var today = DateTime.Today;
            if (endDate > today)
                endDate = today;

            if (endDate < startDate)
                return new List<string>();


            await GenerateKycOfficersPerformance(startDate);
            var jsonReport = await _reportRepository.GetKycOfficersPerformanceJsonRows(startDate, endDate);

            return jsonReport.Where(jRow => !jRow.Contains(KycOfficersPerformanceRow.EmptyDayKycOfficer));
        }

        private async Task<IEnumerable<KycOfficersPerformanceRow>> GenerateKycOfficersPerformance(DateTime from)
        {
            var startDate = from;
            var endDate = DateTime.Today.AddDays(-1);

            var datesArray = Enumerable.Range(0, 1 + endDate.Subtract(startDate).Days)
                  .Select(offset => startDate.AddDays(offset))
                  .ToArray();

            List<KycStatusLogRecord> auditLogEntities = null;

            var reportRows = new List<KycOfficersPerformanceRow>();

            foreach (var startOfDay in datesArray)
            {
                try
                {
                    var existedJsonRows = await _reportRepository.GetKycOfficersPerformanceJsonRows(startOfDay, startOfDay);
                    if (existedJsonRows.Any())
                        continue;

                    if (auditLogEntities == null)
                    {
                        auditLogEntities = await GetKycStatusLogRecords(startDate, endDate);
                    }

                    var itemsTodayGroups =
                        auditLogEntities.Where(i => i.Date >= startOfDay && i.Date < startOfDay.AddDays(1))
                            .GroupBy(i => i.KycOfficer)
                            .ToList();

                    if (!itemsTodayGroups.Any())
                        await _reportRepository.InsertRow(KycOfficersPerformanceRow.MakeNoDataRow(startOfDay));

                    foreach (var itemsToday in itemsTodayGroups)
                    {
                        var kycOfficer = itemsToday.Key;

                        List<KycOfficersPerformanceRow> onBoarded = new List<KycOfficersPerformanceRow>();
                        List<KycOfficersPerformanceRow> declined = new List<KycOfficersPerformanceRow>();
                        List<KycOfficersPerformanceRow> toResubmit = new List<KycOfficersPerformanceRow>();

                        foreach(var row in itemsToday.OrderBy(i => i.Date))
                        {
                            if ((row.StatusCurrent == KycStatus.ReviewDone || row.StatusCurrent == KycStatus.Ok) && IsPending(row))
                            {
                                onBoarded.Add(
                                    new KycOfficersPerformanceRow()
                                    {
                                        ReportDay = startOfDay,
                                        KycOfficer = kycOfficer,
                                        Operation = KycOfficerReportOperationType.OnBoarded,
                                        ClientId = row.ClientId
                                    });
                            }
                            if ((row.StatusCurrent == KycStatus.RestrictedArea || row.StatusCurrent == KycStatus.Rejected) && IsPending(row))
                            {
                                declined.Add(
                                    new KycOfficersPerformanceRow()
                                    {
                                        ReportDay = startOfDay,
                                        KycOfficer = kycOfficer,
                                        Operation = KycOfficerReportOperationType.Declined,
                                        ClientId = row.ClientId
                                    });
                            }
                            if (row.StatusCurrent == KycStatus.NeedToFillData && IsPending(row))
                            {
                                toResubmit.Add(
                                    new KycOfficersPerformanceRow()
                                    {
                                        ReportDay = startOfDay,
                                        KycOfficer = kycOfficer,
                                        Operation = KycOfficerReportOperationType.ToResubmit,
                                        ClientId = row.ClientId
                                    });
                            }
                        }

                        reportRows.AddRange(onBoarded);
                        reportRows.AddRange(declined);
                        reportRows.AddRange(toResubmit);

                        if (onBoarded.Count == 0 && declined.Count == 0 && toResubmit.Count == 0)
                            await _reportRepository.InsertRow(KycOfficersPerformanceRow.MakeNoDataRow(startOfDay));
                    }
                }
                catch (Exception ex)
                {
                    await _log.WriteErrorAsync("SrvReports", "GenerateKycOfficersPerformance (process day)", startOfDay.ToString(), ex);
                }


            }

            var clientIds = reportRows.Select(row => row.ClientId).Where(id => !string.IsNullOrEmpty(id)).Distinct();
            if (reportRows.Count > 0)
            {
                var personalData = (await _personalDataService.GetAsync(clientIds)).ToDictionary(key => key.Id, val => val.Email);
                foreach (var reportRow in reportRows)
                {
                    try
                    {
                        reportRow.ClientEmail = string.Empty;

                        if (string.IsNullOrWhiteSpace(reportRow.ClientId))
                            continue;

                        if (personalData.ContainsKey(reportRow.ClientId))
                            reportRow.ClientEmail = personalData[reportRow.ClientId];

                        await _reportRepository.InsertRow(reportRow);
                    }
                    catch (Exception ex)
                    {
                        await _log.WriteErrorAsync("SrvReports", "GenerateKycOfficersPerformance (insert new rows)", null, ex);
                    }
                }
            }

            return reportRows;
        }

        public async Task<bool> RebuildKycOfficersPerformance()
        {
            var startDate = DateTime.Today.AddDays(-14);
            var endDate = DateTime.Today;
            
            var datesArray = Enumerable.Range(0, 1 + endDate.Subtract(startDate).Days)
                  .Select(offset => startDate.AddDays(offset))
                  .ToArray();

            var auditLogEntities = await GetKycStatusLogRecords(startDate, endDate);

            var reportRows = new List<KycOfficersPerformanceRow>();

            foreach (var startOfDay in datesArray)
            {
                try
                {
                    var itemsTodayGroups =
                        auditLogEntities.Where(i => i.Date >= startOfDay && i.Date < startOfDay.AddDays(1))
                            .GroupBy(i => i.KycOfficer)
                            .ToList();

                    if (!itemsTodayGroups.Any())
                        await _reportRepository.InsertRow(KycOfficersPerformanceRow.MakeNoDataRow(startOfDay));

                    foreach (var itemsToday in itemsTodayGroups)
                    {
                        var kycOfficer = itemsToday.Key;
                        
                        List<KycOfficersPerformanceRow> onBoarded = new List<KycOfficersPerformanceRow>();
                        List<KycOfficersPerformanceRow> declined = new List<KycOfficersPerformanceRow>();
                        List<KycOfficersPerformanceRow> toResubmit = new List<KycOfficersPerformanceRow>();

                        foreach (var row in itemsToday.OrderBy(i => i.Date))
                        {
                            if ((row.StatusCurrent == KycStatus.ReviewDone || row.StatusCurrent == KycStatus.Ok) && IsPending(row))
                            {
                                onBoarded.Add(
                                    new KycOfficersPerformanceRow()
                                    {
                                        ReportDay = startOfDay,
                                        KycOfficer = kycOfficer,
                                        Operation = KycOfficerReportOperationType.OnBoarded,
                                        ClientId = row.ClientId
                                    });
                            }
                            if ((row.StatusCurrent == KycStatus.RestrictedArea || row.StatusCurrent == KycStatus.Rejected) && IsPending(row))
                            {
                                declined.Add(
                                    new KycOfficersPerformanceRow()
                                    {
                                        ReportDay = startOfDay,
                                        KycOfficer = kycOfficer,
                                        Operation = KycOfficerReportOperationType.Declined,
                                        ClientId = row.ClientId
                                    });
                            }
                            if (row.StatusCurrent == KycStatus.NeedToFillData && IsPending(row))
                            {
                                toResubmit.Add(
                                    new KycOfficersPerformanceRow()
                                    {
                                        ReportDay = startOfDay,
                                        KycOfficer = kycOfficer,
                                        Operation = KycOfficerReportOperationType.ToResubmit,
                                        ClientId = row.ClientId
                                    });
                            }
                        }

                        reportRows.AddRange(onBoarded);
                        reportRows.AddRange(declined);
                        reportRows.AddRange(toResubmit);

                        if (onBoarded.Count == 0 && declined.Count == 0 && toResubmit.Count == 0)
                            await _reportRepository.InsertRow(KycOfficersPerformanceRow.MakeNoDataRow(startOfDay));

                    }
                }
                catch (Exception ex)
                {
                    await _log.WriteErrorAsync("SrvReports", "GenerateKycOfficersPerformance (process day)", startOfDay.ToString(), ex);
                    return false;
                }


            }

            var clientIds = reportRows.Select(row => row.ClientId).Where(id => !string.IsNullOrEmpty(id)).Distinct();
            var personalData = (await _personalDataService.GetAsync(clientIds)).ToDictionary(key => key.Id, val => val.Email);
            foreach (var reportRow in reportRows)
            {
                try
                {
                    reportRow.ClientEmail = string.Empty;

                    if (string.IsNullOrWhiteSpace(reportRow.ClientId))
                        continue;

                    if (personalData.ContainsKey(reportRow.ClientId))
                        reportRow.ClientEmail = personalData[reportRow.ClientId];

                    await _reportRepository.InsertRow(reportRow);
                }
                catch (Exception ex)
                {
                    await _log.WriteErrorAsync("SrvReports", "GenerateKycOfficersPerformance (insert new rows)", null, ex);
                    return false;
                }
            }
            
            return true;
        }

        private bool IsPending(KycStatusLogRecord row)
        {
            return row.StatusPrevious == KycStatus.Pending || row.StatusPrevious == KycStatus.JumioFailed || row.StatusPrevious == KycStatus.JumioOk;
        }

        private async Task<List<KycStatusLogRecord>> GetKycStatusLogRecords(DateTime startDate, DateTime endDate)
        {
            ReportFilter f = new ReportFilter();
            f.DatePeriod = new DatePeriod();
            f.DatePeriod.StartDate = startDate;
            f.DatePeriod.EndDate = endDate;

            IEnumerable<IKycStatuschangeItem> items = await _kycStatusService.GetRecordsForPeriodAsync(f);
            IEnumerable<Partner> partners = await _partnersService.GetPartnersAsync();
            Dictionary<string, string> partnersDict = new Dictionary<string, string>();
            foreach (Partner p in partners)
            {
                partnersDict[p.PublicId] = p.Name;
            }

            Dictionary<string, ClientAccountInformationModel> clients = null;
            if (items != null)
            {
                clients = (await _clientAccountService.GetClientsByIdsAsync(items.Select(_ => _.ClientId).Distinct().ToArray())).ToDictionary(_ => _.Id, _ => _);
            }

            var auditLogEntities = (items)?
                            .Select(item =>
                            {
                                var status = (KycStatus)item.CurrentStatus;
                                var previousStatus = (KycStatus)item.PreviousStatus;
                                
                                string kycOfficer;
                                if (item.Changer.StartsWith(_boChanger))
                                    kycOfficer = item.Changer.Substring(_boChanger.Length);
                                else
                                    return null;

                                var partnerName = _lykkeWalletPartnerName;
                                //var client = (_clientAccountService.GetByIdAsync(item.ClientId)).Result;
                                ClientAccountInformationModel client;
                                if (clients.TryGetValue(item.ClientId, out client)) {
                                    if (client.PartnerId != null)
                                    {
                                        if (partnersDict.TryGetValue(client.PartnerId, out string name))
                                        {
                                            partnerName = name;
                                        }
                                        else
                                        {
                                            partnerName = null;
                                            _log.WriteWarningAsync("GetKycStatusLogRecords", new { startDate, endDate }.ToJson(), $"Cannot find Partner with ID = {client.PartnerId}");
                                        }
                                    }
                                }

                                return new KycStatusLogRecord(item.ClientId, status, previousStatus, item.CreatedTime, kycOfficer, partnerName);
                            }).Where(item => item != null).ToList();

            return auditLogEntities;
        }

        public async Task<IEnumerable<KycClientStatRow>> GetKycClientStatRows(DateTime startDate, DateTime endDate, KycStatus[] statusFilter = null)
        {
            List<KycClientStatRow> result = new List<KycClientStatRow>();

            startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day);

            if (endDate < startDate)
            {
                return result;
            }

            ReportFilter f = new ReportFilter();
            f.DatePeriod = new DatePeriod { StartDate = startDate, EndDate = endDate };
            f.StatusFilter = statusFilter;

            var auditLogEntities = await _kycStatusService.GetRecordsForPeriodAsync(f);
            IEnumerable<string> allClientIds = auditLogEntities.Select(_ => _.ClientId).Distinct();
            if (allClientIds.Count() == 0)
            {
                return result;
            }
            IEnumerable<IPersonalData> clients = await _personalDataService.GetAsync(allClientIds);
            var personalDataDict = clients.ToDictionary(_ => _.Id, _ => _);
            var clientPartnerIdsDict = await _clientAccountService.GetPartnerIdsAsync(clients.Select(_ => _.Email).ToArray());
            List<string> allPartnerIds = new List<string>();
            allPartnerIds.AddRange(clientPartnerIdsDict.Values.SelectMany(_ => _).Distinct());

            IEnumerable<Partner> partners = await _partnersService.GetPartnersAsync();
            Dictionary<string, Partner> partnersDict = new Dictionary<string, Partner>();
            foreach (Partner p in partners)
            {
                partnersDict[p.PublicId] = p;
            }
            
            HashSet<string> dubbedPhones = new HashSet<string>();
            HashSet<string> phones = new HashSet<string>();
            var clientsAndPhones = await _clientAccountService.GetClientsByPhonesAsync(clients.Select(_ => _.ContactPhone).Where(_ => !String.IsNullOrWhiteSpace(_)).ToArray());
            foreach (KeyValuePair<string, string> t in clientsAndPhones)
            {
                string phone = t.Value;
                if (!phones.Contains(phone))
                {
                    phones.Add(phone);
                }
                else
                {
                    dubbedPhones.Add(phone);
                }
            }

            IList<string> clientIds = clients.Select(_ => _.Id).ToList();
            var bannedClients = await _clientAccountService.GetBannedClientsAsync(clientIds);
            var bannedClientIds = new HashSet<string>(bannedClients.Select(_ => _.ClientId));

            Dictionary<string, Tuple<string, string>> kycSpiderCheckPersonResult = new Dictionary<string, Tuple<string, string>>();

            IEnumerable<IKycCheckPersonResult> cpRes = await _kycStatusService.GetCheckPersonResultAsync(clientIds.ToArray());
            foreach (IKycCheckPersonResult res in cpRes)
            {
                if (res != null)
                {
                    if (res.PersonProfiles != null && res.PersonProfiles.Count() > 0)
                    {
                        kycSpiderCheckPersonResult[res.Id] = new Tuple<string, string>("Yes", res.CheckDate.ToString(_displayDateFormat));
                    }
                    else
                    {
                        kycSpiderCheckPersonResult[res.Id] = new Tuple<string, string>("No", res.CheckDate.ToString(_displayDateFormat));
                    }
                }
            }

            foreach (Lykke.Service.Kyc.Abstractions.Domain.Verification.IKycStatuschangeItem item in auditLogEntities)
            {
                KycClientStatRow r = new KycClientStatRow();
                r.KycOfficer = item.Changer;
                r.KycStatus = ((Lykke.Service.Kyc.Abstractions.Domain.Verification.KycStatus)item.CurrentStatus).ToString();
                r.ChangeDate = item.CreatedTime;
                r.Date = item.CreatedTime.ToString(_displayDateFormat);

                IList<string> partnerIds;
                if (clientPartnerIdsDict.TryGetValue(item.ClientId, out partnerIds))
                {
                    List<string> partnerInfo = new List<string>();
                    foreach (string partnerId in partnerIds)
                    {
                        Partner partnerData;
                        if (partnersDict.TryGetValue(partnerId, out partnerData))
                        {
                            partnerInfo.Add(partnerId + "/" + partnerData.Name);
                        }
                        else
                        {
                            partnerInfo.Add(partnerId);
                        }
                    }
                    r.PartnerIdName = String.Join(", ", partnerInfo.ToArray());
                }

                r.Id = item.ClientId;
                r.IsBanned = bannedClientIds.Contains(item.ClientId) ? "Yes" : "No";
                r.KycSpiderCheckDate = kycSpiderCheckPersonResult.ContainsKey(item.ClientId) ? kycSpiderCheckPersonResult[item.ClientId].Item2 : "";
                r.IsKycSpiderReturnMatches = kycSpiderCheckPersonResult.ContainsKey(item.ClientId) ? kycSpiderCheckPersonResult[item.ClientId].Item1 : "";

                IPersonalData pd;
                if (personalDataDict.TryGetValue(item.ClientId, out pd))
                {
                    r.CountryFromID = pd.CountryFromID;
                    r.CountryFromPOA = pd.CountryFromPOA;
                    r.CountryFromIP = pd.Country;
                    r.IsDateOfBirthNotEmpty = pd.DateOfBirth == null ? "No" : "Yes";
                    r.DateOfPoaDocument = pd.DateOfPoaDocument?.ToString(_displayDateFormat);
                    r.DateOfExpiryOfID = pd.DateOfExpiryOfID?.ToString(_displayDateFormat);
                    r.IsAddressNotEmpty = String.IsNullOrWhiteSpace(pd.Address) ? "No" : "Yes";
                    r.IsCityNotEmpty = String.IsNullOrWhiteSpace(pd.City) ? "No" : "Yes";
                    r.IsZipNotEmpty = String.IsNullOrWhiteSpace(pd.Zip) ? "No" : "Yes";
                    r.IsPhoneInAnotherAccount = dubbedPhones.Contains(pd.ContactPhone) ? "Yes" : "No";
                }

                result.Add(r);
            }

            IEnumerable<KycClientStatRow> sortedResult = result.OrderByDescending(_ => _.ChangeDate);

            return sortedResult;
        }

    }
}
