using SimCardApi.Models;
using SimCardApi.Repositories.Interfaces;

namespace SimCardApi.Repositories.Services
{
    public class SimCardRepository : ISimCardRepository
    {
        private static List<SimCard> _simCards = new List<SimCard>
        {
            new SimCard { Id = 1, MobileNumber = "9945258780", EmployeeName = "Kinglaya Nama", IsActive = true },
            new SimCard { Id = 2, MobileNumber = "9545378071", EmployeeName = "Ravindra S", IsActive = true }
        };

        public async Task<IEnumerable<SimCard>> GetAllSimCardsAsync()
        {
            return await Task.FromResult(_simCards);
        }

        public async Task<SimCard> GetSimCardByIdAsync(int id)
        {
            var simCard = _simCards.FirstOrDefault(s => s.Id == id);
            if (simCard == null)
            {
                throw new InvalidOperationException($"SimCard with Id {id} not found.");
            }
            return await Task.FromResult(simCard);
        }

        public async Task AddSimCardAsync(SimCard simCard)
        {
            simCard.Id = _simCards.Max(s => s.Id) + 1;
            _simCards.Add(simCard);
            await Task.CompletedTask;
        }

        public async Task UpdateSimCardAsync(SimCard simCard)
        {
            var existingSimCard = _simCards.FirstOrDefault(s => s.Id == simCard.Id);
            if (existingSimCard != null)
            {
                existingSimCard.MobileNumber = simCard.MobileNumber;
                existingSimCard.EmployeeName = simCard.EmployeeName;
                existingSimCard.IsActive = simCard.IsActive;
            }
            await Task.CompletedTask;
        }

        public async Task DeleteSimCardAsync(int id)
        {
            var simCardToRemove = _simCards.FirstOrDefault(s => s.Id == id);
            if (simCardToRemove != null)
            {
                _simCards.Remove(simCardToRemove);
            }
            await Task.CompletedTask;
        }
    }
}