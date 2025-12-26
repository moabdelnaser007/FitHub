using FitHubBackendAPI.Services.Interfaces.OwnerServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FitHubBackendAPI.Controllers.GymControllers
{
    
        [ApiController]
        [Route("api/owner/wallet")]
        [Authorize(Roles = "Owner")]
        public class OwnerWalletController : ControllerBase
        {
            private readonly IOwnerWalletService _walletService;

            public OwnerWalletController(IOwnerWalletService walletService)
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

            [HttpGet("branch/{branchId}")]
            public async Task<IActionResult> GetBranchRevenue(int branchId)
                => Ok(await _walletService.GetBranchRevenueAsync(GetUserId(), branchId));
        }
    }
