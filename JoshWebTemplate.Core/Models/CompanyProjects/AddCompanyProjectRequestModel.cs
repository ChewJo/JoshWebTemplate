namespace JoshWebTemplate.Core.Models.CompanyProjects;
public class AddCompanyProjectRequestModel
{
    public string Title { get; set; } = "";

    public string Description { get; set; } = "";

    public int CreatedById { get; set; }
}
