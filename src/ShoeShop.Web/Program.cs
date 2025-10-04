using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShoeShop.Repository.Data;
using ShoeShop.Services;
using ShoeShop.Services.Mapping;
using ShoeShop.Web.Data;
using ShoeShop.Web.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add Entity Framework (using In-Memory database for testing)
builder.Services.AddDbContext<ShoeShopDbContext>(options =>
    options.UseInMemoryDatabase("ShoeShopDB"));

// Add Identity services
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("IdentityDB"));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configure authentication cookie
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(24);
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

// Add Service Layer services
builder.Services.AddServices();

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Initialize database with seed data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ShoeShopDbContext>();
    context.Database.EnsureCreated();
    
    // Initialize Identity database and seed roles
    var identityContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    
    identityContext.Database.EnsureCreated();
    
    // Seed roles
    if (!roleManager.Roles.Any())
    {
        var roles = new[] { "Administrator", "Manager", "Employee" };
        foreach (var role in roles)
        {
            roleManager.CreateAsync(new IdentityRole(role)).Wait();
        }
    }
    
    // Seed default admin user
    if (!userManager.Users.Any())
    {
        var adminUser = new ApplicationUser
        {
            UserName = "admin",
            Email = "admin@shoeshop.com",
            FirstName = "System",
            LastName = "Administrator",
            EmailConfirmed = true
        };
        
        var result = userManager.CreateAsync(adminUser, "Admin@123").Result;
        if (result.Succeeded)
        {
            userManager.AddToRoleAsync(adminUser, "Administrator").Wait();
        }
        
        // Add a manager user for testing
        var managerUser = new ApplicationUser
        {
            UserName = "manager",
            Email = "manager@shoeshop.com",
            FirstName = "Store",
            LastName = "Manager",
            EmailConfirmed = true
        };
        
        result = userManager.CreateAsync(managerUser, "Manager@123").Result;
        if (result.Succeeded)
        {
            userManager.AddToRoleAsync(managerUser, "Manager").Wait();
        }
        
        // Add an employee user for testing
        var employeeUser = new ApplicationUser
        {
            UserName = "employee",
            Email = "employee@shoeshop.com",
            FirstName = "Store",
            LastName = "Employee",
            EmailConfirmed = true
        };
        
        result = userManager.CreateAsync(employeeUser, "Employee@123").Result;
        if (result.Succeeded)
        {
            userManager.AddToRoleAsync(employeeUser, "Employee").Wait();
        }
    }
}

app.Run();
