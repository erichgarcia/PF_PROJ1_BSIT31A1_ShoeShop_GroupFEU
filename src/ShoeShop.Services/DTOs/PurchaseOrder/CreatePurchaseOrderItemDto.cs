using System.ComponentModel.DataAnnotations;

namespace ShoeShop.Services.DTOs.PurchaseOrder
{
    public class CreatePurchaseOrderItemDto
    {
        [Required(ErrorMessage = "Shoe color variation is required")]
        [Display(Name = "Shoe Color Variation")]
        public int ShoeColorVariationId { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, 1000, ErrorMessage = "Quantity must be between 1 and 1,000")]
        [Display(Name = "Quantity to Order")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Unit cost is required")]
        [Range(0.01, 5000.00, ErrorMessage = "Unit cost must be between $0.01 and $5,000")]
        [DataType(DataType.Currency)]
        [Display(Name = "Unit Cost")]
        public decimal UnitCost { get; set; }

        // Helper properties for display (will be populated by service)
        public string ShoeName { get; set; } = string.Empty;
        public string ShoeColorName { get; set; } = string.Empty;
        public string ShoeBrand { get; set; } = string.Empty;

        // Calculated properties
        public decimal TotalCost => Quantity * UnitCost;
    }
}