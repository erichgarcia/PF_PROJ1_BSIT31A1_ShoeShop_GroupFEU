using Microsoft.EntityFrameworkCore;
using ShoeShop.Repository.Data;
using ShoeShop.Service.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add Entity Framework (using In-Memory database for testing)
builder.Services.AddDbContext<ShoeShopDbContext>(options =>
    options.UseInMemoryDatabase("ShoeShopDB"));

// Add Service Layer services (from ServiceConfiguration)
builder.Services.AddServiceLayer();

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

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
}

app.Run();
