using System.ComponentModel.DataAnnotations;

namespace JoshWebTemplate.Core.Models.CompanyNews;
public class AddCompanyNewsRequest
{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, ErrorMessage = "Title must be less than 200 characters")]
    public string Title { get; set; } = "";

    [Required(ErrorMessage = "Description is required")]
    [StringLength(1000, ErrorMessage = "Description must be less than 1000 characters")]
    public string Description { get; set; } = "";

    public int CreatedById { get; set; }
}
