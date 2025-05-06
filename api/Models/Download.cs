using System;
using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    public class Download
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        public User User { get; set; } // Relación con User

        [Required]
        public int SongId { get; set; }
        public Song Song { get; set; } // Relación con Song

        public DateTime DownloadedAt { get; set; } = DateTime.UtcNow;

        public bool IsOfflineAvailable { get; set; }
    }
}