using System.ComponentModel.DataAnnotations;

namespace CommonLib.Models
{
    public class SilentstormUserDto
    {
        public uint? Id { get; set; }
        [Required]
        [MaxLength(32)]
        public string? Username { get; set; }
        public string? DiscordId { get; set; }
    }
}
