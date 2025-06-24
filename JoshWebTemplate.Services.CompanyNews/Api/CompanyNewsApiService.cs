using FluentResults;
using JoshWebTemplate.Core.Models.CompanyNews;
using JoshWebTemplate.Core.Services.CompanyNews.Api;
using JoshWebTemplate.Providers.Auth;
using Microsoft.AspNetCore.Http.HttpResults;

namespace JoshWebTemplate.Services.CompanyNews.Api;
public class CompanyNewsApiService(Supabase.Client supabase) : ICompanyNewsApiService
{
    private readonly Supabase.Client _supabase = supabase;

    public async Task<Result<CompanyNewsModel>> AddCompanyNewsAsync(AddCompanyNewsRequest model)
    {
        var companyNews = new CompanyNewsModel
        {
            Title = model.Title,
            Description = model.Description,
            CreatedById = model.CreatedById,
            CreatedDate = DateTime.UtcNow
        };

        var response = await _supabase
            .From<CompanyNewsModel>()
            .Insert(companyNews);

        if (response?.Models?.FirstOrDefault() is CompanyNewsModel createdNews)
        {
            return Result.Ok(createdNews);
        }

        return Result.Fail<CompanyNewsModel>("Failed to create company news");

    }

    public async Task<Result> DeleteCompanyNewsAsync(int newsId)
    {
        try
        {
            var session = _supabase.Auth.CurrentSession;
            var userId = session?.User?.Id;

            if (string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out var currentUserId))
                return Result.Fail("User not authenticated");

            if (newsId <= 0)
                return Result.Fail("Invalid news ID");

            var existingNews = await _supabase
                .From<CompanyNewsModel>()
                .Where(x => x.Id == newsId)
                .Get();

            var newsToDelete = existingNews?.Models?.FirstOrDefault();
            if (newsToDelete == null)
                return Result.Fail("Company news not found");

            await _supabase
                .From<CompanyNewsModel>()
                .Where(x => x.Id == newsId)
                .Delete();

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error deleting company news: {ex.Message}");
        }
    }

    public async Task<List<CompanyNewsModel>> GetAllCompanyNewsAsync()
    {
        var response = await _supabase
            .From<CompanyNewsModel>()
            .Order(x => x.CreatedDate, Supabase.Postgrest.Constants.Ordering.Descending)
            .Get();

        var newsList = response?.Models?.ToList() ?? [];
        return newsList;
    }

    public async Task<CompanyNewsModel> GetCompanyNewsByIdAsync(int newsId)
    {
        if (newsId <= 0)
            return new CompanyNewsModel();

        var response = await _supabase
            .From<CompanyNewsModel>()
            .Where(x => x.Id == newsId)
            .Get();

        var companyNews = response?.Models?.FirstOrDefault();

        if (companyNews == null)
            return new CompanyNewsModel();

        return companyNews;
    }
}
