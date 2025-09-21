using SimCardApi.Models;

namespace SimCardApi.Repositories.Interfaces
{
    public interface ISimCardRepository
    {
        Task<IEnumerable<SimCard>> GetAllSimCardsAsync();
        Task<SimCard> GetSimCardByIdAsync(int id);
        Task AddSimCardAsync(SimCard simCard);
        Task UpdateSimCardAsync(SimCard simCard);
        Task DeleteSimCardAsync(int id);
    }
}