namespace ShoeShop.Services.DTOs.Shoe
{
    public class ColorVariationDto
    {
        public int Id { get; set; }
        public int ShoeId { get; set; }
        public string ColorName { get; set; } = string.Empty;
        public string? HexCode { get; set; }
        public int StockQuantity { get; set; }
        public int ReorderLevel { get; set; }
        public bool IsActive { get; set; }

        // Navigation properties
        public string ShoeName { get; set; } = string.Empty;
        public string ShoeBrand { get; set; } = string.Empty;

        // Calculated properties
        public bool IsLowStock => StockQuantity <= ReorderLevel;
        public bool IsOutOfStock => StockQuantity == 0;
        public string StockStatus => IsOutOfStock ? "Out of Stock" : IsLowStock ? "Low Stock" : "In Stock";
    }
}