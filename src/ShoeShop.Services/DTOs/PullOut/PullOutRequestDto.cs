using ShoeShop.Repository.Models;

namespace ShoeShop.Services.DTOs.PullOut
{
    public class PullOutRequestDto
    {
        public int Id { get; set; }
        public int ShoeColorVariationId { get; set; }
        public int Quantity { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string? ReasonDetails { get; set; }
        public string RequestedBy { get; set; } = string.Empty;
        public string? ApprovedBy { get; set; }
        public DateTime PullOutDate { get; set; }
        public StockPullOutStatus Status { get; set; }

        // Navigation properties for display
        public string ShoeName { get; set; } = string.Empty;
        public string ShoeBrand { get; set; } = string.Empty;
        public string ColorName { get; set; } = string.Empty;
        public string? HexCode { get; set; }
        public int CurrentStock { get; set; }

        // Calculated properties
        public string StatusDisplay => Status.ToString();
        public bool CanApprove => Status == StockPullOutStatus.Pending;
        public bool CanReject => Status == StockPullOutStatus.Pending;
        public bool CanComplete => Status == StockPullOutStatus.Approved;
        public bool IsApproved => Status == StockPullOutStatus.Approved;
        public bool IsCompleted => Status == StockPullOutStatus.Completed;
        public bool IsRejected => Status == StockPullOutStatus.Rejected;
        public string PriorityLevel => Quantity > 50 ? "High" : Quantity > 10 ? "Medium" : "Low";
        public int DaysSinceRequest => (DateTime.Now - PullOutDate).Days;
        public bool IsOverdue => Status == StockPullOutStatus.Pending && DaysSinceRequest > 2;
    }
}