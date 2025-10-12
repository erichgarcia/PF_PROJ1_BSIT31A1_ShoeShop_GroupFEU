using System.ComponentModel.DataAnnotations;

namespace ShoeShop.Services.DTOs.Shoe
{
    public class CreateShoeDto
    {
        [Required(ErrorMessage = "Shoe name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        [Display(Name = "Shoe Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Brand is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Brand must be between 2 and 50 characters")]
        [Display(Name = "Brand")]
        public string Brand { get; set; } = string.Empty;

        [Required(ErrorMessage = "Cost is required")]
        [Range(0.01, 10000.00, ErrorMessage = "Cost must be between $0.01 and $10,000")]
        [DataType(DataType.Currency)]
        [Display(Name = "Cost Price")]
        public decimal Cost { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 15000.00, ErrorMessage = "Price must be between $0.01 and $15,000")]
        [DataType(DataType.Currency)]
        [Display(Name = "Selling Price")]
        public decimal Price { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        public string? Description { get; set; }

        [StringLength(500, ErrorMessage = "Image URL cannot exceed 500 characters")]
        [DataType(DataType.ImageUrl)]
        [Display(Name = "Image URL")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        [Required(ErrorMessage = "At least one color variation is required")]
        [MinLength(1, ErrorMessage = "At least one color variation is required")]
        public List<CreateColorVariationDto> ColorVariations { get; set; } = new();

        // Custom validation method
        public bool IsValidPricing()
        {
            return Price > Cost;
        }
    }
}