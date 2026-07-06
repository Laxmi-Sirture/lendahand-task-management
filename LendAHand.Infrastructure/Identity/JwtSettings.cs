using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendAHand.Infrastructure.Identity
{
    public class JwtSettings
    {
        public string SecretKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int AccessTokenExpiryHours { get; set; } = 8;
        public int RefreshTokenExpiryDays { get; set; } = 7;
        public int RememberMeExpiryDays { get; set; } = 30;
    }
}
