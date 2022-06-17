using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace WebAppCMS.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [NotMapped]
        [Display(Name = "Category")]
        public string CategoryName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 2)]
        public string Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(9,2)")]
        [Display(Name = "Price")]
        public decimal UnitPrice { get; set; }

        [Display(Name = "Created")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Modified")]
        public DateTime ModifiedAt { get; set; }

        [Display(Name = "Modified By")]
        public IdentityUser ModifiedBy { get; set; }
    }
}
