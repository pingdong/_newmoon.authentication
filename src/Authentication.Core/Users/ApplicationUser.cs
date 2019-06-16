using System;

using Microsoft.AspNetCore.Identity;

namespace PingDong.Newmoon.Authentication
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
