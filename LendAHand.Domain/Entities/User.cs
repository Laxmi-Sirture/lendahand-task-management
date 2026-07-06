using LendAHand.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendAHand.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
        public string? RememberMeToken { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Employee? Employee { get; set; }
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
