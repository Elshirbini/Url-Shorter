using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UrlShorter.Modules.Links.Models;

[Table("clicks")]
public class Click
{
    [Key]
    [Column("click_id")]
    public Guid ClickId { get; set; } = Guid.NewGuid();

    // 🔗 FK
    [Required]
    [Column("link_id")]
    public Guid LinkId { get; set; }

    public Link Link { get; set; } = null!;

    [Column("device_type")]
    [MaxLength(50)]
    public string DeviceType { get; set; } = "unknown";

    [Column("ip")]
    public string? Ip { get; set; }

    [Column("referer")]
    public string? Referer { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}