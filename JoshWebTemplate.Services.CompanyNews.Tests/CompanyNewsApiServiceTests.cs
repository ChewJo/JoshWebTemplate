using JoshWebTemplate.Core.Models.CompanyNews;
using JoshWebTemplate.Core.Services.CompanyNews.Api;
using Moq;

namespace JoshWebTemplate.Services.CompanyNews.Tests;

public class CompanyNewsApiServiceTests
{
    private readonly Mock<ICompanyNewsApiService> _mockCompanyNewsApiService;

    public CompanyNewsApiServiceTests()
    {
        _mockCompanyNewsApiService = new Mock<ICompanyNewsApiService>();
    }

    public class GetAllCompanyNewsAsync : CompanyNewsApiServiceTests
    {
        [Fact]
        public async Task ReturnsAllCompanyNews()
        {
            // Arrange
            var expectedNews = new List<CompanyNewsModel>
            {
                new() {
                    Id = 1,
                    Title = "First News",
                    Description = "First Description",
                    CreatedById = 1,
                    CreatedDate = DateTime.UtcNow.AddDays(-2)
                },
                new() {
                    Id = 2,
                    Title = "Second News",
                    Description = "Second Description",
                    CreatedById = 1,
                    CreatedDate = DateTime.UtcNow.AddDays(-1)
                },
                new() {
                    Id = 3,
                    Title = "Latest News",
                    Description = "Latest Description",
                    CreatedById = 1,
                    CreatedDate = DateTime.UtcNow
                }
            };

            _mockCompanyNewsApiService
                .Setup(x => x.GetAllCompanyNewsAsync())
                .ReturnsAsync(expectedNews);

            // Act
            var result = await _mockCompanyNewsApiService.Object.GetAllCompanyNewsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Equal(expectedNews[0].Id, result[0].Id);
            Assert.Equal(expectedNews[1].Id, result[1].Id);
            Assert.Equal(expectedNews[2].Id, result[2].Id);

            // Verify the method was called
            _mockCompanyNewsApiService.Verify(x => x.GetAllCompanyNewsAsync(), Times.Once);
        }

        [Fact]
        public async Task ReturnsEmptyListWhenNoNews()
        {
            // Arrange
            var emptyNewsList = new List<CompanyNewsModel>();

            _mockCompanyNewsApiService
                .Setup(x => x.GetAllCompanyNewsAsync())
                .ReturnsAsync(emptyNewsList);

            // Act
            var result = await _mockCompanyNewsApiService.Object.GetAllCompanyNewsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);

            _mockCompanyNewsApiService.Verify(x => x.GetAllCompanyNewsAsync(), Times.Once);
        }
    }

    public class GetCompanyNewsByIdAsync : CompanyNewsApiServiceTests
    {
        [Fact]
        public async Task ReturnsCorrectNewsItem()
        {
            // Arrange
            var expectedNews = new CompanyNewsModel
            {
                Id = 1,
                Title = "Test News",
                Description = "Test Description",
                CreatedById = 1,
                CreatedDate = DateTime.UtcNow
            };

            _mockCompanyNewsApiService
                .Setup(x => x.GetCompanyNewsByIdAsync(1))
                .ReturnsAsync(expectedNews);

            // Act
            var result = await _mockCompanyNewsApiService.Object.GetCompanyNewsByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedNews.Id, result.Id);
            Assert.Equal(expectedNews.Title, result.Title);
            Assert.Equal(expectedNews.Description, result.Description);

            _mockCompanyNewsApiService.Verify(x => x.GetCompanyNewsByIdAsync(1), Times.Once);
        }
    }
}
