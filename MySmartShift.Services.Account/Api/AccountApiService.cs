using FluentResults;
using MySmartShift.Core.Models.Account;
using MySmartShift.Core.Services.Account.Api;
using MySmartShift.Providers.Auth;

namespace MySmartShift.Services.Account.Api;

public class AccountApiService(Supabase.Client supabase, SupabaseAuthStateProvider authProvider) : IAccountApiService
{
    private readonly Supabase.Client _supabase = supabase;

    private readonly SupabaseAuthStateProvider _authProvider = authProvider;

    public async Task<Result> LoginAsync(LoginUserModel loginUserModel)
    {
        try
        {
            var session = await _supabase.Auth.SignIn(loginUserModel.Email, loginUserModel.Password);

            if (session?.AccessToken == null)
            {
                return Result.Fail("Login failed");
            }

            await _authProvider.Auth_OnAuthStateChanged(session);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }
    }

    public async Task<Result> RegisterAsync(RegisterUserModel registerUserModel)
    {
        try
        {
            var response = await _supabase.Auth.SignUp(registerUserModel.Email, registerUserModel.Password);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }
    }

    public async Task<Result> LogoutAsync()
    {
        try
        {
            await _supabase.Auth.SignOut();

            await _authProvider.Auth_OnAuthStateChanged(null);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }
    }

    public async Task<UserModel?> GetCurrentUserModelAsync()
    {
        var session = _supabase.Auth.CurrentSession;
        var userId = session?.User?.Id;

        if (string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out var guid))
            return null;

        var response = await _supabase
            .From<UserModel>()
            .Where(x => x.Id == guid)
            .Get();

        return response.Models.SingleOrDefault();
    }

}
