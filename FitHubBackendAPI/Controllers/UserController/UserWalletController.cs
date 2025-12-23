using FitHubBackendAPI.DTOs.Wallet;
using FitHubBackendAPI.Services.Interfaces.UserServices;
using FitHubBackendAPI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitHubBackendAPI.Controllers.User_Controller
{
    [Route("api/wallet")]
    [ApiController]
    [Authorize] // يتطلب توكن صالح للدخول (Security Layer)
    public class UserWalletController : ControllerBase
    {
        private readonly IUserWalletService _walletService;

        public UserWalletController(IUserWalletService walletService)
        {
            _walletService = walletService;
        }

        // ==============================================================================================
        // 1. Get Available Plans | عرض خطط الأسعار المتاحة
        // ==============================================================================================
        // Description: Retrieves active subscription plans (e.g., Basic, Premium, Gold) to be displayed on the UI cards.
        // Endpoint:    GET api/wallet/plans
        // ==============================================================================================
        [HttpGet("plans")]
        public async Task<IActionResult> GetPlans()
        {
            var result = await _walletService.GetAllPlansAsync();

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        // ==============================================================================================
        // 2. Purchase Plan | شراء باقة رصيد
        // ==============================================================================================
        // Description: Handles the purchasing logic. It links the user to a plan, calculates tax, 
        //              updates the wallet balance, and logs the transaction.
        // Endpoint:    POST api/wallet/{userId}/purchase
        // Payload:     { "planId": 1 }
        // ==============================================================================================
        [HttpPost("{userId}/purchase")]
        public async Task<IActionResult> PurchasePlan(int userId, [FromBody] PurchasePlanDto dto)
        {
            if (dto == null || dto.PlanId <= 0)
                return BadRequest(ResponseViewModel<bool>.Fail("Invalid Plan Data"));

            var result = await _walletService.PurchasePlanAsync(userId, dto.PlanId);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        // ==============================================================================================
        // 3. Get Wallet Balance | عرض رصيد المحفظة الحالي
        // ==============================================================================================
        // Description: Returns the current available credits for a specific user.
        // Endpoint:    GET api/wallet/{userId}/balance
        // ==============================================================================================
        [HttpGet("{userId}/balance")]
        public async Task<IActionResult> GetBalance(int userId)
        {
            var result = await _walletService.GetWalletBalanceAsync(userId);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        // ==============================================================================================
        // 4. Get Transaction History | عرض سجل المعاملات المالية
        // ==============================================================================================
        // Description: Retrieves a list of all past transactions (Purchases, Usage, Refunds) 
        //              to be displayed in the "Billing & Transactions" table.
        // Endpoint:    GET api/wallet/{userId}/history
        // ==============================================================================================
        [HttpGet("{userId}/history")]
        public async Task<IActionResult> GetTransactions(int userId)
        {
            var result = await _walletService.GetTransactionHistoryAsync(userId);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
    }
}