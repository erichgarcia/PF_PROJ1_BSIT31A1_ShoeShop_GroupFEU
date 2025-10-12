using ShoeShop.Repository.Models;
using ShoeShop.Services.DTOs.PullOut;

namespace ShoeShop.Services.Interfaces
{
    public interface IPullOutService
    {
        // Pull-Out Request Management
        Task<IEnumerable<PullOutRequestDto>> GetAllPullOutRequestsAsync();
        Task<IEnumerable<PullOutRequestDto>> GetPullOutRequestsAsync(int page, int pageSize, StockPullOutStatus? statusFilter = null);
        Task<PullOutRequestDto?> GetPullOutRequestByIdAsync(int id);
        Task<PullOutRequestDto> CreatePullOutRequestAsync(CreatePullOutDto createPullOutDto);
        Task<bool> UpdatePullOutRequestAsync(int id, CreatePullOutDto updatePullOutDto);
        Task<bool> DeletePullOutRequestAsync(int id);

        // Approval Workflow
        Task<bool> ApprovePullOutRequestAsync(int id, string approvedBy, string? approvalNotes = null);
        Task<bool> RejectPullOutRequestAsync(int id, string rejectedBy, string rejectionReason);
        Task<bool> CompletePullOutRequestAsync(int id, string completedBy);
        Task<IEnumerable<PullOutRequestDto>> GetPendingApprovalRequestsAsync();

        // Business Logic and Validation
        Task<bool> ValidatePullOutRequestAsync(CreatePullOutDto pullOutDto);
        Task<bool> RequiresManagerApprovalAsync(CreatePullOutDto pullOutDto);
        Task<bool> HasSufficientStockAsync(int colorVariationId, int requestedQuantity);
        Task<int> GetAvailableStockAsync(int colorVariationId);

        // Stock Impact Processing
        Task<bool> ProcessPullOutImpactAsync(int pullOutRequestId);
        Task<bool> ReversePullOutImpactAsync(int pullOutRequestId, string reason, string userId);

        // Reporting and Analytics
        Task<IEnumerable<PullOutRequestDto>> GetPullOutsByDateRangeAsync(DateTime fromDate, DateTime toDate);
        Task<IEnumerable<PullOutRequestDto>> GetPullOutsByReasonAsync(string reason);
        Task<IEnumerable<PullOutRequestDto>> GetPullOutsByUserAsync(string userId);
        Task<IEnumerable<PullOutRequestDto>> GetOverduePullOutRequestsAsync();

        // Dashboard and Alerts
        Task<int> GetPendingPullOutCountAsync();
        Task<IEnumerable<PullOutRequestDto>> GetRecentPullOutsAsync(int count = 10);
        Task<Dictionary<string, int>> GetPullOutReasonSummaryAsync(int months = 6);

        // Audit and History
        Task<IEnumerable<PullOutRequestDto>> GetPullOutHistoryAsync(int colorVariationId);
        Task<decimal> GetTotalPullOutValueAsync(DateTime fromDate, DateTime toDate);

        // Configuration and Settings
        Task<int> GetApprovalThresholdQuantityAsync();
        Task<bool> SetApprovalThresholdQuantityAsync(int threshold, string userId);
        Task<List<string>> GetCommonPullOutReasonsAsync();
    }
}