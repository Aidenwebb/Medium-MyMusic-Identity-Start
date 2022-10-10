using System;
using Microsoft.AspNetCore.Identity;

namespace MyMusic.Core.Models.Auth
{
    public class Role : IdentityRole<Guid>
    {
        public Role() : base()
        {
        }

        public Role(string roleName) : base(roleName)
        {
        }
    }
}