using Blazored.LocalStorage;
using FluentResults;
using Microsoft.AspNetCore.Components;
using MySmartShiftNoAI.Models;
using Supabase.Gotrue;

namespace MySmartShiftNoAI.Services;
public class AuthService(ILocalStorageService localStorage, NavigationManager navigation, IConfiguration configuration, Supabase.Client supabase)
{
    private readonly string AccessToken = nameof(AccessToken);
    private readonly string RefreshToken = nameof(RefreshToken);

    private readonly ILocalStorageService _LocalStorage = localStorage;
    private readonly NavigationManager _navigation = navigation;
    private readonly IConfiguration _configuration = configuration;
    private readonly Supabase.Client _supabase = supabase;

    public async Task<Result> LoginAsync(string email, string password)
    {
        try
        {
            var response = await _supabase.Auth.SignIn(email, password);

            if (string.IsNullOrEmpty(response?.AccessToken))
            {
                return Result.Fail("Login failed");
            }

            await _LocalStorage.SetItemAsync(AccessToken, response.AccessToken);
            await _LocalStorage.SetItemAsync(RefreshToken, response.RefreshToken);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }
    }

    public async Task<Result> RegisterAsync(LoginArgs model)
    {
        try
        {
            var data = await _supabase.Auth.SignUp(model.Usernmane, model.Password);
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }
    }

}