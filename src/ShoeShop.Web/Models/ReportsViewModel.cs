using ShoeShop.Service.DTOs;

namespace ShoeShop.Web.Models
{
    public class ReportsViewModel
    {
        public int TotalProducts { get; set; }
        public decimal TotalValue { get; set; }
        public decimal AveragePrice { get; set; }
        public int TotalStockUnits { get; set; }
        
        public List<BrandReport> BrandReport { get; set; } = new List<BrandReport>();
        public List<CategoryReport> CategoryReport { get; set; } = new List<CategoryReport>();
        public StockStatusReport StockReport { get; set; } = new StockStatusReport();
        public List<PriceRangeReport> PriceRanges { get; set; } = new List<PriceRangeReport>();
    }

    public class BrandReport
    {
        public string Brand { get; set; } = string.Empty;
        public int ProductCount { get; set; }
        public decimal TotalValue { get; set; }
        public decimal AveragePrice { get; set; }
        public int TotalStock { get; set; }
        public decimal ValuePercentage => TotalValue > 0 ? Math.Round((TotalValue / (TotalValue + 1)) * 100, 1) : 0;
    }

    public class CategoryReport
    {
        public string Category { get; set; } = string.Empty;
        public int ProductCount { get; set; }
        public decimal TotalValue { get; set; }
        public decimal AveragePrice { get; set; }
        public int TotalStock { get; set; }
        public decimal ValuePercentage => TotalValue > 0 ? Math.Round((TotalValue / (TotalValue + 1)) * 100, 1) : 0;
    }

    public class StockStatusReport
    {
        public int InStockCount { get; set; }
        public int LowStockCount { get; set; }
        public int OutOfStockCount { get; set; }
        public int TotalProducts { get; set; }
        
        public decimal InStockPercentage => TotalProducts > 0 ? Math.Round((decimal)InStockCount / TotalProducts * 100, 1) : 0;
        public decimal LowStockPercentage => TotalProducts > 0 ? Math.Round((decimal)LowStockCount / TotalProducts * 100, 1) : 0;
        public decimal OutOfStockPercentage => TotalProducts > 0 ? Math.Round((decimal)OutOfStockCount / TotalProducts * 100, 1) : 0;
    }

    public class PriceRangeReport
    {
        public string Range { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class ValueAnalysisViewModel
    {
        public List<ShoeDto> TopValueProducts { get; set; } = new List<ShoeDto>();
        public List<ShoeDto> HighestPricedProducts { get; set; } = new List<ShoeDto>();
        public List<ShoeDto> MostStockedProducts { get; set; } = new List<ShoeDto>();
    }
}