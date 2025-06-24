using FluentResults;
using JoshWebTemplate.Core.Models.CompanyNews;

namespace JoshWebTemplate.Core.Services.CompanyNews.Api;
public interface ICompanyNewsApiService
{
    Task<Result<CompanyNewsModel>> AddCompanyNewsAsync(AddCompanyNewsRequest model);

    Task<Result> DeleteCompanyNewsAsync(int newsId);

    Task<List<CompanyNewsModel>> GetAllCompanyNewsAsync();

    Task<CompanyNewsModel> GetCompanyNewsByIdAsync(int newsId);

}
