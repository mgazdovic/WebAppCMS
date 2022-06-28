using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppCMS.Data.DTOs
{
    public class ConfigureUserRolesViewModel
    {
        public string Id { get; set; }
        public string RoleName { get; set; }
        public bool Selected { get; set; }
    }
}
