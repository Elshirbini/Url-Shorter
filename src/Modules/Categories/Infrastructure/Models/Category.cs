using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UrlShorter.src.Modules.Users.Infrastructure.Models;

namespace UrlShorter.src.Modules.Categories.Infrastructure.Models;

[Table("categories")]
public class Category
{
    [Key]
    [Column("category_id")]
    public Guid CategoryId { get; set; } = Guid.NewGuid();

    // 🔗 Foreign Key
    [Required]
    [Column("user_id")]
    public Guid UserId { get; set; }

    // 🧠 Navigation Property
    public User User { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}