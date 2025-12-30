using FitHubBackendAPI.DTOs.SettlementDto;
using FitHubBackendAPI.Services.Interfaces.AdminServices;
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
        private readonly IAdminRevenue _revenue;

        public AdminSettlementController(IOwnerSettlementService service, IAdminRevenue revenue)
        {
            _service = service;
            _revenue = revenue;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _service.GetAllSettlementsAsync());

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateSettlementStatusDto dto)
            => Ok(await _service.UpdateSettlementStatusAsync(id, dto));
        [HttpGet("Revenue")]
        public async Task<IActionResult> getRevenue(DateTime? start, DateTime? End) 
        {
            return Ok(await _revenue.GetAdminAllRevenue(start, End));
        }
    }

}
