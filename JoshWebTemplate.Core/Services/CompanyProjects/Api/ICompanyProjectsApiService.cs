using FluentResults;
using JoshWebTemplate.Core.Models.CompanyProjects;

namespace JoshWebTemplate.Core.Services.CompanyProjects.Api;
public interface ICompanyProjectsApiService
{
    Task<Result<CompanyProjectModel>> AddCompanyProjectAsync(AddCompanyProjectRequestModel model);

    Task<Result> DeleteCompanyProjectAsync(int projectId);

    Task<List<CompanyProjectModel>> GetAllCompanyProjectsAsync();

    Task<CompanyProjectModel> GetCompanyProjectByIdAsync(int projectId);
}
