using FitHubBackendAPI.DTOs.Wallet;
using FitHubBackendAPI.Services.Interfaces.UserServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/users/wallet")]
[Authorize]
public class UserWalletController : ControllerBase
{
    private readonly IUserWalletService _walletService;

    public UserWalletController(IUserWalletService walletService)
    {
        _walletService = walletService;
    }

    private int GetUserId()
    {
        return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
    }

    [HttpPost("credit")]
    public async Task<IActionResult> ChargeWallet([FromBody] ChargeWalletDto dto)
    {
        int userId = GetUserId();
        var result = await _walletService.ChargeWalletAsync(userId, dto);
        return Ok(result);
    }

    [HttpGet("balance")]
    public async Task<IActionResult> GetBalance()
    {
        int userId = GetUserId();
        var result = await _walletService.GetWalletBalanceAsync(userId);
        return Ok(result);
    }

    [HttpGet("transactions")]
    public async Task<IActionResult> GetTransactions()
    {
        int userId = GetUserId();
        var result = await _walletService.GetMyTransactionsAsync(userId);
        return Ok(result);
    }
}
