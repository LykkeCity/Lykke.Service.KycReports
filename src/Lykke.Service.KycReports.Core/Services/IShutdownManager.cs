using System.Threading.Tasks;

namespace Lykke.Service.KycReports.Core.Services
{
    public interface IShutdownManager
    {
        Task StopAsync();
    }
}