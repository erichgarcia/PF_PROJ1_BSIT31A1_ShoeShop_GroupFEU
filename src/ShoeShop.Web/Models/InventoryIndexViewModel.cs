using ShoeShop.Service.DTOs;

namespace ShoeShop.Web.Models
{
    public class InventoryIndexViewModel
    {
        public IEnumerable<ShoeDto> Shoes { get; set; } = new List<ShoeDto>();
        
        // Pagination
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        
        // Filters
        public string SearchQuery { get; set; } = string.Empty;
        public string BrandFilter { get; set; } = string.Empty;
        public string CategoryFilter { get; set; } = string.Empty;
        public string StockFilter { get; set; } = string.Empty;
        public string SortBy { get; set; } = "name";
        
        // Summary metrics
        public int TotalShoes { get; set; }
        public decimal TotalValue { get; set; }
        public int LowStockCount { get; set; }
        public int OutOfStockCount { get; set; }
        
        // Helper properties for pagination
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
        public int PreviousPage => CurrentPage - 1;
        public int NextPage => CurrentPage + 1;
        
        // Helper method for getting page numbers for pagination
        public IEnumerable<int> GetPageNumbers()
        {
            var startPage = Math.Max(1, CurrentPage - 2);
            var endPage = Math.Min(TotalPages, CurrentPage + 2);
            
            for (int i = startPage; i <= endPage; i++)
            {
                yield return i;
            }
        }
    }
}