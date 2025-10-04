using Microsoft.AspNetCore.Mvc;
using ShoeShop.Service.Interfaces;
using ShoeShop.Web.Models;

namespace ShoeShop.Web.Controllers
{
    public class ReportsController : Controller
    {
        private readonly IInventoryService _inventoryService;
        private readonly ILogger<ReportsController> _logger;

        public ReportsController(IInventoryService inventoryService, ILogger<ReportsController> logger)
        {
            _inventoryService = inventoryService;
            _logger = logger;
        }

        // GET: Reports
        public async Task<IActionResult> Index()
        {
            try
            {
                var allShoes = await _inventoryService.GetAllShoesAsync();
                var viewModel = new ReportsViewModel();

                // Calculate overview metrics
                viewModel.TotalProducts = allShoes.Count();
                viewModel.TotalValue = allShoes.Sum(s => s.Price * s.StockQuantity);
                viewModel.AveragePrice = allShoes.Any() ? allShoes.Average(s => s.Price) : 0;
                viewModel.TotalStockUnits = allShoes.Sum(s => s.StockQuantity);

                // Brand analysis
                viewModel.BrandReport = allShoes
                    .GroupBy(s => s.Brand)
                    .Select(g => new BrandReport
                    {
                        Brand = g.Key,
                        ProductCount = g.Count(),
                        TotalValue = g.Sum(s => s.Price * s.StockQuantity),
                        AveragePrice = g.Average(s => s.Price),
                        TotalStock = g.Sum(s => s.StockQuantity)
                    })
                    .OrderByDescending(b => b.TotalValue)
                    .ToList();

                // Category analysis
                viewModel.CategoryReport = allShoes
                    .GroupBy(s => s.Category)
                    .Select(g => new CategoryReport
                    {
                        Category = g.Key,
                        ProductCount = g.Count(),
                        TotalValue = g.Sum(s => s.Price * s.StockQuantity),
                        AveragePrice = g.Average(s => s.Price),
                        TotalStock = g.Sum(s => s.StockQuantity)
                    })
                    .OrderByDescending(c => c.TotalValue)
                    .ToList();

                // Stock status report
                viewModel.StockReport = new StockStatusReport
                {
                    InStockCount = allShoes.Count(s => s.StockQuantity > 10),
                    LowStockCount = allShoes.Count(s => s.StockQuantity > 0 && s.StockQuantity <= 10),
                    OutOfStockCount = allShoes.Count(s => s.StockQuantity == 0),
                    TotalProducts = allShoes.Count()
                };

                // Price range analysis
                viewModel.PriceRanges = new List<PriceRangeReport>
                {
                    new PriceRangeReport { Range = "Under ₱2,000", Count = allShoes.Count(s => s.Price < 2000) },
                    new PriceRangeReport { Range = "₱2,000 - ₱5,000", Count = allShoes.Count(s => s.Price >= 2000 && s.Price < 5000) },
                    new PriceRangeReport { Range = "₱5,000 - ₱8,000", Count = allShoes.Count(s => s.Price >= 5000 && s.Price < 8000) },
                    new PriceRangeReport { Range = "₱8,000 - ₱12,000", Count = allShoes.Count(s => s.Price >= 8000 && s.Price < 12000) },
                    new PriceRangeReport { Range = "₱12,000+", Count = allShoes.Count(s => s.Price >= 12000) }
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating reports");
                TempData["ErrorMessage"] = "Unable to generate reports. Please try again.";
                return View(new ReportsViewModel());
            }
        }

        // GET: Reports/LowStock
        public async Task<IActionResult> LowStock()
        {
            try
            {
                var lowStockShoes = await _inventoryService.GetLowStockShoesAsync(10);
                return View(lowStockShoes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading low stock report");
                TempData["ErrorMessage"] = "Unable to load low stock report.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Reports/ValueAnalysis
        public async Task<IActionResult> ValueAnalysis()
        {
            try
            {
                var allShoes = await _inventoryService.GetAllShoesAsync();
                
                var valueAnalysis = new ValueAnalysisViewModel
                {
                    TopValueProducts = allShoes
                        .OrderByDescending(s => s.Price * s.StockQuantity)
                        .Take(10)
                        .ToList(),
                        
                    HighestPricedProducts = allShoes
                        .OrderByDescending(s => s.Price)
                        .Take(10)
                        .ToList(),
                        
                    MostStockedProducts = allShoes
                        .OrderByDescending(s => s.StockQuantity)
                        .Take(10)
                        .ToList()
                };

                return View(valueAnalysis);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading value analysis");
                TempData["ErrorMessage"] = "Unable to load value analysis.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Reports/Export
        public async Task<IActionResult> Export(string format = "csv")
        {
            try
            {
                var allShoes = await _inventoryService.GetAllShoesAsync();
                
                if (format.ToLower() == "csv")
                {
                    var csv = GenerateCsvReport(allShoes);
                    var fileName = $"inventory_report_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                    return File(System.Text.Encoding.UTF8.GetBytes(csv), "text/csv", fileName);
                }

                // For now, default to CSV. Could add Excel, PDF export later
                TempData["InfoMessage"] = "Only CSV export is currently supported.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting report");
                TempData["ErrorMessage"] = "Unable to export report. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        private string GenerateCsvReport(IEnumerable<ShoeShop.Service.DTOs.ShoeDto> shoes)
        {
            var csv = new System.Text.StringBuilder();
            csv.AppendLine("ID,Name,Brand,Category,Size,Color,Price,Stock Quantity,Description,SKU");

            foreach (var shoe in shoes)
            {
                csv.AppendLine($"{shoe.Id}," +
                             $"\"{shoe.Name}\"," +
                             $"\"{shoe.Brand}\"," +
                             $"\"{shoe.Category}\"," +
                             $"\"{shoe.Size}\"," +
                             $"\"{shoe.Color}\"," +
                             $"{shoe.Price}," +
                             $"{shoe.StockQuantity}," +
                             $"\"{shoe.Description?.Replace("\"", "\"\"")}\"," +
                             $"\"{shoe.SKU}\"");
            }

            return csv.ToString();
        }
    }
}