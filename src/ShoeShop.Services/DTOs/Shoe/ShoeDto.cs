namespace ShoeShop.Services.DTOs.Shoe
{
    public class ShoeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public decimal Cost { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }

        public List<ColorVariationDto> ColorVariations { get; set; } = new();

        // Calculated properties
        public decimal ProfitMargin => Price > 0 ? ((Price - Cost) / Price) * 100 : 0;
        public int TotalStock => ColorVariations.Sum(c => c.StockQuantity);
        public bool IsLowStock => ColorVariations.Any(c => c.StockQuantity <= c.ReorderLevel);
        public int LowStockVariations => ColorVariations.Count(c => c.StockQuantity <= c.ReorderLevel);
    }
}