using FluentResults;
using JoshWebTemplate.Core.Models.Account;

namespace JoshWebTemplate.Core.Services.Account.Api;

public interface IAccountApiService
{
    public Task<Result> LoginAsync(LoginUserModel loginUserModel);

    public Task<Result> RegisterAsync(RegisterUserModel registerUserModel);

    public Task<Result> LogoutAsync();

    public Task<UserModel?> GetCurrentUserModelAsync();

    public Task<UserModel?> GetUserModelByProfileIdAsync(int profileId);
}