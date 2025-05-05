using FluentResults;
using MySmartShift.Core.Models.Account;
using MySmartShift.Core.Services.Account.Api;
using Blazored.LocalStorage;

public class SupabaseAuthStateProvider : AuthenticationStateProvider
{
    private readonly Supabase.Client _supabase;
    private readonly ILocalStorageService _localStorage;

    private const string SessionKey = "auth-token";

    public SupabaseAuthStateProvider(Supabase.Client supabase, ILocalStorageService localStorage)
    {
        _supabase = supabase;
        _localStorage = localStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var sessionJson = await _localStorage.GetItemAsync<string>(SessionKey);
    }
}
