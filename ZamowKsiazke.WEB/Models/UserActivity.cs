using System;
using System.ComponentModel.DataAnnotations;

namespace ZamowKsiazke.Models
{
    public class UserActivity
    {
        public int Id { get; set; }
        
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        [Required]
        public string ActivityType { get; set; } = string.Empty; // Login, Logout, Order, ProfileUpdate
        
        [Required]
        public string Description { get; set; } = string.Empty;
        
        public DateTime Timestamp { get; set; }
        public string? RelatedEntityId { get; set; } // e.g. OrderId for order activities
        
        // Navigation property
        public virtual DefaultUser? User { get; set; }
    }
}
