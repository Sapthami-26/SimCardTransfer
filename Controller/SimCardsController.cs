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

        [HttpGet("employee/{MempId}")]
        public async Task<ActionResult<IEnumerable<SimCard>>> GetSimCardsByEmployee(int MempId)
        {
            var simCards = await _repository.GetSimCardsByEmployeeIdAsync(MempId);
            return Ok(simCards);
        }

        // Changed parameter type to long to match the repository interface
        [HttpGet("transfer/{MasterId}")]
        public async Task<ActionResult<IEnumerable<SimCard>>> GetTransferDetailsByMasterId(long masterId)
        {
            var simCards = await _repository.GetTransferDetailsByMasterIdAsync(masterId);
            if (simCards == null || !simCards.Any())
            {
                return NotFound();
            }
            return Ok(simCards);
        }

        // Updated the parameter to accept SimCardTransferDto directly
        [HttpPost("transfer")]
        public async Task<ActionResult> PostSimCardTransfer([FromBody] SimCardTransferDto requestData)
        {
            // Validate that the list of SimCardIds is not null or empty
            if (requestData.SimCardIds == null || requestData.SimCardIds.Count == 0)
            {
                return BadRequest("The list of SimCardIds cannot be empty.");
            }

            int masterId = await _repository.AddSimCardTransferAsync(requestData);
            return Ok(masterId);
        }

        [HttpPut("approve/{SimId}")]
        public async Task<IActionResult> ApproveSimCardTransfer(int simId, [FromQuery] int newOwnerEmployeeId)
        {
            await _repository.UpdateSimCardMasterAsync(simId, newOwnerEmployeeId);
            return NoContent();
        }
    }
}