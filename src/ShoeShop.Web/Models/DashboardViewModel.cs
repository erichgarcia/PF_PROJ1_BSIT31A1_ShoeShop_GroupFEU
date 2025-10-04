namespace ShoeShop.Web.Models
{
    public class DashboardViewModel
    {
        public int TotalShoes { get; set; }
        public decimal TotalValue { get; set; }
        public int LowStockCount { get; set; }
        public int OutOfStockCount { get; set; }
        public List<ActivityItem> RecentActivities { get; set; } = new List<ActivityItem>();
        public List<StockAlert> StockAlerts { get; set; } = new List<StockAlert>();
        public List<CategorySummary> TopCategories { get; set; } = new List<CategorySummary>();
    }

    public class ActivityItem
    {
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string Icon { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
    }

    public class StockAlert
    {
        public string ShoeName { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public string Status { get; set; } = string.Empty;
        public string AlertType { get; set; } = string.Empty;
    }

    public class CategorySummary
    {
        public string Category { get; set; } = string.Empty;
        public int TotalUnits { get; set; }
        public decimal TotalValue { get; set; }
        public string TopBrand { get; set; } = string.Empty;
        public string StockStatus { get; set; } = string.Empty;
    }
}