using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System.ComponentModel.DataAnnotations;

namespace MySmartShift.Core.Models.Projects;

[Table("projects")]
public class ProjectModel : BaseModel
{
    [PrimaryKey("id")]
    [Column("id")]
    [Required]
    public Guid Id { get; set; }

    [Column("name")]
    [Required]
    public string Name { get; set; } = "";

    [Column("description")]
    [Required]
    public string Description { get; set; } = "";

    [Column("owner_id")]
    [Required]
    public Guid OwnerId { get; set; }

    [Column("created_at")]
    [Required]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}
