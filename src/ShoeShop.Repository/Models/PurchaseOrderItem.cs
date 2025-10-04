using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoeShop.Repository.Models
{
    public class PurchaseOrderItem
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("PurchaseOrder")]
        public int PurchaseOrderId { get; set; }

        [ForeignKey("ShoeColorVariation")]
        public int ShoeColorVariationId { get; set; }

        public int QuantityOrdered { get; set; }

        public int QuantityReceived { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitCost { get; set; }

        // Navigation properties
        public virtual PurchaseOrder PurchaseOrder { get; set; } = null!;
        public virtual ShoeColorVariation ShoeColorVariation { get; set; } = null!;
    }
}