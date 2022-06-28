using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppCMS.Data.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [NotMapped]
        [Display(Name = "User")]
        public string UserName { get; set; }

        public OrderState State { get; set; }

        [Column(TypeName = "decimal(9,2)")]
        [Display(Name = "Discount [%]")]
        public decimal PercentDiscount { get; set; }

        [Column(TypeName = "decimal(9,2)")]
        [Display(Name = "Tax [%]")]
        public decimal PercentTax { get; set; }

        [Column(TypeName = "decimal(9,2)")]
        [Display(Name = "Delivery Fee")]
        public decimal DeliveryFee { get; set; }

        public List<OrderItem> OrderItems { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string DeliveryFirstName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string DeliveryLastName { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Full Address (Delivery)")]
        public string DeliveryFullAddress { get; set; }

        [StringLength(500)]
        public string Message { get; set; }

        [Display(Name = "Created")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Modified")]
        public DateTime ModifiedAt { get; set; }

        [Display(Name = "Modified By")]
        public AppUser ModifiedBy { get; set; }

        public enum OrderState
        {
            New = 0,
            Paid = 1,
            Delivered = 2,
            Canceled = -1 // New order can be canceled before payment is done (0 --> -1)
        }

        /// <summary>
        /// Calculates the total considering ItemsTotal, DeliveryFee, Discount (%) and Tax (%). 
        /// </summary>
        /// <returns></returns>
        public decimal GetTotal()
        {
            return GetTotalWithDelivery() - GetDiscountAbsolute() + GetTaxAbsolute();
        }

        public decimal GetItemsTotal()
        {
            if (OrderItems == null) return 0;

            decimal total = 0;
            foreach (var item in OrderItems)
            {
                total += item.GetTotal();
            }
            return total;
        }

        private decimal GetTotalWithDelivery()
        {
            return GetItemsTotal() + DeliveryFee;
        }

        public decimal GetDiscountAbsolute()
        {
            return GetTotalWithDelivery() * (PercentDiscount / 100);
        }

        public decimal GetTaxAbsolute()
        {
            return (GetTotalWithDelivery() - GetDiscountAbsolute()) * (PercentTax / 100);
        }
    }
}
