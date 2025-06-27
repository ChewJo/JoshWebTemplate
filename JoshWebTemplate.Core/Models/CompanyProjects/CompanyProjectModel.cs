﻿using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace JoshWebTemplate.Core.Models.CompanyProjects;

[Table("company_projects")]
public class CompanyProjectModel : BaseModel
{
    [PrimaryKey("id")]
    public int Id { get; set; }

    [Column("title")]
    public string Title { get; set; } = "";

    [Column("description")]
    public string Description { get; set; } = "";

    [Column("created_by")]
    public int CreatedById { get; set; }

    [Column("created_date")]
    public DateTime CreatedDate { get; set; }

    [Column("modified_date")]
    public DateTime? ModifiedDate { get; set; }
}
