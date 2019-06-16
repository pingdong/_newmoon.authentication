using System;

using Microsoft.AspNetCore.Identity;

namespace PingDong.NewMoon.UserManagement
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
