using FitHubBackendAPI.DTOs.UserDTOs;
using FitHubBackendAPI.Services.Interfaces.UserServices;
using FitHubBackendAPI.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FitHubBackendAPI.Controllers.UserController
{

    [ApiController]
    [Route("api/gyms")]
    public class GymSearchController : ControllerBase
    {      

        private readonly IGymSearchService _service;

        public GymSearchController(IGymSearchService service)
        {
            _service = service;
        }

        // GET /api/gyms
        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] GymSearchQueryDto query)
        {
            var result = await _service.SearchAsync(query);

            return Ok(ResponseViewModel<List<GymSearchResultDto>>
                .Success(result));
        }
    }
}
