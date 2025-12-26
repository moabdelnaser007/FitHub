using FitHubBackendAPI.DTOs.Bookings;
using FitHubBackendAPI.Services.Interfaces.UserServices;
using FitHubBackendAPI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FitHubBackendAPI.Controllers.User_Controller
{
    [ApiController]
    [Route("api/bookings")]
    [Authorize]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _service;

        public BookingController(IBookingService service)
        {
            _service = service;
        }

        private int GetUserId()
        => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        [HttpPost]
        public async Task<IActionResult> Create(CreateBookingDto dto)
            => Ok(await _service.CreateBookingAsync(GetUserId(), dto));

        [HttpGet("my")]
        public async Task<IActionResult> MyBookings()
            => Ok(await _service.GetMyBookingsAsync(GetUserId()));

        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int id)
            => Ok(await _service.GetBookingDetailsAsync(GetUserId(), id));

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id)
            => Ok(await _service.CancelBookingAsync(GetUserId(), id));
    }
}
