// Admin App JavaScript Functions
document.addEventListener('DOMContentLoaded', function() {
    
    // Mobile menu toggle functionality
    const mobileToggle = document.createElement('button');
    mobileToggle.className = 'mobile-menu-toggle';
    mobileToggle.innerHTML = '<i class="fas fa-bars"></i>';
    document.body.appendChild(mobileToggle);
    
    const sidebar = document.querySelector('.sidebar');
    
    mobileToggle.addEventListener('click', function() {
        sidebar.classList.toggle('show');
    });
    
    // Close sidebar when clicking outside on mobile
    document.addEventListener('click', function(event) {
        if (window.innerWidth <= 768) {
            if (!sidebar.contains(event.target) && !mobileToggle.contains(event.target)) {
                sidebar.classList.remove('show');
            }
        }
    });
    
    // Handle window resize
    window.addEventListener('resize', function() {
        if (window.innerWidth > 768) {
            sidebar.classList.remove('show');
        }
    });
    
    // Set active navigation item based on current page
    const currentPath = window.location.pathname;
    const navLinks = document.querySelectorAll('.sidebar .nav-link');
    
    navLinks.forEach(link => {
        const href = link.getAttribute('href');
        if (currentPath.startsWith(href) && href !== '/') {
            link.classList.add('active');
        } else if (currentPath === '/' && href === '/Home/Dashboard') {
            link.classList.add('active');
        }
    });
    
    // Initialize tooltips
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });
    
    // Auto-hide alerts after 5 seconds
    const alerts = document.querySelectorAll('.alert:not(.alert-permanent)');
    alerts.forEach(alert => {
        setTimeout(() => {
            const bsAlert = new bootstrap.Alert(alert);
            bsAlert.close();
        }, 5000);
    });
    
    // Form validation enhancement
    const forms = document.querySelectorAll('.needs-validation');
    forms.forEach(form => {
        form.addEventListener('submit', function(event) {
            if (!form.checkValidity()) {
                event.preventDefault();
                event.stopPropagation();
            }
            form.classList.add('was-validated');
        });
    });
    
    // DataTable initialization (if DataTables is loaded)
    if (typeof $.fn.DataTable !== 'undefined') {
        $('.data-table').DataTable({
            responsive: true,
            pageLength: 25,
            language: {
                search: "Search:",
                lengthMenu: "Show _MENU_ entries",
                info: "Showing _START_ to _END_ of _TOTAL_ entries",
                paginate: {
                    first: "First",
                    last: "Last",
                    next: "Next",
                    previous: "Previous"
                }
            }
        });
    }
    
    // Confirmation dialogs for delete actions
    const deleteButtons = document.querySelectorAll('[data-action="delete"]');
    deleteButtons.forEach(button => {
        button.addEventListener('click', function(e) {
            e.preventDefault();
            
            const confirmMessage = this.getAttribute('data-confirm') || 'Are you sure you want to delete this item?';
            const targetUrl = this.href || this.getAttribute('data-url');
            
            if (confirm(confirmMessage)) {
                if (this.tagName === 'A') {
                    window.location.href = targetUrl;
                } else if (this.closest('form')) {
                    this.closest('form').submit();
                }
            }
        });
    });
    
    // Auto-submit search forms on change
    const searchForms = document.querySelectorAll('.auto-submit');
    searchForms.forEach(form => {
        const inputs = form.querySelectorAll('input, select');
        inputs.forEach(input => {
            input.addEventListener('change', function() {
                form.submit();
            });
        });
    });
    
    // Numeric input formatting
    const numericInputs = document.querySelectorAll('input[type="number"], .numeric-input');
    numericInputs.forEach(input => {
        input.addEventListener('blur', function() {
            const value = parseFloat(this.value);
            if (!isNaN(value)) {
                if (this.classList.contains('currency')) {
                    this.value = value.toFixed(2);
                } else if (this.classList.contains('percentage')) {
                    this.value = value.toFixed(1);
                }
            }
        });
    });
    
    // Image preview functionality
    const imageInputs = document.querySelectorAll('input[type="file"][accept*="image"]');
    imageInputs.forEach(input => {
        input.addEventListener('change', function() {
            const file = this.files[0];
            const previewContainer = document.querySelector(`#${this.id}-preview`);
            
            if (file && previewContainer) {
                const reader = new FileReader();
                reader.onload = function(e) {
                    previewContainer.innerHTML = `<img src="${e.target.result}" class="img-thumbnail" style="max-width: 200px; max-height: 200px;">`;
                };
                reader.readAsDataURL(file);
            }
        });
    });
    
    // Stock level color coding
    const stockElements = document.querySelectorAll('[data-stock-level]');
    stockElements.forEach(element => {
        const stockLevel = parseInt(element.getAttribute('data-stock-level'));
        const lowStockThreshold = parseInt(element.getAttribute('data-low-stock') || '10');
        
        if (stockLevel === 0) {
            element.classList.add('text-danger', 'fw-bold');
        } else if (stockLevel <= lowStockThreshold) {
            element.classList.add('text-warning', 'fw-bold');
        } else {
            element.classList.add('text-success');
        }
    });
    
    // Quick actions keyboard shortcuts
    document.addEventListener('keydown', function(e) {
        // Ctrl/Cmd + N for new item
        if ((e.ctrlKey || e.metaKey) && e.key === 'n') {
            e.preventDefault();
            const newButton = document.querySelector('[href*="/Create"]');
            if (newButton) {
                newButton.click();
            }
        }
        
        // Ctrl/Cmd + F for search
        if ((e.ctrlKey || e.metaKey) && e.key === 'f') {
            e.preventDefault();
            const searchInput = document.querySelector('input[type="search"], input[name*="search"]');
            if (searchInput) {
                searchInput.focus();
            }
        }
        
        // ESC to close modals
        if (e.key === 'Escape') {
            const openModals = document.querySelectorAll('.modal.show');
            openModals.forEach(modal => {
                const bsModal = bootstrap.Modal.getInstance(modal);
                if (bsModal) {
                    bsModal.hide();
                }
            });
        }
    });
    
    // Real-time search functionality
    const searchInputs = document.querySelectorAll('.real-time-search');
    searchInputs.forEach(input => {
        let searchTimeout;
        
        input.addEventListener('input', function() {
            clearTimeout(searchTimeout);
            const query = this.value;
            const targetTable = document.querySelector(this.getAttribute('data-target'));
            
            searchTimeout = setTimeout(() => {
                filterTable(targetTable, query);
            }, 300);
        });
    });
    
    function filterTable(table, query) {
        if (!table) return;
        
        const rows = table.querySelectorAll('tbody tr');
        const searchTerm = query.toLowerCase();
        
        rows.forEach(row => {
            const text = row.textContent.toLowerCase();
            if (text.includes(searchTerm)) {
                row.style.display = '';
            } else {
                row.style.display = 'none';
            }
        });
    }
    
    // Loading states for buttons
    const loadingButtons = document.querySelectorAll('.btn[data-loading-text]');
    loadingButtons.forEach(button => {
        button.addEventListener('click', function() {
            const originalText = this.innerHTML;
            const loadingText = this.getAttribute('data-loading-text');
            
            this.innerHTML = `<i class="fas fa-spinner fa-spin me-1"></i>${loadingText}`;
            this.disabled = true;
            
            // Re-enable after 3 seconds (adjust as needed)
            setTimeout(() => {
                this.innerHTML = originalText;
                this.disabled = false;
            }, 3000);
        });
    });
    
    // Smooth scrolling for anchor links
    const anchorLinks = document.querySelectorAll('a[href^="#"]');
    anchorLinks.forEach(link => {
        link.addEventListener('click', function(e) {
            e.preventDefault();
            const targetId = this.getAttribute('href').substring(1);
            const targetElement = document.getElementById(targetId);
            
            if (targetElement) {
                targetElement.scrollIntoView({
                    behavior: 'smooth',
                    block: 'start'
                });
            }
        });
    });
    
    // Auto-refresh functionality for dashboard
    if (window.location.pathname.includes('Dashboard')) {
        const refreshInterval = 300000; // 5 minutes
        
        setInterval(() => {
            // Refresh specific elements instead of entire page
            refreshDashboardMetrics();
        }, refreshInterval);
    }
    
    function refreshDashboardMetrics() {
        // This would typically make an AJAX call to get updated metrics
        console.log('Refreshing dashboard metrics...');
        
        // Example: Update timestamp
        const timestampElements = document.querySelectorAll('.last-updated');
        timestampElements.forEach(element => {
            element.textContent = new Date().toLocaleTimeString();
        });
    }
    
    // Print functionality
    const printButtons = document.querySelectorAll('[data-action="print"]');
    printButtons.forEach(button => {
        button.addEventListener('click', function() {
            const printArea = document.querySelector(this.getAttribute('data-target') || '.main-content');
            const originalContent = document.body.innerHTML;
            
            document.body.innerHTML = printArea.innerHTML;
            window.print();
            document.body.innerHTML = originalContent;
            
            // Re-initialize after print
            location.reload();
        });
    });
    
    // Export functionality placeholder
    const exportButtons = document.querySelectorAll('[data-action="export"]');
    exportButtons.forEach(button => {
        button.addEventListener('click', function(e) {
            e.preventDefault();
            const format = this.getAttribute('data-format') || 'csv';
            const endpoint = this.getAttribute('data-endpoint') || '/Export';
            
            // This would trigger an actual export in a real implementation
            console.log(`Exporting data in ${format} format from ${endpoint}`);
            
            // Simulate download
            const link = document.createElement('a');
            link.download = `export_${new Date().toISOString().split('T')[0]}.${format}`;
            link.href = endpoint;
            link.click();
        });
    });
    
    console.log('Admin interface initialized successfully');
});

// Utility functions
function showNotification(message, type = 'info') {
    const alertHtml = `
        <div class="alert alert-${type} alert-dismissible fade show position-fixed" 
             style="top: 20px; right: 20px; z-index: 1050; min-width: 300px;" role="alert">
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        </div>
    `;
    
    document.body.insertAdjacentHTML('beforeend', alertHtml);
    
    // Auto-remove after 5 seconds
    setTimeout(() => {
        const alert = document.querySelector('.alert:last-of-type');
        if (alert) {
            const bsAlert = new bootstrap.Alert(alert);
            bsAlert.close();
        }
    }, 5000);
}

function formatCurrency(amount) {
    return new Intl.NumberFormat('en-PH', {
        style: 'currency',
        currency: 'PHP'
    }).format(amount);
}

function formatNumber(number, decimals = 0) {
    return new Intl.NumberFormat('en-US', {
        minimumFractionDigits: decimals,
        maximumFractionDigits: decimals
    }).format(number);
}

// Global error handler
window.addEventListener('error', function(e) {
    console.error('Application error:', e.error);
    showNotification('An unexpected error occurred. Please refresh the page.', 'danger');
});