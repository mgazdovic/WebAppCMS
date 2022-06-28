using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppCMS.Data.Models
{
    public class AppUser : IdentityUser
    {
        public byte[] Avatar { get; set; }

        [Display(Name = "Created")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Modified")]
        public DateTime ModifiedAt { get; set; }

        [Display(Name = "Modified By")]
        public AppUser ModifiedBy { get; set; }

        public string GetAvatarAsBase64String()
        {
            return Convert.ToBase64String(Avatar);
        }
    }
}
