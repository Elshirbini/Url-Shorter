using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UrlShorter.Modules.Auth.Models;

[Table("refresh_tokens")]
public class RefreshToken
{
    [Key]
    [Column("refresh_token_id")]
    public Guid RefreshTokenId { get; set; } = Guid.NewGuid();

    [Required]
    [Column("jti")]
    public string Jti { get; set; } = string.Empty;

    [Required]
    [Column("user_id")]
    public Guid UserId { get; set; }

    [Required]
    [Column("expires_at")]
    public DateTime ExpiresAt { get; set; }

    [Column("revoked_at")]
    public DateTime? RevokedAt { get; set; }

 
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

}