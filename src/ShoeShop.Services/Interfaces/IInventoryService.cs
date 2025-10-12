using ShoeShop.Services.DTOs.Shoe;

namespace ShoeShop.Services.Interfaces
{
    public interface IInventoryService
    {
        // Shoe Management
        Task<IEnumerable<ShoeDto>> GetAllShoesAsync();
        Task<IEnumerable<ShoeDto>> GetShoesAsync(int page, int pageSize, string? searchTerm = null, string? brandFilter = null);
        Task<ShoeDto?> GetShoeByIdAsync(int id);
        Task<ShoeDto> CreateShoeAsync(CreateShoeDto createShoeDto);
        Task<ShoeDto> UpdateShoeAsync(int id, CreateShoeDto updateShoeDto);
        Task<bool> DeleteShoeAsync(int id);

        // Color Variation Management
        Task<IEnumerable<ColorVariationDto>> GetColorVariationsAsync(int shoeId);
        Task<ColorVariationDto?> GetColorVariationByIdAsync(int id);
        Task<ColorVariationDto> CreateColorVariationAsync(int shoeId, CreateColorVariationDto createColorVariationDto);
        Task<ColorVariationDto> UpdateColorVariationAsync(int id, CreateColorVariationDto updateColorVariationDto);
        Task<bool> DeleteColorVariationAsync(int id);

        // Stock Management
        Task<bool> AdjustStockAsync(int colorVariationId, int quantityChange, string reason, string userId);
        Task<bool> UpdateStockQuantityAsync(int colorVariationId, int newQuantity, string reason, string userId);
        Task<IEnumerable<ColorVariationDto>> GetLowStockItemsAsync();
        Task<IEnumerable<ColorVariationDto>> GetOutOfStockItemsAsync();

        // Validation and Business Rules
        Task<bool> ValidateStockAvailabilityAsync(int colorVariationId, int requestedQuantity);
        Task<bool> IsValidShoeDataAsync(CreateShoeDto shoeDto);
        Task<bool> ShoeNameExistsAsync(string name, string brand, int? excludeId = null);

        // Search and Filtering
        Task<IEnumerable<ShoeDto>> SearchShoesAsync(string searchTerm);
        Task<IEnumerable<string>> GetUniqueBrandsAsync();
        Task<IEnumerable<ShoeDto>> GetShoesByBrandAsync(string brand);
        Task<IEnumerable<ColorVariationDto>> GetColorVariationsByStockStatusAsync(string status); // "low", "out", "good"
    }
}