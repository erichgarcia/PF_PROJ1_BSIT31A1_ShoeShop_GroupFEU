namespace ShoeShop.Services.DTOs.Report
{
    public class InventoryReportDto
    {
        public int TotalShoes { get; set; }
        public int TotalColorVariations { get; set; }
        public int TotalStockQuantity { get; set; }
        public decimal TotalInventoryValue { get; set; }
        public int LowStockItems { get; set; }
        public int OutOfStockItems { get; set; }
        public int ActiveSuppliers { get; set; }
        public int PendingPurchaseOrders { get; set; }
        public int PendingPullOutRequests { get; set; }

        // Calculated properties
        public decimal AverageStockValue => TotalColorVariations > 0 ? TotalInventoryValue / TotalColorVariations : 0;
        public decimal LowStockPercentage => TotalColorVariations > 0 ? (decimal)LowStockItems / TotalColorVariations * 100 : 0;
        public decimal OutOfStockPercentage => TotalColorVariations > 0 ? (decimal)OutOfStockItems / TotalColorVariations * 100 : 0;
        public bool HasCriticalStockIssues => OutOfStockItems > 0 || LowStockPercentage > 20;
        
        // Dashboard cards data
        public List<StockAlertDto> LowStockAlerts { get; set; } = new();
        public List<TopSellingShoeDto> TopSellingShoes { get; set; } = new();
        public List<SupplierPerformanceDto> SupplierPerformance { get; set; } = new();
        public List<MonthlyStockMovementDto> MonthlyMovements { get; set; } = new();

        // Recent activity
        public DateTime LastUpdated { get; set; } = DateTime.Now;
        public List<RecentTransactionDto> RecentTransactions { get; set; } = new();
    }

    public class StockAlertDto
    {
        public int ShoeColorVariationId { get; set; }
        public string ShoeName { get; set; } = string.Empty;
        public string ColorName { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public int CurrentStock { get; set; }
        public int ReorderLevel { get; set; }
        public string AlertLevel => CurrentStock == 0 ? "Critical" : "Warning";
        public string? HexCode { get; set; }
    }

    public class TopSellingShoeDto
    {
        public int ShoeId { get; set; }
        public string ShoeName { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public int TotalSold { get; set; }
        public decimal Revenue { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class SupplierPerformanceDto
    {
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public int TotalOrders { get; set; }
        public int CompletedOrders { get; set; }
        public decimal TotalValue { get; set; }
        public decimal OnTimeDeliveryRate { get; set; }
        public string PerformanceRating => OnTimeDeliveryRate >= 95 ? "Excellent" : OnTimeDeliveryRate >= 85 ? "Good" : "Needs Improvement";
    }

    public class MonthlyStockMovementDto
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public int ItemsReceived { get; set; }
        public int ItemsPulledOut { get; set; }
        public int NetMovement => ItemsReceived - ItemsPulledOut;
        public string MonthName => new DateTime(Year, Month, 1).ToString("MMMM");
    }

    public class RecentTransactionDto
    {
        public string TransactionType { get; set; } = string.Empty; // "Purchase Order", "Pull Out", "Stock Adjustment"
        public string Description { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
        public string User { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}