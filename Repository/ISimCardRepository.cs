using SimCardApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimCardApi.Repositories.Interfaces
{
    public interface ISimCardRepository
    {
        Task<IEnumerable<SimCard>> GetSimCardsByEmployeeIdAsync(int employeeId);
        Task<IEnumerable<SimCard>> GetTransferDetailsByMasterIdAsync(long masterId);
        Task<int> AddSimCardTransferAsync(SimCardTransferDto transferData);
        Task UpdateSimCardMasterAsync(int simId, int newOwnerEmployeeId);
    }
}