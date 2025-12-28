using FitHubBackendAPI.DTOs.VisitDTOs;
using FitHubBackendAPI.Entities.Models;
using FitHubBackendAPI.Services.Interfaces.UserServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FitHubBackendAPI.Controllers.UserController
{

    [ApiController]
    [Route("api/visits")]
    [Authorize]
    public class VisitController : ControllerBase
    {
    
            private readonly IVisitService _service;

            public VisitController(IVisitService service)
            {
                _service = service;
            }

        private int GetUserId()
        => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            [HttpPost("check-in")]
            [Authorize(Roles = "Staff")]
            public async Task<IActionResult> CheckIn(CheckInVisitDto dto)
                => Ok(await _service.CheckInAsync(GetUserId(), dto));

           [HttpGet("history")]
           [Authorize]
            public async Task<IActionResult> MyVisits()
                => Ok(await _service.GetMyVisitsAsync(GetUserId()));

            [HttpGet("branch/{branchId}")]
            [Authorize]
            public async Task<IActionResult> BranchVisits(int branchId)
                => Ok(await _service.GetBranchVisitsAsync(branchId, GetUserId()));
        }
    }

