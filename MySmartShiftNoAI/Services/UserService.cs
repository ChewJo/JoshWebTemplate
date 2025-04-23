using Microsoft.AspNetCore.Components.Authorization;
using MySmartShiftNoAI.Models;
using System.Text.Json;

namespace MySmartShiftNoAI.Services;

public interface IUserService
{
    Task<ApplicationUser?> GetUser();
}

public class UserService(AuthenticationStateProvider authenticationStateProvider) : IUserService
{
    private readonly AuthenticationStateProvider _authenticationStateProvider = authenticationStateProvider;

    public async Task<ApplicationUser?> GetUser()
    {
        try
        {
            var session = await _authenticationStateProvider.GetAuthenticationStateAsync();

            if (session?.User?.Identity?.IsAuthenticated != true)
            {
                return null;
            }

            var userMetadataClaim = session.User.FindFirst("user_metadata");
            var subClaim = session.User.FindFirst("sub");
            var roleClaim = session.User.FindFirst("role");

            if (userMetadataClaim == null || subClaim == null || roleClaim == null)
            {
                return null;
            }

            var metadata = JsonSerializer.Deserialize<UserMetaData>(userMetadataClaim.Value);
            if (metadata == null)
            {
                return null;
            }

            return new ApplicationUser
            {
                Id = Guid.Parse(subClaim.Value),
                Email = metadata.email,
                FirstName = metadata.first_name,
                LastName = metadata.last_name,
                Role = roleClaim.Value
            };
        }
        catch (Exception)
        {
            // Log exception if needed
            return null;
        }
    }
}