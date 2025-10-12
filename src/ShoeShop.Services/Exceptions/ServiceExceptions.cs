namespace ShoeShop.Services.Exceptions
{
    // Base service exception
    public class ServiceException : Exception
    {
        public ServiceException(string message) : base(message) { }
        public ServiceException(string message, Exception innerException) : base(message, innerException) { }
    }

    // Business validation exceptions
    public class BusinessValidationException : ServiceException
    {
        public BusinessValidationException(string message) : base(message) { }
        public BusinessValidationException(string message, Exception innerException) : base(message, innerException) { }
    }

    // Resource not found exception
    public class ResourceNotFoundException : ServiceException
    {
        public ResourceNotFoundException(string resourceName, object key) 
            : base($"{resourceName} with ID '{key}' was not found.") { }
        
        public ResourceNotFoundException(string message) : base(message) { }
    }

    // Insufficient stock exception
    public class InsufficientStockException : BusinessValidationException
    {
        public int AvailableStock { get; }
        public int RequestedQuantity { get; }

        public InsufficientStockException(int availableStock, int requestedQuantity)
            : base($"Insufficient stock. Available: {availableStock}, Requested: {requestedQuantity}")
        {
            AvailableStock = availableStock;
            RequestedQuantity = requestedQuantity;
        }
    }

    // Duplicate resource exception
    public class DuplicateResourceException : BusinessValidationException
    {
        public DuplicateResourceException(string resourceName, string duplicateField, object value)
            : base($"{resourceName} with {duplicateField} '{value}' already exists.") { }
    }

    // Invalid operation exception
    public class InvalidBusinessOperationException : BusinessValidationException
    {
        public InvalidBusinessOperationException(string operation, string reason)
            : base($"Cannot perform operation '{operation}': {reason}") { }
    }

    // Stock adjustment exception
    public class StockAdjustmentException : BusinessValidationException
    {
        public StockAdjustmentException(string message) : base(message) { }
    }

    // Purchase order exception
    public class PurchaseOrderException : BusinessValidationException
    {
        public PurchaseOrderException(string message) : base(message) { }
    }

    // Pull-out request exception
    public class PullOutException : BusinessValidationException
    {
        public PullOutException(string message) : base(message) { }
    }
}