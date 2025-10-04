using ShoeShop.Repository.Models;
using ShoeShop.Services.DTOs.PurchaseOrder;

namespace ShoeShop.Services.Interfaces
{
    public interface IPurchaseOrderService
    {
        // Purchase Order Management
        Task<IEnumerable<PurchaseOrderDto>> GetAllPurchaseOrdersAsync();
        Task<IEnumerable<PurchaseOrderDto>> GetPurchaseOrdersAsync(int page, int pageSize, PurchaseOrderStatus? statusFilter = null);
        Task<PurchaseOrderDto?> GetPurchaseOrderByIdAsync(int id);
        Task<PurchaseOrderDto> CreatePurchaseOrderAsync(CreatePurchaseOrderDto createOrderDto, string userId);
        Task<bool> UpdatePurchaseOrderStatusAsync(int id, PurchaseOrderStatus status, string userId);
        Task<bool> CancelPurchaseOrderAsync(int id, string reason, string userId);

        // Order Processing Workflow
        Task<bool> ConfirmOrderAsync(int orderId, string userId);
        Task<bool> MarkOrderAsShippedAsync(int orderId, string trackingNumber, string userId);
        Task<bool> ReceiveOrderAsync(int orderId, List<ReceiveOrderItemDto> receivedItems, string userId);
        Task<bool> CompleteOrderAsync(int orderId, string userId);

        // Order Items Management
        Task<IEnumerable<PurchaseOrderItemDto>> GetOrderItemsAsync(int orderId);
        Task<bool> AddOrderItemAsync(int orderId, CreatePurchaseOrderItemDto itemDto);
        Task<bool> UpdateOrderItemAsync(int itemId, CreatePurchaseOrderItemDto itemDto);
        Task<bool> RemoveOrderItemAsync(int itemId);

        // Business Logic and Validation
        Task<string> GenerateOrderNumberAsync();
        Task<bool> ValidateOrderAsync(CreatePurchaseOrderDto orderDto);
        Task<bool> CanCancelOrderAsync(int orderId);
        Task<bool> CanReceiveOrderAsync(int orderId);
        Task<decimal> CalculateOrderTotalAsync(CreatePurchaseOrderDto orderDto);

        // Reporting and Analytics
        Task<IEnumerable<PurchaseOrderDto>> GetOrdersBySupplierAsync(int supplierId);
        Task<IEnumerable<PurchaseOrderDto>> GetOrdersByDateRangeAsync(DateTime fromDate, DateTime toDate);
        Task<IEnumerable<PurchaseOrderDto>> GetOverdueOrdersAsync();
        Task<IEnumerable<PurchaseOrderDto>> GetPendingOrdersAsync();

        // Supplier Integration
        Task<bool> ValidateSupplierAsync(int supplierId);
        Task<IEnumerable<PurchaseOrderDto>> GetOrderHistoryBySupplierAsync(int supplierId, int months = 12);
    }

    // Helper DTO for receiving orders
    public class ReceiveOrderItemDto
    {
        public int PurchaseOrderItemId { get; set; }
        public int QuantityReceived { get; set; }
        public string? Notes { get; set; }
        public bool IsFullyReceived { get; set; }
    }
}