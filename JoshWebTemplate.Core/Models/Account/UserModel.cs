using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System.ComponentModel.DataAnnotations;

namespace JoshWebTemplate.Core.Models.Account;

[Table("users")]
public class UserModel : BaseModel
{
    [PrimaryKey("id")]
    [Column("id")]
    [Required]
    public Guid Id { get; set; }

    [Column("profile_id")]
    [Required]
    public int ProfileId { get; set; }

    [Column("firstname")]
    [Required]
    public string Firstname { get; set; } = "";

    [Column("lastname")]
    public string Lastname { get; set; } = "";

    [Column("email")]
    [Required]
    public string Email { get; set; } = "";

    [Column("created_at")]
    [Required]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [Column("role")]
    public string? Role { get; set; }
}
