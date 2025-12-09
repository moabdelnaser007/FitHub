using FitHubBackendAPI.DTOs.Wallet;
using FitHubBackendAPI.Services.Interfaces.UserServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FitHubBackendAPI.Controllers.User_Controller
{
    [Route("api/users/wallet")]
    [ApiController]
    public class UserWalletController : ControllerBase
    {
        private readonly IUserWalletService _walletService;

        //injecting the wallet service
        public UserWalletController(IUserWalletService walletService)
        {
            _walletService = walletService;
        }

        // 1. Endpoint للشحن
        // POST: api/users/wallet/credit
        [HttpPost("credit")]
        public async Task<IActionResult> ChargeWallet([FromBody] ChargeWalletDto dto)
        {
            // ⚠️ ملحوظة: مؤقتاً هنثبت الـ UserID بـ 1 لحد ما نخلص الاوثنتيكيشن
            // بعدين هنغيرها لـ: int.Parse(User.FindFirst("uid")?.Value);
            int userId = 3;

            var result = await _walletService.ChargeWalletAsync(userId, dto);

            if (!result.IsSuccess)
                return BadRequest(result); // رجع ايرور لو فشل

            return Ok(result); // رجع 200 OK لو نجح
        }

        // 2. Endpoint لعرض الرصيد
        // GET: api/users/wallet
        [HttpGet]
        public async Task<IActionResult> GetBalance()
        {
            // نفس الكلام، مثبتين اليوزر 1 مؤقتاً
            int userId = 3;

            var result = await _walletService.GetWalletBalanceAsync(userId);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
    }
}

