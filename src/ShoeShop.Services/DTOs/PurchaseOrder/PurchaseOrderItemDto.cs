namespace ShoeShop.Services.DTOs.PurchaseOrder
{
    public class PurchaseOrderItemDto
    {
        public int Id { get; set; }
        public int PurchaseOrderId { get; set; }
        public int ShoeColorVariationId { get; set; }
        public int QuantityOrdered { get; set; }
        public int QuantityReceived { get; set; }
        public decimal UnitCost { get; set; }

        // Navigation properties for display
        public string ShoeName { get; set; } = string.Empty;
        public string ShoeBrand { get; set; } = string.Empty;
        public string ColorName { get; set; } = string.Empty;
        public string? HexCode { get; set; }

        // Calculated properties
        public decimal TotalCost => QuantityOrdered * UnitCost;
        public int QuantityPending => QuantityOrdered - QuantityReceived;
        public bool IsFullyReceived => QuantityReceived >= QuantityOrdered;
        public decimal PercentageReceived => QuantityOrdered > 0 ? (decimal)QuantityReceived / QuantityOrdered * 100 : 0;
        public string ReceiveStatus => IsFullyReceived ? "Complete" : QuantityReceived > 0 ? "Partial" : "Pending";
    }
}