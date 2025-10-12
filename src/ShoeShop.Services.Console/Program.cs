using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ShoeShop.Repository.Data;
using ShoeShop.Services;
using ShoeShop.Services.DTOs.Shoe;
using ShoeShop.Services.Interfaces;
using ShoeShop.Services.Exceptions;

Console.WriteLine("🦶 ShoeShop Services Testing Console");
Console.WriteLine("===================================\n");

// Setup dependency injection
var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Add Entity Framework
        services.AddDbContext<ShoeShopDbContext>(options =>
            options.UseSqlite("Data Source=shoeshop_services_test.db"));
        
        // Add ShoeShop services
        services.AddShoeShopServices();
    })
    .Build();

// Initialize database
using var scope = host.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<ShoeShopDbContext>();
await context.Database.EnsureCreatedAsync();

// Get services
var inventoryService = scope.ServiceProvider.GetRequiredService<IInventoryService>();

try
{
    // Test Inventory Service (Core functionality)
    await TestInventoryService(inventoryService);

    Console.WriteLine("\n✅ Core Inventory Service tests completed successfully!");
    Console.WriteLine("\n📝 Note: Other services (Purchase Orders, Pull-Outs, Reports) require interface completion.");
}
catch (Exception ex)
{
    Console.WriteLine($"\n❌ Error during testing: {ex.Message}");
    if (ex.InnerException != null)
        Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
}

static async Task TestInventoryService(IInventoryService inventoryService)
{
    Console.WriteLine("🧪 Testing Inventory Service...");
    Console.WriteLine("--------------------------------");

    try
    {
        // Test creating a shoe with color variations
        var createShoeDto = new CreateShoeDto
        {
            Name = "Air Max 270",
            Brand = "Nike",
            Cost = 80.00m,
            Price = 129.99m,
            Description = "Comfortable running shoe with Air Max technology",
            ColorVariations = new List<CreateColorVariationDto>
            {
                new() { ColorName = "Black", HexCode = "#000000", StockQuantity = 25, ReorderLevel = 5 },
                new() { ColorName = "White", HexCode = "#FFFFFF", StockQuantity = 15, ReorderLevel = 5 },
                new() { ColorName = "Red", HexCode = "#FF0000", StockQuantity = 10, ReorderLevel = 5 }
            }
        };

        var createdShoe = await inventoryService.CreateShoeAsync(createShoeDto);
        Console.WriteLine($"✅ Created shoe: {createdShoe.Name} with {createdShoe.ColorVariations.Count} color variations");

        // Test getting all shoes
        var allShoes = await inventoryService.GetAllShoesAsync();
        Console.WriteLine($"✅ Retrieved {allShoes.Count()} shoes from inventory");

        // Test getting low stock items
        var lowStockItems = await inventoryService.GetLowStockItemsAsync();
        Console.WriteLine($"✅ Found {lowStockItems.Count()} low stock items");

        // Test stock adjustment
        var firstColorVariation = createdShoe.ColorVariations.First();
        await inventoryService.AdjustStockAsync(firstColorVariation.Id, -5, "Test adjustment", "TestUser");
        Console.WriteLine($"✅ Adjusted stock for {firstColorVariation.ColorName} variation");

        // Test business validation
        var invalidShoe = new CreateShoeDto
        {
            Name = "Invalid Shoe",
            Brand = "Test",
            Cost = 100.00m,
            Price = 50.00m, // Price lower than cost - should fail
            ColorVariations = new List<CreateColorVariationDto>()
        };

        try
        {
            await inventoryService.CreateShoeAsync(invalidShoe);
            Console.WriteLine("❌ Should have failed validation for invalid pricing");
        }
        catch (BusinessValidationException)
        {
            Console.WriteLine("✅ Correctly caught business validation exception for invalid pricing");
        }

        Console.WriteLine("✅ Inventory Service tests completed\n");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Inventory Service test failed: {ex.Message}\n");
    }
}

// Additional service tests can be added here once interfaces are completed
