using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoeShop.Web.Models;
using ShoeShop.Services.Interfaces;
using ShoeShop.Services.DTOs.Simple;

namespace ShoeShop.Web.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ISimpleInventoryService _inventoryService;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ISimpleInventoryService inventoryService, ILogger<HomeController> logger)
    {
        _inventoryService = inventoryService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        // Redirect to Dashboard as the main landing page
        return RedirectToAction("Dashboard");
    }

    public async Task<IActionResult> Dashboard()
    {
        try
        {
            var viewModel = new DashboardViewModel();
            
            // Get all inventory items for dashboard metrics
            var allShoes = await _inventoryService.GetAllShoesAsync();
            
            // Calculate dashboard metrics
            viewModel.TotalShoes = allShoes.Count();
            viewModel.TotalValue = allShoes.Sum(s => s.Price * s.StockQuantity);
            viewModel.LowStockCount = allShoes.Count(s => s.StockQuantity > 0 && s.StockQuantity <= 10);
            viewModel.OutOfStockCount = allShoes.Count(s => s.StockQuantity == 0);
            
            // Get recent activity (last 5 items sorted by ID descending as proxy for recent)
            viewModel.RecentActivities = allShoes
                .OrderByDescending(s => s.Id)
                .Take(5)
                .Select(s => new ActivityItem
                {
                    Type = "Stock Available",
                    Description = $"{s.Name} - {s.StockQuantity} units in inventory",
                    Timestamp = DateTime.Now.AddHours(-new Random().Next(1, 48)),
                    Icon = "fas fa-box",
                    Color = s.StockQuantity > 10 ? "success" : s.StockQuantity > 0 ? "warning" : "danger"
                }).ToList();

            // Get low stock alerts
            viewModel.StockAlerts = allShoes
                .Where(s => s.StockQuantity <= 10)
                .OrderBy(s => s.StockQuantity)
                .Take(4)
                .Select(s => new StockAlert
                {
                    ShoeName = s.Name,
                    StockQuantity = s.StockQuantity,
                    Status = s.StockQuantity == 0 ? "Out of Stock" : "Low Stock",
                    AlertType = s.StockQuantity == 0 ? "danger" : "warning"
                }).ToList();

            // Get top categories
            viewModel.TopCategories = allShoes
                .GroupBy(s => s.Category)
                .Select(g => new CategorySummary
                {
                    Category = g.Key,
                    TotalUnits = g.Sum(s => s.StockQuantity),
                    TotalValue = g.Sum(s => s.Price * s.StockQuantity),
                    TopBrand = g.GroupBy(s => s.Brand)
                               .OrderByDescending(bg => bg.Count())
                               .First().Key,
                    StockStatus = g.Sum(s => s.StockQuantity) > 50 ? "Good" : 
                                 g.Sum(s => s.StockQuantity) > 20 ? "Low" : "Critical"
                })
                .OrderByDescending(c => c.TotalValue)
                .Take(4)
                .ToList();

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading dashboard");
            ViewBag.ErrorMessage = "Unable to load dashboard data. Please try again.";
            return View(new DashboardViewModel());
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
