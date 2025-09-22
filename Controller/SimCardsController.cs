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
        public async Task<ActionResult<IEnumerable<SimCard>>> GetSimCardsByEmployee(int employeeId)
        {
            var simCards = await _repository.GetSimCardsByEmployeeIdAsync(employeeId);
            return Ok(simCards);
        }

        [HttpGet("transfer/{MasterId}")]
        public async Task<ActionResult<IEnumerable<SimCard>>> GetTransferDetailsByMasterId(int masterId)
        {
            var simCards = await _repository.GetTransferDetailsByMasterIdAsync(masterId);
            if (simCards == null || !simCards.Any())
            {
                return NotFound();
            }
            return Ok(simCards);
        }

        [HttpPost("transfer")]
        public async Task<ActionResult> PostSimCardTransfer([FromBody] SimCardTransferDto transferData)
        {
            int masterId = await _repository.AddSimCardTransferAsync(transferData);
            return CreatedAtAction(nameof(GetTransferDetailsByMasterId), new { masterId = masterId }, transferData);
        }

        [HttpPut("approve/{SimId}")]
        public async Task<IActionResult> ApproveSimCardTransfer(int simId, [FromQuery] int newOwnerEmployeeId)
        {
            await _repository.UpdateSimCardMasterAsync(simId, newOwnerEmployeeId);
            return NoContent();
        }
    }
}