using System.ComponentModel.DataAnnotations;

namespace ShoeShop.Services.DTOs.PullOut
{
    public class CreatePullOutDto
    {
        [Required(ErrorMessage = "Shoe color variation is required")]
        [Display(Name = "Shoe Color Variation")]
        public int ShoeColorVariationId { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, 1000, ErrorMessage = "Quantity must be between 1 and 1,000")]
        [Display(Name = "Quantity to Pull Out")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Reason is required")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Reason must be between 5 and 100 characters")]
        [Display(Name = "Reason for Pull-Out")]
        public string Reason { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Reason details cannot exceed 1000 characters")]
        [Display(Name = "Additional Details")]
        [DataType(DataType.MultilineText)]
        public string? ReasonDetails { get; set; }

        [Required(ErrorMessage = "Requested by is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Requested by must be between 2 and 100 characters")]
        [Display(Name = "Requested By")]
        public string RequestedBy { get; set; } = string.Empty;

        // Helper properties for validation
        public int AvailableStock { get; set; }
        public bool RequiresApproval { get; set; }

        // Common reasons (for dropdown)
        public static readonly List<string> CommonReasons = new()
        {
            "Damaged goods",
            "Customer return",
            "Promotional items",
            "Quality control issues",
            "Theft/loss",
            "Transfer to other location",
            "Sample/display item",
            "Defective product"
        };

        // Validation methods
        public bool HasSufficientStock()
        {
            return Quantity <= AvailableStock;
        }

        public bool IsLargeQuantity()
        {
            return Quantity > 10; // Configurable threshold
        }
    }
}