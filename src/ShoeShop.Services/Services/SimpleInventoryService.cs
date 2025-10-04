using Microsoft.EntityFrameworkCore;
using ShoeShop.Repository.Data;
using ShoeShop.Repository.Models;
using ShoeShop.Services.DTOs.Simple;
using ShoeShop.Services.Interfaces;

namespace ShoeShop.Services.Services
{
    public class SimpleInventoryService : ISimpleInventoryService
    {
        private readonly ShoeShopDbContext _context;

        public SimpleInventoryService(ShoeShopDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SimpleShoeDto>> GetAllShoesAsync()
        {
            var shoes = await _context.Shoes
                .Where(s => s.IsActive)
                .OrderBy(s => s.Name)
                .ToListAsync();

            return shoes.Select(s => MapToDto(s));
        }

        public async Task<SimpleShoeDto?> GetShoeByIdAsync(int id)
        {
            var shoe = await _context.Shoes.FindAsync(id);
            return shoe != null ? MapToDto(shoe) : null;
        }

        public async Task<SimpleShoeDto> AddShoeAsync(SimpleShoeCreateDto createDto)
        {
            var shoe = new Shoe
            {
                Name = createDto.Name,
                Brand = createDto.Brand,
                Category = createDto.Category,
                Size = createDto.Size,
                Color = createDto.Color,
                SKU = createDto.SKU,
                StockQuantity = createDto.StockQuantity,
                Price = createDto.Price,
                Description = createDto.Description,
                IsActive = createDto.IsActive,
                CreatedDate = DateTime.Now
            };

            _context.Shoes.Add(shoe);
            await _context.SaveChangesAsync();

            return MapToDto(shoe);
        }

        public async Task<SimpleShoeDto> UpdateShoeAsync(SimpleShoeUpdateDto updateDto)
        {
            var shoe = await _context.Shoes.FindAsync(updateDto.Id);
            if (shoe == null)
                throw new ArgumentException($"Shoe with ID {updateDto.Id} not found");

            shoe.Name = updateDto.Name;
            shoe.Brand = updateDto.Brand;
            shoe.Category = updateDto.Category;
            shoe.Size = updateDto.Size;
            shoe.Color = updateDto.Color;
            shoe.SKU = updateDto.SKU;
            shoe.StockQuantity = updateDto.StockQuantity;
            shoe.Price = updateDto.Price;
            shoe.Description = updateDto.Description;
            shoe.IsActive = updateDto.IsActive;

            await _context.SaveChangesAsync();
            return MapToDto(shoe);
        }

        public async Task DeleteShoeAsync(int id)
        {
            var shoe = await _context.Shoes.FindAsync(id);
            if (shoe != null)
            {
                shoe.IsActive = false; // Soft delete
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateStockAsync(int id, int quantity)
        {
            var shoe = await _context.Shoes.FindAsync(id);
            if (shoe != null)
            {
                shoe.StockQuantity = quantity;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<SimpleShoeDto>> GetLowStockShoesAsync(int threshold = 10)
        {
            var shoes = await _context.Shoes
                .Where(s => s.IsActive && s.StockQuantity <= threshold)
                .OrderBy(s => s.StockQuantity)
                .ToListAsync();

            return shoes.Select(s => MapToDto(s));
        }

        public async Task<IEnumerable<SimpleShoeDto>> SearchShoesAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return await GetAllShoesAsync();

            var shoes = await _context.Shoes
                .Where(s => s.IsActive && 
                           (s.Name.Contains(query) || 
                            s.Brand.Contains(query) || 
                            s.SKU.Contains(query) ||
                            s.Category.Contains(query)))
                .OrderBy(s => s.Name)
                .ToListAsync();

            return shoes.Select(s => MapToDto(s));
        }

        public async Task<IEnumerable<SimpleShoeDto>> GetShoesByBrandAsync(string brand)
        {
            var shoes = await _context.Shoes
                .Where(s => s.IsActive && s.Brand == brand)
                .OrderBy(s => s.Name)
                .ToListAsync();

            return shoes.Select(s => MapToDto(s));
        }

        public async Task<IEnumerable<SimpleShoeDto>> GetShoesByCategoryAsync(string category)
        {
            var shoes = await _context.Shoes
                .Where(s => s.IsActive && s.Category == category)
                .OrderBy(s => s.Name)
                .ToListAsync();

            return shoes.Select(s => MapToDto(s));
        }

        private SimpleShoeDto MapToDto(Shoe shoe)
        {
            return new SimpleShoeDto
            {
                Id = shoe.Id,
                Name = shoe.Name,
                Brand = shoe.Brand,
                Category = shoe.Category,
                Size = shoe.Size,
                Color = shoe.Color,
                SKU = shoe.SKU,
                StockQuantity = shoe.StockQuantity,
                Price = shoe.Price,
                Description = shoe.Description,
                IsActive = shoe.IsActive,
                CreatedDate = shoe.CreatedDate
            };
        }
    }
}