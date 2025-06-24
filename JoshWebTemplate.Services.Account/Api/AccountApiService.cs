using FluentResults;
using JoshWebTemplate.Core.Models.Account;
using JoshWebTemplate.Core.Services.Account.Api;
using JoshWebTemplate.Providers.Auth;

namespace JoshWebTemplate.Services.Account.Api;

public class AccountApiService(Supabase.Client supabase, ISupabaseAuthStateProvider supabaseAuth) : IAccountApiService
{
    private readonly Supabase.Client _supabase = supabase;
    private readonly ISupabaseAuthStateProvider _supabaseAuth = supabaseAuth;

    public async Task<Result> LoginAsync(LoginUserModel loginUserModel)
    {
        try
        {
            var session = await _supabase.Auth.SignIn(loginUserModel.Email, loginUserModel.Password);

            if (session?.AccessToken == null)
            {
                return Result.Fail("Login failed");
            }

            await _supabaseAuth.Auth_OnAuthStateChanged(session);

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
            await _supabaseAuth.Auth_OnAuthStateChanged(null);

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

    public async Task<UserModel?> GetUserModelByProfileIdAsync(int profileId)
    {
        var response = await _supabase
            .From<UserModel>()
            .Where(x => x.ProfileId == profileId)
            .Get();

        return response.Models.SingleOrDefault();
    }

    public async Task<List<UserModel>> GetAllUserModelsAsync()
    {
        try
        {
            var response = await _supabase
                .From<UserModel>()
                .Get();

            return response.Models ?? new List<UserModel>();
        }
        catch (Exception)
        {
            return new List<UserModel>();
        }
    }


}
