
using FluentResults;
using JoshWebTemplate.Core.Models.CompanyNews;
using JoshWebTemplate.Core.Models.CompanyProjects;
using JoshWebTemplate.Core.Services.CompanyProjects.Api;

namespace JoshWebTemplate.Services.CompanyProjects;
public class CompanyProjectsApiService(Supabase.Client supabase) : ICompanyProjectsApiService
{
    private readonly Supabase.Client _supabase = supabase;

    public async Task<Result<CompanyProjectModel>> AddCompanyProjectAsync(AddCompanyProjectRequestModel model)
    {
        var companyProject = new CompanyProjectModel
        {
            Title = model.Title,
            Description = model.Description,
            CreatedById = model.CreatedById,
            CreatedDate = DateTime.UtcNow
        };

        var response = await _supabase
            .From<CompanyProjectModel>()
            .Insert(companyProject);

        if (response?.Models?.FirstOrDefault() is CompanyProjectModel createdProject)
        {
            return Result.Ok(createdProject);
        }

        return Result.Fail<CompanyProjectModel>("Failed to create company news");
    }

    public async Task<Result> DeleteCompanyProjectAsync(int projectId)
    {
        try
        {
            var session = _supabase.Auth.CurrentSession;
            var userId = session?.User?.Id;

            if (string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out var currentUserId))
                return Result.Fail("User not authenticated");

            if (projectId <= 0)
                return Result.Fail("Invalid news ID");

            var existingNews = await _supabase
                .From<CompanyProjectModel>()
                .Where(x => x.Id == projectId)
                .Get();

            var newsToDelete = existingNews?.Models?.FirstOrDefault();
            if (newsToDelete == null)
                return Result.Fail("Company project not found");

            await _supabase
                .From<CompanyProjectModel>()
                .Where(x => x.Id == projectId)
                .Delete();

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error deleting company project: {ex.Message}");
        }
    }

    public async Task<List<CompanyProjectModel>> GetAllCompanyProjectsAsync()
    {
        var response = await _supabase
            .From<CompanyProjectModel>()
            .Order(x => x.CreatedDate, Supabase.Postgrest.Constants.Ordering.Descending)
            .Get();

        var projectList = response?.Models?.ToList() ?? [];
        return projectList;
    }

    public async Task<CompanyProjectModel> GetCompanyProjectByIdAsync(int projectId)
    {
        if (projectId <= 0)
            return new CompanyProjectModel();

        var response = await _supabase
            .From<CompanyProjectModel>()
            .Where(x => x.Id == projectId)
            .Get();

        var companyProject = response?.Models?.FirstOrDefault();

        if (companyProject == null)
            return new CompanyProjectModel();

        return companyProject;
    }
}
