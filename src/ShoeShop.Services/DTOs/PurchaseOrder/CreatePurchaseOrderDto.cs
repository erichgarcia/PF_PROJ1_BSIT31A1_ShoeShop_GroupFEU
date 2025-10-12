using System.ComponentModel.DataAnnotations;

namespace ShoeShop.Services.DTOs.PurchaseOrder
{
    public class CreatePurchaseOrderDto
    {
        [Required(ErrorMessage = "Supplier is required")]
        [Display(Name = "Supplier")]
        public int SupplierId { get; set; }

        [Required(ErrorMessage = "Expected delivery date is required")]
        [Display(Name = "Expected Delivery Date")]
        [DataType(DataType.Date)]
        public DateTime ExpectedDate { get; set; } = DateTime.Now.AddDays(7);

        [Required(ErrorMessage = "At least one order item is required")]
        [MinLength(1, ErrorMessage = "At least one order item is required")]
        [Display(Name = "Order Items")]
        public List<CreatePurchaseOrderItemDto> Items { get; set; } = new();

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        [Display(Name = "Order Notes")]
        [DataType(DataType.MultilineText)]
        public string? Notes { get; set; }

        // Custom validation
        public bool IsValidDeliveryDate()
        {
            return ExpectedDate >= DateTime.Now.Date;
        }

        // Calculate total amount
        public decimal CalculateTotalAmount()
        {
            return Items.Sum(item => item.Quantity * item.UnitCost);
        }
    }
}