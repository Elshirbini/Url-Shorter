using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UrlShorter.Modules.Categories.Models;
using UrlShorter.Modules.Users.Models;

namespace UrlShorter.Modules.Links.Models;

[Table("links")]
public class Link
{
    [Key]
    [Column("link_id")]
    public Guid LinkId { get; set; } = Guid.NewGuid();

    [Column("user_id")]
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    // 🔗 Optional relation with category
    [Column("category_id")]
    public Guid? CategoryId { get; set; }

    public Category? Category { get; set; }

    [Required]
    [MaxLength(10)] // 6 chars + future flexibility
    [Column("code")]
    public string Code { get; set; } = string.Empty;

    [Required]
    [Column("redirect_url")]
    public string RedirectUrl { get; set; } = string.Empty;

    // 🔥 denormalized counter (for fast reads)
    [Column("clicks")]
    public int Clicks { get; set; } = 0;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // 🔗 Navigation
    public ICollection<Click> ClicksHistory { get; set; } = new List<Click>();
}