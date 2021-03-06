using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppCMS.Data.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        [Required]
        public int ProductId { get; set; }

        [NotMapped]
        [Display(Name = "Product")]
        public string ProductName { get; set; }
        [NotMapped]
        [Display(Name = "Price")]
        public decimal ProductUnitPrice { get; set; }

        [Column(TypeName = "decimal(9,2)")]
        [Required(ErrorMessage = "Quantity is required!")]
        public decimal Quantity { get; set; }

        [Display(Name = "Created")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Modified")]
        public DateTime ModifiedAt { get; set; }

        [Display(Name = "Modified By")]
        public AppUser ModifiedBy { get; set; }

        public decimal GetTotal()
        {
            return Quantity * ProductUnitPrice;
        }
    }
}
