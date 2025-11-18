using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
namespace ShortLink.Models
{
    public class ShortenedUrl
    {
        public int Id { get; set; }
        [Required]
        [Url]
        public string OriginalUrl { get; set; } = string.Empty;
        [Required]
        [MaxLength(10)]
        public string ShortCode { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int ClickCount { get; set; } = 0;
        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual IdentityUser? User { get; set; }
    }
}
