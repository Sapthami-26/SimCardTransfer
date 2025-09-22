using SimCardApi.Models;

namespace SimCardApi.Repositories.Interfaces
{
    public interface ISimCardRepository
    {
        Task<IEnumerable<SimCard>> GetSimCardsByEmployeeIdAsync(int employeeId);
        Task<IEnumerable<SimCard>> GetTransferDetailsByMasterIdAsync(int masterId);
        Task<int> AddSimCardTransferAsync(SimCardTransferDto transferData);
        Task UpdateSimCardMasterAsync(int simId, int newOwnerEmployeeId);
    }
}