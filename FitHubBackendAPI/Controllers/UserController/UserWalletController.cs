using FitHubBackendAPI.DTOs.Wallet;
using FitHubBackendAPI.Services.Interfaces.UserServices;
using FitHubBackendAPI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FitHubBackendAPI.Controllers.User_Controller
{
    [ApiController]
    [Route("api/wallet")]
    [Authorize]
    public class UserWalletController : ControllerBase
    {
        private readonly IUserWalletService _walletService;

        public UserWalletController(IUserWalletService walletService)
        {
            _walletService = walletService;
        }

        private int GetUserId()
            => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        [HttpGet("balance")]
        public async Task<IActionResult> GetBalance()
            => Ok(await _walletService.GetBalanceAsync(GetUserId()));

        [HttpGet("transactions")]
        public async Task<IActionResult> GetTransactions()
            => Ok(await _walletService.GetTransactionsAsync(GetUserId()));

        [HttpPost("recharge")]
        public async Task<IActionResult> Recharge([FromBody] RechargeWalletDto dto)
            => Ok(await _walletService.RechargeAsync(GetUserId(), dto));

        [HttpPost("refund/{bookingId}")]
        public async Task<IActionResult> Refund(int bookingId)
            => Ok(await _walletService.RefundBookingAsync(GetUserId(), bookingId));
    }
}