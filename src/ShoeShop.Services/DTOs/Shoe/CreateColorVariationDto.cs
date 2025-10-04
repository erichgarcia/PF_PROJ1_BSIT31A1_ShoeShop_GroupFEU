using System.ComponentModel.DataAnnotations;

namespace ShoeShop.Services.DTOs.Shoe
{
    public class CreateColorVariationDto
    {
        [Required(ErrorMessage = "Color name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Color name must be between 2 and 50 characters")]
        [Display(Name = "Color Name")]
        public string ColorName { get; set; } = string.Empty;

        [StringLength(7, MinimumLength = 7, ErrorMessage = "Hex code must be exactly 7 characters (including #)")]
        [RegularExpression("^#[0-9A-Fa-f]{6}$", ErrorMessage = "Hex code must be in format #RRGGBB (e.g., #FF0000)")]
        [Display(Name = "Hex Color Code")]
        public string? HexCode { get; set; }

        [Range(0, 10000, ErrorMessage = "Initial stock must be between 0 and 10,000")]
        [Display(Name = "Initial Stock Quantity")]
        public int StockQuantity { get; set; } = 0;

        [Range(1, 100, ErrorMessage = "Reorder level must be between 1 and 100")]
        [Display(Name = "Reorder Level")]
        public int ReorderLevel { get; set; } = 5;

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;
    }
}