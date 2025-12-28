using FitHubBackendAPI.DTOs.SettlementDto;
using FitHubBackendAPI.Services.Interfaces.SettlementsService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FitHubBackendAPI.Controllers.AdminController
{
    [ApiController]
    [Route("api/admin/settlements")]
    [Authorize(Roles = "Admin")]
    public class AdminSettlementController : ControllerBase
    {
        private readonly IOwnerSettlementService _service;

        public AdminSettlementController(IOwnerSettlementService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _service.GetAllSettlementsAsync());

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateSettlementStatusDto dto)
            => Ok(await _service.UpdateSettlementStatusAsync(id, dto));
    }

}
