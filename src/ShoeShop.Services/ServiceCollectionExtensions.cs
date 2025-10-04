using Microsoft.Extensions.DependencyInjection;
using ShoeShop.Services.Interfaces;
using ShoeShop.Services.Services;
using ShoeShop.Services.Mapping;

namespace ShoeShop.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddShoeShopServices(this IServiceCollection services)
        {
            // AutoMapper
            services.AddAutoMapper(typeof(MappingProfile));

            // Service registrations
            services.AddScoped<IInventoryService, InventoryService>();
            // TODO: Complete other service implementations
            // services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
            // services.AddScoped<IPullOutService, PullOutService>();
            // services.AddScoped<IReportService, ReportService>();

            return services;
        }
    }
}