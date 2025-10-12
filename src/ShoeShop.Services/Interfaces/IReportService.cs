using ShoeShop.Services.DTOs.Report;

namespace ShoeShop.Services.Interfaces
{
    public interface IReportService
    {
        // Dashboard Reporting
        Task<InventoryReportDto> GetInventoryDashboardAsync();
        Task<InventoryReportDto> GetInventoryReportAsync(DateTime? fromDate = null, DateTime? toDate = null);
        Task<InventoryReportDto> RefreshDashboardDataAsync();

        // Stock Reports
        Task<IEnumerable<StockAlertDto>> GetLowStockAlertsAsync();
        Task<IEnumerable<StockAlertDto>> GetOutOfStockAlertsAsync();
        Task<IEnumerable<StockAlertDto>> GetCriticalStockAlertsAsync();
        Task<byte[]> ExportStockReportAsync(string format = "excel"); // "excel", "csv", "pdf"

        // Financial Reports
        Task<decimal> GetTotalInventoryValueAsync();
        Task<decimal> GetInventoryValueByBrandAsync(string brand);
        Task<Dictionary<string, decimal>> GetInventoryValueBreakdownAsync();
        Task<IEnumerable<TopSellingShoeDto>> GetTopSellingItemsAsync(int count = 10, int months = 6);

        // Supplier Performance Reports
        Task<IEnumerable<SupplierPerformanceDto>> GetSupplierPerformanceAsync(int months = 12);
        Task<SupplierPerformanceDto> GetSupplierPerformanceByIdAsync(int supplierId, int months = 12);
        Task<byte[]> ExportSupplierReportAsync(int months = 12, string format = "excel");

        // Transaction History Reports
        Task<IEnumerable<RecentTransactionDto>> GetRecentTransactionsAsync(int count = 50);
        Task<IEnumerable<RecentTransactionDto>> GetTransactionHistoryAsync(DateTime fromDate, DateTime toDate);
        Task<IEnumerable<RecentTransactionDto>> GetTransactionsByTypeAsync(string transactionType, int months = 6);

        // Movement and Trend Reports
        Task<IEnumerable<MonthlyStockMovementDto>> GetMonthlyMovementsAsync(int months = 12);
        Task<IEnumerable<MonthlyStockMovementDto>> GetMovementTrendsAsync(int shoeId, int months = 12);
        Task<Dictionary<string, object>> GetInventoryTrendAnalysisAsync(int months = 6);

        // Custom Reports and Analytics
        Task<Dictionary<string, int>> GetStockDistributionByBrandAsync();
        Task<Dictionary<string, decimal>> GetProfitMarginAnalysisAsync();
        Task<IEnumerable<object>> GetSeasonalTrendsAsync(int years = 2);
        Task<Dictionary<string, object>> GetInventoryMetricsAsync();

        // Export and Download
        Task<byte[]> ExportInventoryReportAsync(DateTime? fromDate, DateTime? toDate, string format = "excel");
        Task<byte[]> ExportLowStockReportAsync(string format = "excel");
        Task<byte[]> ExportTransactionHistoryAsync(DateTime fromDate, DateTime toDate, string format = "excel");

        // Real-time Monitoring
        Task<bool> CheckForLowStockAlertsAsync();
        Task<bool> ShouldSendStockAlertsAsync();
        Task<IEnumerable<string>> GetCriticalStockNotificationsAsync();

        // Performance Metrics
        Task<Dictionary<string, object>> GetPerformanceMetricsAsync();
        Task<Dictionary<string, decimal>> GetInventoryTurnoverRatesAsync();
        Task<IEnumerable<object>> GetSlowMovingItemsAsync(int months = 6);

        // Forecasting and Predictions (Basic)
        Task<Dictionary<string, object>> GetBasicForecastingDataAsync(int shoeId, int months = 6);
        Task<IEnumerable<object>> GetReorderSuggestionsAsync();
        Task<Dictionary<string, int>> GetEstimatedStockNeedsAsync(int months = 3);
    }
}