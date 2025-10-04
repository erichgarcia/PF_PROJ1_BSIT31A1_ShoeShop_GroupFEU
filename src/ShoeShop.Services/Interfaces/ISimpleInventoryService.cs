using ShoeShop.Services.DTOs.Simple;

namespace ShoeShop.Services.Interfaces
{
    public interface ISimpleInventoryService
    {
        // Basic CRUD operations
        Task<IEnumerable<SimpleShoeDto>> GetAllShoesAsync();
        Task<SimpleShoeDto?> GetShoeByIdAsync(int id);
        Task<SimpleShoeDto> AddShoeAsync(SimpleShoeCreateDto createDto);
        Task<SimpleShoeDto> UpdateShoeAsync(SimpleShoeUpdateDto updateDto);
        Task DeleteShoeAsync(int id);
        
        // Stock management
        Task UpdateStockAsync(int id, int quantity);
        Task<IEnumerable<SimpleShoeDto>> GetLowStockShoesAsync(int threshold = 10);
        
        // Search and filtering
        Task<IEnumerable<SimpleShoeDto>> SearchShoesAsync(string query);
        Task<IEnumerable<SimpleShoeDto>> GetShoesByBrandAsync(string brand);
        Task<IEnumerable<SimpleShoeDto>> GetShoesByCategoryAsync(string category);
    }
}