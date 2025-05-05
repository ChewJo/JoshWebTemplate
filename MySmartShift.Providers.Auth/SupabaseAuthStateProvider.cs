using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text.Json;
using Supabase.Gotrue;

namespace MySmartShift.Providers.Auth;

public class SupabaseAuthStateProvider(Supabase.Client supabase, ILocalStorageService localStorage) : AuthenticationStateProvider
{
    private readonly Supabase.Client _supabase = supabase;
    private readonly ILocalStorageService _localStorage = localStorage;

    private const string SessionKey = "auth-token";

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var sessionJson = await _localStorage.GetItemAsync<string>(SessionKey);

        if (string.IsNullOrWhiteSpace(sessionJson))        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        var session = JsonSerializer.Deserialize<Session>(sessionJson);

        if (session == null || session.User == null || string.IsNullOrWhiteSpace(session.AccessToken) || string.IsNullOrWhiteSpace(session.RefreshToken))
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        await _supabase.Auth.SetSession(session.AccessToken, session.RefreshToken);

        var identiy = new ClaimsIdentity([new Claim(ClaimTypes.Name, session.User.Id ?? string.Empty)], "supabase");

        var user = new ClaimsPrincipal(identiy);

        return new AuthenticationState(user);
    }

    public async Task Auth_OnAuthStateChanged(Session? session)
    {
        if (session != null)
        {
            var json = JsonSerializer.Serialize(session);

            await _localStorage.SetItemAsync(SessionKey, json);

            var authState = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.Name, session.User?.Id ?? string.Empty)], "supabase")));
            NotifyAuthenticationStateChanged(Task.FromResult(authState));
        }
        else
        {
            await _localStorage.RemoveItemAsync(SessionKey);
            var anon = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            NotifyAuthenticationStateChanged(Task.FromResult(anon));
        }
    }
}
