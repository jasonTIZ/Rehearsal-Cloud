using System;
using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    public class CloudStorage
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        public User User { get; set; } // Relación con User

        [Required]
        [MaxLength(255)]
        public string FileName { get; set; }

        [Required]
        [MaxLength(255)]
        public string FileUrl { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public long FileSize { get; set; }
    }
}