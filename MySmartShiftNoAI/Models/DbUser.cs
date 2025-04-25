using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace MySmartShiftNoAI.Models;

[Table("users")]
public class DbUser : BaseModel
{
    [PrimaryKey("id")]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("email")]
    public string Email { get; set; } = default!;

    [Column("role")]
    public string Role { get; set; } = "user";

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
}
