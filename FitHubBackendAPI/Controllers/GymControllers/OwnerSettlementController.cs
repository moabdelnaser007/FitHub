using FitHubBackendAPI.DTOs.SettlementDto;
using FitHubBackendAPI.Services.Interfaces.SettlementsService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FitHubBackendAPI.Controllers.GymControllers
{
    [ApiController]
    [Route("api/owner/settlements")]
    [Authorize(Roles = "Owner")]
    public class OwnerSettlementController : ControllerBase
    {
        private readonly IOwnerSettlementService _service;

        public OwnerSettlementController(IOwnerSettlementService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSettlementDto dto)
        {
            int ownerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            return Ok(await _service.CreateSettlementAsync(ownerId, dto));
        }

        [HttpGet]
        public async Task<IActionResult> MySettlements()
        {
            int ownerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            return Ok(await _service.GetOwnerSettlementsAsync(ownerId));
        }
    }
}
