using FluentResults;
using MySmartShift.Core.Models.Projects;

namespace MySmartShift.Core.Services.Projects.Api;
public interface IProjectApiService
{
    public Task<ProjectModel> GetProjectByOwnerAsync();

    public Task<Result> CreateNewProjectAsync();
}
