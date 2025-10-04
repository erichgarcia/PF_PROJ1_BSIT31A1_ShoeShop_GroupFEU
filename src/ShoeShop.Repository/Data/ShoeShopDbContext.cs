using Microsoft.EntityFrameworkCore;
using ShoeShop.Repository.Models;

namespace ShoeShop.Repository.Data
{
    public class ShoeShopDbContext : DbContext
    {
        public ShoeShopDbContext(DbContextOptions<ShoeShopDbContext> options) : base(options)
        {
        }

        // DbSets
        public DbSet<Shoe> Shoes { get; set; }
        public DbSet<ShoeColorVariation> ShoeColorVariations { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }
        public DbSet<StockPullOut> StockPullOuts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Shoe entity
            modelBuilder.Entity<Shoe>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Brand).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Cost).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.ImageUrl).HasMaxLength(500);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("datetime('now')");

                // Index for performance
                entity.HasIndex(e => e.Brand);
                entity.HasIndex(e => e.IsActive);
            });

            // Configure ShoeColorVariation entity
            modelBuilder.Entity<ShoeColorVariation>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ColorName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.HexCode).HasMaxLength(7);
                entity.Property(e => e.StockQuantity).HasDefaultValue(0);
                entity.Property(e => e.ReorderLevel).HasDefaultValue(5);
                entity.Property(e => e.IsActive).HasDefaultValue(true);

                // Relationships
                entity.HasOne(e => e.Shoe)
                      .WithMany(s => s.ColorVariations)
                      .HasForeignKey(e => e.ShoeId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Indexes
                entity.HasIndex(e => e.ShoeId);
                entity.HasIndex(e => e.StockQuantity);
                entity.HasIndex(e => e.IsActive);

                // Unique constraint for Shoe + Color combination
                entity.HasIndex(e => new { e.ShoeId, e.ColorName }).IsUnique();
            });

            // Configure Supplier entity
            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.ContactEmail).HasMaxLength(100);
                entity.Property(e => e.ContactPhone).HasMaxLength(20);
                entity.Property(e => e.Address).HasMaxLength(500);
                entity.Property(e => e.IsActive).HasDefaultValue(true);

                // Indexes
                entity.HasIndex(e => e.Name);
                entity.HasIndex(e => e.IsActive);
            });

            // Configure PurchaseOrder entity
            modelBuilder.Entity<PurchaseOrder>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.OrderNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.OrderDate).HasDefaultValueSql("datetime('now')");
                entity.Property(e => e.Status).HasDefaultValue(PurchaseOrderStatus.Pending);
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");

                // Relationships
                entity.HasOne(e => e.Supplier)
                      .WithMany(s => s.PurchaseOrders)
                      .HasForeignKey(e => e.SupplierId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Indexes
                entity.HasIndex(e => e.OrderNumber).IsUnique();
                entity.HasIndex(e => e.SupplierId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.OrderDate);
            });

            // Configure PurchaseOrderItem entity
            modelBuilder.Entity<PurchaseOrderItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.QuantityReceived).HasDefaultValue(0);
                entity.Property(e => e.UnitCost).HasColumnType("decimal(18,2)");

                // Relationships
                entity.HasOne(e => e.PurchaseOrder)
                      .WithMany(po => po.Items)
                      .HasForeignKey(e => e.PurchaseOrderId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.ShoeColorVariation)
                      .WithMany(scv => scv.PurchaseOrderItems)
                      .HasForeignKey(e => e.ShoeColorVariationId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Indexes
                entity.HasIndex(e => e.PurchaseOrderId);
                entity.HasIndex(e => e.ShoeColorVariationId);
            });

            // Configure StockPullOut entity
            modelBuilder.Entity<StockPullOut>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Reason).IsRequired().HasMaxLength(100);
                entity.Property(e => e.ReasonDetails).HasMaxLength(1000);
                entity.Property(e => e.RequestedBy).IsRequired().HasMaxLength(100);
                entity.Property(e => e.ApprovedBy).HasMaxLength(100);
                entity.Property(e => e.PullOutDate).HasDefaultValueSql("datetime('now')");
                entity.Property(e => e.Status).HasDefaultValue(StockPullOutStatus.Pending);

                // Relationships
                entity.HasOne(e => e.ShoeColorVariation)
                      .WithMany(scv => scv.StockPullOuts)
                      .HasForeignKey(e => e.ShoeColorVariationId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Indexes
                entity.HasIndex(e => e.ShoeColorVariationId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.PullOutDate);
                entity.HasIndex(e => e.RequestedBy);
            });
        }
    }
}