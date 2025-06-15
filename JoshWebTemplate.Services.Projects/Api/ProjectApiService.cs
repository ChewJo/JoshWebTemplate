using FluentResults;
using MySmartShift.Core.Models.Projects;
using MySmartShift.Core.Services.Projects.Api;

namespace MySmartShift.Services.Projects.Api;
public class ProjectApiService(Supabase.Client supabase) : IProjectApiService
{
    private readonly Supabase.Client _supabase = supabase;

    public async Task<ProjectModel> GetProjectByOwnerAsync()
    {
        var session = _supabase.Auth.CurrentSession;
        var userId = session?.User?.Id;

        Console.WriteLine("User Id: ", userId);

        if (string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out var ownerId))
            return null!;

        var response = await _supabase
            .From<ProjectModel>()
            .Where(x => x.OwnerId == ownerId)
            .Get();

        return response.Models.FirstOrDefault()!;
    }

    public async Task<Result> CreateNewProjectAsync()
    {

        try
        {
            var session = _supabase.Auth.CurrentSession;
            var userId = session?.User?.Id;

            Console.WriteLine("User Id: ", userId);
            if (string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out var ownerId))
                return Result.Fail("User not authenticated");

            // Update this to recieve from the front end.
            var project = new ProjectModel
            {
                Id = Guid.NewGuid(),
                Name = "My Project",
                Description = "My Project Description",
                OwnerId = ownerId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };

            var response = await _supabase
                .From<ProjectModel>()
                .Insert(project);

            if (response.Models.Count == 0)
                return Result.Fail("Failed to create project");

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }

    }

}
