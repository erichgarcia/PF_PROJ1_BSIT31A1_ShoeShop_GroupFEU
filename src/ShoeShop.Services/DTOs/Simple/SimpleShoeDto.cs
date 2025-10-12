using System.ComponentModel.DataAnnotations;

namespace ShoeShop.Services.DTOs.Simple
{
    public class SimpleShoeDto
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string Brand { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string Category { get; set; } = string.Empty;
        
        [Required]
        [StringLength(10)]
        public string Size { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string Color { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string SKU { get; set; } = string.Empty;
        
        [Required]
        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }
        
        [Required]
        [Range(0.01, 100000)]
        public decimal Price { get; set; }
        
        [StringLength(1000)]
        public string? Description { get; set; }
        
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; }
    }
    
    public class SimpleShoeCreateDto
    {
        [Required(ErrorMessage = "Shoe name is required")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Brand is required")]
        [StringLength(50)]
        public string Brand { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Category is required")]
        [StringLength(50)]
        public string Category { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Size is required")]
        [StringLength(10)]
        public string Size { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Color is required")]
        [StringLength(50)]
        public string Color { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "SKU is required")]
        [StringLength(50)]
        public string SKU { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Stock quantity is required")]
        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }
        
        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 100000)]
        public decimal Price { get; set; }
        
        [StringLength(1000)]
        public string? Description { get; set; }
        
        public bool IsActive { get; set; } = true;
    }
    
    public class SimpleShoeUpdateDto : SimpleShoeCreateDto
    {
        public int Id { get; set; }
    }
}