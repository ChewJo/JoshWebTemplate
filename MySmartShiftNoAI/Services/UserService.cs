using Microsoft.AspNetCore.Components.Authorization;
using MySmartShiftNoAI.Models;
using System.Text.Json;
using Supabase;

namespace MySmartShiftNoAI.Services;

public interface IUserService
{
    Task<DbUser?> GetDbUser();
    Task<ApplicationUser?> GetUser();
}

public class UserService(AuthenticationStateProvider authenticationStateProvider, Supabase.Client supabaseClient) : IUserService
{
    private readonly AuthenticationStateProvider _authenticationStateProvider = authenticationStateProvider;
    private readonly Supabase.Client _supabaseClient = supabaseClient;

    public async Task<DbUser?> GetDbUser()
    {
        try
        {
            // Use Supabase Auth to get the current session
            var session = _supabaseClient.Auth.CurrentSession;
            if (session == null || session.User == null)
            {
                Console.WriteLine("No authenticated session found.");
                return null;
            }

            var id = session.User.Id;
            Console.WriteLine($"[GetDbUser] Authenticated user id: {id}");

            var guid = Guid.Parse(id);
            Console.WriteLine($"[GetDbUser] Querying for DbUser with Id: {guid}");

            var response = await _supabaseClient.From<DbUser>().Where(x => x.Id == guid).Get();
            Console.WriteLine($"[GetDbUser] DbUser query returned {response.Models.Count} results.");

            var dbUser = response.Models.FirstOrDefault();
            if (dbUser == null)
            {
                Console.WriteLine("[GetDbUser] No DbUser found for this GUID.");
            }
            else
            {
                Console.WriteLine($"[GetDbUser] Found DbUser: {dbUser.Email}, Role: {dbUser.Role}");
            }
            return dbUser;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GetDbUser] Exception: {ex.Message}");
            return null;
        }
    }

    public async Task<ApplicationUser?> GetUser()
    {
        try
        {
            var session = await _authenticationStateProvider.GetAuthenticationStateAsync();

            if (session?.User?.Identity?.IsAuthenticated != true)
            {
                return null;
            }

            var email = session.User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            var id = session.User.FindFirst("sub")?.Value;
            var role = session.User.FindFirst("role")?.Value;
            var firstName = session.User.FindFirst("user_metadata_first_name")?.Value;
            var lastName = session.User.FindFirst("user_metadata_last_name")?.Value;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(id))
            {
                return null;
            }

            return new ApplicationUser
            {
                Id = Guid.Parse(id),
                Email = email,
                FirstName = firstName ?? "",
                LastName = lastName ?? "",
                Role = role ?? "user"
            };
        }
        catch (Exception)
        {
            return null;
        }
    }
}