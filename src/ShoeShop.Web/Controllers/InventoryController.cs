using Microsoft.AspNetCore.Mvc;
using ShoeShop.Service.Interfaces;
using ShoeShop.Service.DTOs;
using ShoeShop.Service.Exceptions;
using ShoeShop.Web.Models;

namespace ShoeShop.Web.Controllers
{
    public class InventoryController : Controller
    {
        private readonly IInventoryService _inventoryService;
        private readonly ILogger<InventoryController> _logger;

        public InventoryController(IInventoryService inventoryService, ILogger<InventoryController> logger)
        {
            _inventoryService = inventoryService;
            _logger = logger;
        }

        // GET: Inventory
        public async Task<IActionResult> Index(string searchQuery = "", string brandFilter = "", 
            string categoryFilter = "", string stockFilter = "", string sortBy = "name", int page = 1, int pageSize = 10)
        {
            try
            {
                var allShoes = await _inventoryService.GetAllShoesAsync();

                // Apply search filter
                if (!string.IsNullOrEmpty(searchQuery))
                {
                    allShoes = allShoes.Where(s => 
                        s.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                        s.Brand.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                        s.SKU.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)
                    );
                }

                // Apply brand filter
                if (!string.IsNullOrEmpty(brandFilter))
                {
                    allShoes = allShoes.Where(s => s.Brand.Equals(brandFilter, StringComparison.OrdinalIgnoreCase));
                }

                // Apply category filter
                if (!string.IsNullOrEmpty(categoryFilter))
                {
                    allShoes = allShoes.Where(s => s.Category.Equals(categoryFilter, StringComparison.OrdinalIgnoreCase));
                }

                // Apply stock filter
                if (!string.IsNullOrEmpty(stockFilter))
                {
                    allShoes = stockFilter.ToLower() switch
                    {
                        "instock" => allShoes.Where(s => s.StockQuantity > 10),
                        "lowstock" => allShoes.Where(s => s.StockQuantity > 0 && s.StockQuantity <= 10),
                        "outofstock" => allShoes.Where(s => s.StockQuantity == 0),
                        _ => allShoes
                    };
                }

                // Apply sorting
                allShoes = sortBy.ToLower() switch
                {
                    "price" => allShoes.OrderBy(s => s.Price),
                    "stock" => allShoes.OrderBy(s => s.StockQuantity),
                    "brand" => allShoes.OrderBy(s => s.Brand),
                    "created" => allShoes.OrderByDescending(s => s.Id),
                    _ => allShoes.OrderBy(s => s.Name)
                };

                var shoesList = allShoes.ToList();

                // Calculate summary metrics
                var viewModel = new InventoryIndexViewModel
                {
                    Shoes = shoesList.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalItems = shoesList.Count,
                    TotalPages = (int)Math.Ceiling((double)shoesList.Count / pageSize),
                    SearchQuery = searchQuery,
                    BrandFilter = brandFilter,
                    CategoryFilter = categoryFilter,
                    StockFilter = stockFilter,
                    SortBy = sortBy,
                    
                    // Summary metrics
                    TotalShoes = shoesList.Count,
                    TotalValue = shoesList.Sum(s => s.Price * s.StockQuantity),
                    LowStockCount = shoesList.Count(s => s.StockQuantity > 0 && s.StockQuantity <= 10),
                    OutOfStockCount = shoesList.Count(s => s.StockQuantity == 0)
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading inventory");
                TempData["ErrorMessage"] = "Unable to load inventory. Please try again.";
                return View(new InventoryIndexViewModel());
            }
        }

        // GET: Inventory/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var shoe = await _inventoryService.GetShoeByIdAsync(id);
                if (shoe == null)
                {
                    TempData["ErrorMessage"] = "Shoe not found.";
                    return RedirectToAction(nameof(Index));
                }

                return View(shoe);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading shoe details for ID: {Id}", id);
                TempData["ErrorMessage"] = "Unable to load shoe details.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Inventory/Create
        public IActionResult Create()
        {
            var model = new ShoeCreateDto();
            PopulateDropdowns();
            return View(model);
        }

        // POST: Inventory/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ShoeCreateDto model)
        {
            if (!ModelState.IsValid)
            {
                PopulateDropdowns();
                return View(model);
            }

            try
            {
                var createdShoe = await _inventoryService.AddShoeAsync(model);
                TempData["SuccessMessage"] = $"Shoe '{createdShoe.Name}' has been added successfully.";
                return RedirectToAction(nameof(Details), new { id = createdShoe.Id });
            }
            catch (DuplicateSkuException ex)
            {
                ModelState.AddModelError("SKU", ex.Message);
                PopulateDropdowns();
                return View(model);
            }
            catch (InvalidPriceException ex)
            {
                ModelState.AddModelError("Price", ex.Message);
                PopulateDropdowns();
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating shoe");
                TempData["ErrorMessage"] = "Unable to create shoe. Please try again.";
                PopulateDropdowns();
                return View(model);
            }
        }

        // GET: Inventory/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var shoe = await _inventoryService.GetShoeByIdAsync(id);
                if (shoe == null)
                {
                    TempData["ErrorMessage"] = "Shoe not found.";
                    return RedirectToAction(nameof(Index));
                }

                var model = new ShoeUpdateDto
                {
                    Id = shoe.Id,
                    Name = shoe.Name,
                    Brand = shoe.Brand,
                    Category = shoe.Category,
                    Size = shoe.Size,
                    Color = shoe.Color,
                    Price = shoe.Price,
                    StockQuantity = shoe.StockQuantity,
                    Description = shoe.Description
                };

                PopulateDropdowns();
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading shoe for editing. ID: {Id}", id);
                TempData["ErrorMessage"] = "Unable to load shoe for editing.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Inventory/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ShoeUpdateDto model)
        {
            if (id != model.Id)
            {
                TempData["ErrorMessage"] = "Invalid shoe ID.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                PopulateDropdowns();
                return View(model);
            }

            try
            {
                var updatedShoe = await _inventoryService.UpdateShoeAsync(model);
                TempData["SuccessMessage"] = $"Shoe '{updatedShoe.Name}' has been updated successfully.";
                return RedirectToAction(nameof(Details), new { id = updatedShoe.Id });
            }
            catch (ShoeNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidPriceException ex)
            {
                ModelState.AddModelError("Price", ex.Message);
                PopulateDropdowns();
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating shoe with ID: {Id}", id);
                TempData["ErrorMessage"] = "Unable to update shoe. Please try again.";
                PopulateDropdowns();
                return View(model);
            }
        }

        // GET: Inventory/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var shoe = await _inventoryService.GetShoeByIdAsync(id);
                if (shoe == null)
                {
                    TempData["ErrorMessage"] = "Shoe not found.";
                    return RedirectToAction(nameof(Index));
                }

                return View(shoe);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading shoe for deletion. ID: {Id}", id);
                TempData["ErrorMessage"] = "Unable to load shoe for deletion.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Inventory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _inventoryService.DeleteShoeAsync(id);
                TempData["SuccessMessage"] = "Shoe has been deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (ShoeNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting shoe with ID: {Id}", id);
                TempData["ErrorMessage"] = "Unable to delete shoe. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Inventory/UpdateStock/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStock(int id, int quantity)
        {
            try
            {
                await _inventoryService.UpdateStockAsync(id, quantity);
                TempData["SuccessMessage"] = "Stock updated successfully.";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (ShoeNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (InsufficientStockException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating stock for shoe ID: {Id}", id);
                TempData["ErrorMessage"] = "Unable to update stock. Please try again.";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        // GET: Inventory/LowStock
        public async Task<IActionResult> LowStock()
        {
            try
            {
                var lowStockShoes = await _inventoryService.GetLowStockShoesAsync(10);
                return View(lowStockShoes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading low stock shoes");
                TempData["ErrorMessage"] = "Unable to load low stock items.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Inventory/Search
        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Json(new List<object>());
            }

            try
            {
                var results = await _inventoryService.SearchShoesAsync(query);
                var searchResults = results.Take(10).Select(s => new
                {
                    id = s.Id,
                    name = s.Name,
                    brand = s.Brand,
                    price = s.Price.ToString("C"),
                    stock = s.StockQuantity
                });

                return Json(searchResults);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching shoes with query: {Query}", query);
                return Json(new List<object>());
            }
        }

        private void PopulateDropdowns()
        {
            ViewBag.Brands = new List<string>
            {
                "Nike", "Adidas", "Puma", "Converse", "Vans", 
                "Jordan", "New Balance", "Reebok", "Under Armour", "ASICS"
            };

            ViewBag.Categories = new List<string>
            {
                "Running", "Basketball", "Casual", "Lifestyle", 
                "Training", "Soccer", "Tennis", "Skateboarding"
            };

            ViewBag.Sizes = new List<string>
            {
                "6", "6.5", "7", "7.5", "8", "8.5", "9", "9.5", 
                "10", "10.5", "11", "11.5", "12", "12.5", "13"
            };
        }
    }
}