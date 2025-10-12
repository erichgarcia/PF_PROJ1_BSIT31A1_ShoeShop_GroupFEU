using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoeShop.Repository.Models
{
    public enum StockPullOutStatus
    {
        Pending = 0,
        Approved = 1,
        Completed = 2,
        Rejected = 3
    }

    public class StockPullOut
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("ShoeColorVariation")]
        public int ShoeColorVariationId { get; set; }

        public int Quantity { get; set; }

        [Required]
        [StringLength(100)]
        public string Reason { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? ReasonDetails { get; set; }

        [Required]
        [StringLength(100)]
        public string RequestedBy { get; set; } = string.Empty;

        [StringLength(100)]
        public string? ApprovedBy { get; set; }

        public DateTime PullOutDate { get; set; } = DateTime.UtcNow;

        public StockPullOutStatus Status { get; set; } = StockPullOutStatus.Pending;

        // Navigation properties
        public virtual ShoeColorVariation ShoeColorVariation { get; set; } = null!;
    }
}