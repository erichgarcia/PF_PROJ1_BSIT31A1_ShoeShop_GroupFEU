using AutoMapper;
using ShoeShop.Repository.Models;
using ShoeShop.Services.DTOs.Shoe;
using ShoeShop.Services.DTOs.PurchaseOrder;
using ShoeShop.Services.DTOs.PullOut;
using ShoeShop.Services.DTOs.Report;

namespace ShoeShop.Services.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            ConfigureShoeMappings();
            ConfigureColorVariationMappings();
            ConfigureSupplierMappings();
            ConfigurePurchaseOrderMappings();
            ConfigurePullOutMappings();
        }

        private void ConfigureShoeMappings()
        {
            // Shoe mappings
            CreateMap<Shoe, ShoeDto>()
                .ForMember(dest => dest.ColorVariations, opt => opt.MapFrom(src => src.ColorVariations));

            CreateMap<CreateShoeDto, Shoe>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.ColorVariations, opt => opt.MapFrom(src => src.ColorVariations));
        }

        private void ConfigureColorVariationMappings()
        {
            // Color Variation mappings
            CreateMap<ShoeColorVariation, ColorVariationDto>();

            CreateMap<CreateColorVariationDto, ShoeColorVariation>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ShoeId, opt => opt.Ignore())
                .ForMember(dest => dest.Shoe, opt => opt.Ignore());
        }

        private void ConfigureSupplierMappings()
        {
            // Supplier mappings (if needed)
            CreateMap<Supplier, SupplierDto>();
            CreateMap<CreateSupplierDto, Supplier>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PurchaseOrders, opt => opt.Ignore());
        }

        private void ConfigurePurchaseOrderMappings()
        {
            // Purchase Order mappings
            CreateMap<PurchaseOrder, PurchaseOrderDto>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items))
                .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier.Name));

            CreateMap<CreatePurchaseOrderDto, PurchaseOrder>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => PurchaseOrderStatus.Pending))
                .ForMember(dest => dest.Supplier, opt => opt.Ignore())
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

            CreateMap<PurchaseOrderItem, PurchaseOrderItemDto>()
                .ForMember(dest => dest.ShoeName, opt => opt.MapFrom(src => src.ShoeColorVariation.Shoe.Name))
                .ForMember(dest => dest.ColorName, opt => opt.MapFrom(src => src.ShoeColorVariation.ColorName));

            CreateMap<CreatePurchaseOrderItemDto, PurchaseOrderItem>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PurchaseOrderId, opt => opt.Ignore())
                .ForMember(dest => dest.ShoeColorVariation, opt => opt.Ignore())
                .ForMember(dest => dest.PurchaseOrder, opt => opt.Ignore())
                .ForMember(dest => dest.QuantityReceived, opt => opt.MapFrom(src => 0));
        }

        private void ConfigurePullOutMappings()
        {
            // Pull-Out mappings
            CreateMap<StockPullOut, PullOutRequestDto>()
                .ForMember(dest => dest.ShoeName, opt => opt.MapFrom(src => src.ShoeColorVariation.Shoe.Name))
                .ForMember(dest => dest.ColorName, opt => opt.MapFrom(src => src.ShoeColorVariation.ColorName));

            CreateMap<CreatePullOutDto, StockPullOut>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PullOutDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => StockPullOutStatus.Pending))
                .ForMember(dest => dest.ShoeColorVariation, opt => opt.Ignore());
        }
    }
}

// Additional DTOs that might be missing
namespace ShoeShop.Services.DTOs.Shoe
{
    public class SupplierDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public string? Address { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateSupplierDto
    {
        public string Name { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public string? Address { get; set; }
        public bool IsActive { get; set; } = true;
    }
}