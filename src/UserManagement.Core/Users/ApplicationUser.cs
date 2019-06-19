using Microsoft.AspNetCore.Identity;
using System;

namespace PingDong.NewMoon.UserManagement
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
