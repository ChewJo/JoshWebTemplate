using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using Supabase.Gotrue;
using System.Text.Json;

namespace MySmartShiftNoAI.Auth;

public class SupabaseAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly Supabase.Client _supabase;
    private readonly ILocalStorageService _localStorage;
    private const string SESSION_KEY = "supabase_session";

    public SupabaseAuthenticationStateProvider(Supabase.Client supabase, ILocalStorageService localStorage)
    {
        _supabase = supabase;
        _localStorage = localStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var json = await _localStorage.GetItemAsStringAsync(SESSION_KEY);
            if (string.IsNullOrEmpty(json))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            var session = JsonSerializer.Deserialize<Session>(json);
            if (session?.User == null)
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, session.User.Id),
                new Claim(ClaimTypes.Email, session.User.Email),
                new Claim("sub", session.User.Id),
                new Claim("role", session.User.Role ?? "user")
            };

            if (session.User.UserMetadata != null)
            {
                foreach (var (key, value) in session.User.UserMetadata)
                {
                    claims.Add(new Claim($"user_metadata_{key}", value?.ToString() ?? ""));
                }
            }

            var identity = new ClaimsIdentity(claims, "supabase-auth");
            var principal = new ClaimsPrincipal(identity);
            return new AuthenticationState(principal);
        }
        catch
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
    }

    public void NotifyAuthenticationStateChanged()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
