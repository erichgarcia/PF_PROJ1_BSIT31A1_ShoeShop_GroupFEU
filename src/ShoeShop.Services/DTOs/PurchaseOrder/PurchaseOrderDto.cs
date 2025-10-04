using ShoeShop.Repository.Models;

namespace ShoeShop.Services.DTOs.PurchaseOrder
{
    public class PurchaseOrderDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public DateTime ExpectedDate { get; set; }
        public PurchaseOrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }

        public List<PurchaseOrderItemDto> Items { get; set; } = new();

        // Calculated properties
        public string StatusDisplay => Status.ToString();
        public int TotalItems => Items.Count;
        public int TotalQuantity => Items.Sum(i => i.QuantityOrdered);
        public bool IsOverdue => Status == PurchaseOrderStatus.Pending && ExpectedDate < DateTime.Now;
        public bool CanCancel => Status == PurchaseOrderStatus.Pending;
        public bool CanReceive => Status == PurchaseOrderStatus.Confirmed || Status == PurchaseOrderStatus.Shipped;
        public int DaysUntilExpected => (ExpectedDate - DateTime.Now).Days;
    }
}