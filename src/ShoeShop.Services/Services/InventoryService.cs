using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ShoeShop.Repository.Data;
using ShoeShop.Repository.Models;
using ShoeShop.Services.DTOs.Shoe;
using ShoeShop.Services.Exceptions;
using ShoeShop.Services.Interfaces;

namespace ShoeShop.Services.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly ShoeShopDbContext _context;
        private readonly IMapper _mapper;

        public InventoryService(ShoeShopDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // Shoe Management
        public async Task<IEnumerable<ShoeDto>> GetAllShoesAsync()
        {
            var shoes = await _context.Shoes
                .Include(s => s.ColorVariations)
                .Where(s => s.IsActive)
                .OrderBy(s => s.Brand)
                .ThenBy(s => s.Name)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ShoeDto>>(shoes);
        }

        public async Task<IEnumerable<ShoeDto>> GetShoesAsync(int page, int pageSize, string? searchTerm = null, string? brandFilter = null)
        {
            var query = _context.Shoes
                .Include(s => s.ColorVariations)
                .Where(s => s.IsActive);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(s => s.Name.Contains(searchTerm) || s.Brand.Contains(searchTerm));
            }

            if (!string.IsNullOrWhiteSpace(brandFilter))
            {
                query = query.Where(s => s.Brand == brandFilter);
            }

            var shoes = await query
                .OrderBy(s => s.Brand)
                .ThenBy(s => s.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ShoeDto>>(shoes);
        }

        public async Task<ShoeDto?> GetShoeByIdAsync(int id)
        {
            var shoe = await _context.Shoes
                .Include(s => s.ColorVariations)
                .FirstOrDefaultAsync(s => s.Id == id && s.IsActive);

            return shoe != null ? _mapper.Map<ShoeDto>(shoe) : null;
        }

        public async Task<ShoeDto> CreateShoeAsync(CreateShoeDto createShoeDto)
        {
            // Validate business rules
            if (!createShoeDto.IsValidPricing())
                throw new BusinessValidationException("Selling price must be higher than cost price.");

            // Check for duplicate
            if (await ShoeNameExistsAsync(createShoeDto.Name, createShoeDto.Brand))
                throw new DuplicateResourceException("Shoe", "Name + Brand", $"{createShoeDto.Name} ({createShoeDto.Brand})");

            var shoe = _mapper.Map<Shoe>(createShoeDto);
            
            _context.Shoes.Add(shoe);
            await _context.SaveChangesAsync();

            return _mapper.Map<ShoeDto>(shoe);
        }

        public async Task<ShoeDto> UpdateShoeAsync(int id, CreateShoeDto updateShoeDto)
        {
            var shoe = await _context.Shoes
                .Include(s => s.ColorVariations)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (shoe == null)
                throw new ResourceNotFoundException("Shoe", id);

            if (!updateShoeDto.IsValidPricing())
                throw new BusinessValidationException("Selling price must be higher than cost price.");

            if (await ShoeNameExistsAsync(updateShoeDto.Name, updateShoeDto.Brand, id))
                throw new DuplicateResourceException("Shoe", "Name + Brand", $"{updateShoeDto.Name} ({updateShoeDto.Brand})");

            _mapper.Map(updateShoeDto, shoe);
            await _context.SaveChangesAsync();

            return _mapper.Map<ShoeDto>(shoe);
        }

        public async Task<bool> DeleteShoeAsync(int id)
        {
            var shoe = await _context.Shoes.FindAsync(id);
            if (shoe == null) return false;

            shoe.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        // Color Variation Management
        public async Task<IEnumerable<ColorVariationDto>> GetColorVariationsAsync(int shoeId)
        {
            var variations = await _context.ShoeColorVariations
                .Where(cv => cv.ShoeId == shoeId && cv.IsActive)
                .OrderBy(cv => cv.ColorName)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ColorVariationDto>>(variations);
        }

        public async Task<ColorVariationDto?> GetColorVariationByIdAsync(int id)
        {
            var variation = await _context.ShoeColorVariations
                .FirstOrDefaultAsync(cv => cv.Id == id && cv.IsActive);

            return variation != null ? _mapper.Map<ColorVariationDto>(variation) : null;
        }

        public async Task<ColorVariationDto> CreateColorVariationAsync(int shoeId, CreateColorVariationDto createColorVariationDto)
        {
            // Check if shoe exists
            var shoe = await _context.Shoes.FindAsync(shoeId);
            if (shoe == null)
                throw new ResourceNotFoundException("Shoe", shoeId);

            // Check for duplicate color for this shoe
            var existingVariation = await _context.ShoeColorVariations
                .FirstOrDefaultAsync(cv => cv.ShoeId == shoeId && cv.ColorName == createColorVariationDto.ColorName);
            
            if (existingVariation != null)
                throw new DuplicateResourceException("Color Variation", "Color", createColorVariationDto.ColorName);

            var variation = _mapper.Map<ShoeColorVariation>(createColorVariationDto);
            variation.ShoeId = shoeId;

            _context.ShoeColorVariations.Add(variation);
            await _context.SaveChangesAsync();

            return _mapper.Map<ColorVariationDto>(variation);
        }

        public async Task<ColorVariationDto> UpdateColorVariationAsync(int id, CreateColorVariationDto updateColorVariationDto)
        {
            var variation = await _context.ShoeColorVariations.FindAsync(id);
            if (variation == null)
                throw new ResourceNotFoundException("Color Variation", id);

            _mapper.Map(updateColorVariationDto, variation);
            await _context.SaveChangesAsync();

            return _mapper.Map<ColorVariationDto>(variation);
        }

        public async Task<bool> DeleteColorVariationAsync(int id)
        {
            var variation = await _context.ShoeColorVariations.FindAsync(id);
            if (variation == null) return false;

            variation.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        // Stock Management
        public async Task<bool> AdjustStockAsync(int colorVariationId, int quantityChange, string reason, string userId)
        {
            var variation = await _context.ShoeColorVariations.FindAsync(colorVariationId);
            if (variation == null)
                throw new ResourceNotFoundException("Color Variation", colorVariationId);

            var newQuantity = variation.StockQuantity + quantityChange;
            if (newQuantity < 0)
                throw new StockAdjustmentException($"Cannot adjust stock. Would result in negative quantity ({newQuantity}).");

            variation.StockQuantity = newQuantity;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateStockQuantityAsync(int colorVariationId, int newQuantity, string reason, string userId)
        {
            if (newQuantity < 0)
                throw new StockAdjustmentException("Stock quantity cannot be negative.");

            var variation = await _context.ShoeColorVariations.FindAsync(colorVariationId);
            if (variation == null)
                throw new ResourceNotFoundException("Color Variation", colorVariationId);

            variation.StockQuantity = newQuantity;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<ColorVariationDto>> GetLowStockItemsAsync()
        {
            var lowStockItems = await _context.ShoeColorVariations
                .Include(cv => cv.Shoe)
                .Where(cv => cv.IsActive && cv.StockQuantity <= cv.ReorderLevel)
                .OrderBy(cv => cv.StockQuantity)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ColorVariationDto>>(lowStockItems);
        }

        public async Task<IEnumerable<ColorVariationDto>> GetOutOfStockItemsAsync()
        {
            var outOfStockItems = await _context.ShoeColorVariations
                .Include(cv => cv.Shoe)
                .Where(cv => cv.IsActive && cv.StockQuantity == 0)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ColorVariationDto>>(outOfStockItems);
        }

        // Validation and Business Rules
        public async Task<bool> ValidateStockAvailabilityAsync(int colorVariationId, int requestedQuantity)
        {
            var variation = await _context.ShoeColorVariations.FindAsync(colorVariationId);
            return variation != null && variation.StockQuantity >= requestedQuantity;
        }

        public async Task<bool> IsValidShoeDataAsync(CreateShoeDto shoeDto)
        {
            return shoeDto.IsValidPricing() && 
                   !await ShoeNameExistsAsync(shoeDto.Name, shoeDto.Brand);
        }

        public async Task<bool> ShoeNameExistsAsync(string name, string brand, int? excludeId = null)
        {
            var query = _context.Shoes.Where(s => s.Name == name && s.Brand == brand && s.IsActive);
            
            if (excludeId.HasValue)
                query = query.Where(s => s.Id != excludeId.Value);

            return await query.AnyAsync();
        }

        // Search and Filtering
        public async Task<IEnumerable<ShoeDto>> SearchShoesAsync(string searchTerm)
        {
            var shoes = await _context.Shoes
                .Include(s => s.ColorVariations)
                .Where(s => s.IsActive && 
                           (s.Name.Contains(searchTerm) || 
                            s.Brand.Contains(searchTerm) || 
                            s.Description != null && s.Description.Contains(searchTerm)))
                .ToListAsync();

            return _mapper.Map<IEnumerable<ShoeDto>>(shoes);
        }

        public async Task<IEnumerable<string>> GetUniqueBrandsAsync()
        {
            return await _context.Shoes
                .Where(s => s.IsActive)
                .Select(s => s.Brand)
                .Distinct()
                .OrderBy(b => b)
                .ToListAsync();
        }

        public async Task<IEnumerable<ShoeDto>> GetShoesByBrandAsync(string brand)
        {
            var shoes = await _context.Shoes
                .Include(s => s.ColorVariations)
                .Where(s => s.IsActive && s.Brand == brand)
                .OrderBy(s => s.Name)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ShoeDto>>(shoes);
        }

        public async Task<IEnumerable<ColorVariationDto>> GetColorVariationsByStockStatusAsync(string status)
        {
            IQueryable<ShoeColorVariation> query = _context.ShoeColorVariations
                .Include(cv => cv.Shoe)
                .Where(cv => cv.IsActive);

            query = status.ToLower() switch
            {
                "low" => query.Where(cv => cv.StockQuantity <= cv.ReorderLevel && cv.StockQuantity > 0),
                "out" => query.Where(cv => cv.StockQuantity == 0),
                "good" => query.Where(cv => cv.StockQuantity > cv.ReorderLevel),
                _ => query
            };

            var variations = await query.OrderBy(cv => cv.StockQuantity).ToListAsync();
            return _mapper.Map<IEnumerable<ColorVariationDto>>(variations);
        }
    }
}