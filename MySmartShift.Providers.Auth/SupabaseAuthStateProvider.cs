using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;
using Supabase.Gotrue;

namespace MySmartShift.Providers.Auth;

public class SupabaseAuthStateProvider(Supabase.Client supabase, ProtectedLocalStorage localStorage) : AuthenticationStateProvider, ISupabaseAuthStateProvider
{
    private readonly Supabase.Client _supabase = supabase;
    private readonly ProtectedLocalStorage _localStorage = localStorage;

    private const string SessionKey = "sessionState";

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var sessionModel = (await _localStorage.GetAsync<Session>(SessionKey)).Value;

        if (sessionModel == null || sessionModel.User == null || string.IsNullOrWhiteSpace(sessionModel.AccessToken) || string.IsNullOrWhiteSpace(sessionModel.RefreshToken))
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        await _supabase.Auth.SetSession(sessionModel.AccessToken, sessionModel.RefreshToken);

        var identiy = new ClaimsIdentity([new Claim(ClaimTypes.Name, sessionModel.User.Id ?? string.Empty)], "supabase");

        var user = new ClaimsPrincipal(identiy);

        return new AuthenticationState(user);
    }

    public async Task Auth_OnAuthStateChanged(Session? session)
    {
        if (session != null)
        {
            await _localStorage.SetAsync(SessionKey, session);

            var authState = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.Name, session.User?.Id ?? string.Empty)], "supabase")));
            NotifyAuthenticationStateChanged(Task.FromResult(authState));
        }
        else
        {
            await _localStorage.DeleteAsync(SessionKey);
            var anon = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            NotifyAuthenticationStateChanged(Task.FromResult(anon));
        }
    }
}
