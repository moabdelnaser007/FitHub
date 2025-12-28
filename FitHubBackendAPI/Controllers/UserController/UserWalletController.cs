using FitHubBackendAPI.DTOs.Wallet;
using FitHubBackendAPI.Entities.Models;
using FitHubBackendAPI.Services.Interfaces.PaymobService;
using FitHubBackendAPI.Services.Interfaces.UserServices;
using FitHubBackendAPI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace FitHubBackendAPI.Controllers.User_Controller
{
    [ApiController]
    [Route("api/wallet")]
    [Authorize]
    public class UserWalletController : ControllerBase
    {
        private readonly IUserWalletService _walletService;
        private readonly IPaymobService _paymobService;
        private readonly IConfiguration _configuration;
        private object? crossValue = new object();

        public UserWalletController(IUserWalletService walletService,
            IPaymobService paymobService,
            IConfiguration configuration)
        {
            _walletService = walletService;
            _paymobService = paymobService;
            _configuration = configuration;
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
        { 
            var transactionResponse = await _walletService.RechargeAsync(GetUserId(), dto);
            if (!transactionResponse.IsSuccess)
            {
                return BadRequest(transactionResponse);
            }
            var transaction = transactionResponse.Data;
            var (redirectionURL, _) = await _paymobService.CreatePaymentAsync(GetUserId(), dto.PlanId, transaction.Id);
            return Ok(new
            {
                
                RedirectUrl = redirectionURL
            });
        }

        [HttpPost("refund/{bookingId}")]
        public async Task<IActionResult> Refund(int bookingId)
            => Ok(await _walletService.RefundBookingAsync(GetUserId(), bookingId));


        

        [AllowAnonymous]
        [HttpGet("callback")]
        public async Task<ResponseViewModel<bool>> CallbackAsync()
        {
            var query = Request.Query;

            string[] fields = new[]
            {
                "amount_cents", "created_at", "currency", "error_occured", "has_parent_transaction",
                "id", "integration_id", "is_3d_secure", "is_auth", "is_capture", "is_refunded",
                "is_standalone_payment", "is_voided", "order", "owner", "pending",
                "source_data.pan", "source_data.sub_type", "source_data.type", "success"
            };

            var concatenated = new StringBuilder();
            foreach (var field in fields)
            {
                if (query.TryGetValue(field, out var value))
                {
                    concatenated.Append(value);
                }
                else
                {
                    return ResponseViewModel<bool>.Fail("Payment Failed",Entities.Enums.ErrorCode.BadRequest);
                }
            }

            string receivedHmac = query["hmac"];
            string calculatedHmac = _paymobService.ComputeHmacSHA512(concatenated.ToString(), _configuration["Paymob:HMAC"]);

            if (receivedHmac.Equals(calculatedHmac, StringComparison.OrdinalIgnoreCase))
            {
                bool.TryParse(query["success"], out bool isSuccess);
                var specialReference = query["merchant_order_id"];

                if (isSuccess)
                {
                    return ResponseViewModel<bool>.Success(true,"payment Successful");
                }

                return ResponseViewModel<bool>.Fail("Payment Failed", Entities.Enums.ErrorCode.BadRequest);
            }

            return ResponseViewModel<bool>.Fail("Payment Failed", Entities.Enums.ErrorCode.BadRequest);
        }
        [AllowAnonymous]
        [HttpPost("ServerCallback")]
        public async Task<ResponseViewModel<bool>> ServerCallback([FromBody] JsonElement payload)
        {
            string receivedHmac = Request.Query["hmac"];
            string secret = _configuration["Paymob:HMAC"];

            if (!payload.TryGetProperty("obj", out var obj))
                return ResponseViewModel<bool>.Fail("Payment Failed", Entities.Enums.ErrorCode.BadRequest);
            string[] fields = new[]
            {
                    "amount_cents", "created_at", "currency", "error_occured", "has_parent_transaction",
                    "id", "integration_id", "is_3d_secure", "is_auth", "is_capture", "is_refunded",
                    "is_standalone_payment", "is_voided", "order.id", "owner", "pending",
                    "source_data.pan", "source_data.sub_type", "source_data.type", "success"
                };
            var concatenated = new StringBuilder();
            foreach (var field in fields)
            {
                string[] parts = field.Split('.');
                JsonElement current = obj;
                bool found = true;
                foreach (var part in parts)
                {
                    if (current.ValueKind == JsonValueKind.Object && current.TryGetProperty(part, out var next))
                        current = next;
                    else
                    {
                        found = false;
                        break;
                    }
                }

                if (!found || current.ValueKind == JsonValueKind.Null)
                {
                    concatenated.Append(""); // Use empty string for missing/null fields
                }
                else if (current.ValueKind == JsonValueKind.True || current.ValueKind == JsonValueKind.False)
                {
                    concatenated.Append(current.GetBoolean() ? "true" : "false"); // Lowercase boolean
                }
                else
                {
                    concatenated.Append(current.ToString());
                }
            }
            string calculatedHmac = _paymobService.ComputeHmacSHA512(concatenated.ToString(), secret);

            if (!receivedHmac.Equals(calculatedHmac, StringComparison.OrdinalIgnoreCase))
                return ResponseViewModel<bool>.Fail("Payment Failed", Entities.Enums.ErrorCode.BadRequest);

            string merchantOrderId = null;
            if (obj.TryGetProperty("order", out var order) &&
                order.TryGetProperty("merchant_order_id", out var merchantOrderIdElement) &&
                merchantOrderIdElement.ValueKind != JsonValueKind.Null)
            {
                merchantOrderId = merchantOrderIdElement.ToString();
            }

            bool isSuccess = obj.TryGetProperty("success", out var successElement) && successElement.GetBoolean();

            if (!string.IsNullOrEmpty(merchantOrderId))
            {
                int refKey= int.Parse(merchantOrderId);
                if (isSuccess)
                {
                    var transaction = await _paymobService.PaymentSuccess(refKey);
                    await _walletService.UpdateWallet(transaction);
                }
                else
                    await _paymobService.PaymentFailed(refKey);
            }

            //return Ok();

            return ResponseViewModel<bool>.Success(true, "payment Successful");
        }
    }
}