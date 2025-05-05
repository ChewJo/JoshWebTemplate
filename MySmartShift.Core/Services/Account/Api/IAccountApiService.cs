using FluentResults;
using MySmartShift.Core.Models.Account;

namespace MySmartShift.Core.Services.Account.Api;

public interface IAccountApiService
{
    public Task<Result> LoginAsync(LoginUserModel loginUserModel);

    public Task<Result> RegisterAsync(RegisterUserModel registerUserModel);

    public Task<Result> LogoutAsync();

    public Task<UserModel?> GetCurrentUserModelAsync();
}