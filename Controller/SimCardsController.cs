using Microsoft.AspNetCore.Mvc;
using SimCardApi.Models;
using SimCardApi.Repositories.Interfaces;

namespace SimCardApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SimCardsController : ControllerBase
    {
        private readonly ISimCardRepository _repository;

        public SimCardsController(ISimCardRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SimCard>>> GetSimCards()
        {
            var simCards = await _repository.GetAllSimCardsAsync();
            return Ok(simCards);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SimCard>> GetSimCard(int id)
        {
            var simCard = await _repository.GetSimCardByIdAsync(id);

            if (simCard == null)
            {
                return NotFound();
            }

            return Ok(simCard);
        }

        [HttpPost]
        public async Task<ActionResult<SimCard>> PostSimCard(SimCard simCard)
        {
            await _repository.AddSimCardAsync(simCard);
            return CreatedAtAction(nameof(GetSimCard), new { id = simCard.Id }, simCard);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutSimCard(int id, SimCard simCard)
        {
            if (id != simCard.Id)
            {
                return BadRequest();
            }

            await _repository.UpdateSimCardAsync(simCard);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSimCard(int id)
        {
            await _repository.DeleteSimCardAsync(id);
            return NoContent();
        }
    }
}