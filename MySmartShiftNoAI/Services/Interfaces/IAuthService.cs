using FluentResults;
using MySmartShiftNoAI.Models;

namespace MySmartShiftNoAI.Services.Interfaces;

public interface IAuthService
{
    Task<Result> LoginAsync(string email, string password);
    Task<Result> RegisterAsync(LoginArgs model);
    Task<Result> LogoutAsync();
    Task<bool> IsAuthenticatedAsync();
    Task<Result> RefreshTokenAsync();
}
