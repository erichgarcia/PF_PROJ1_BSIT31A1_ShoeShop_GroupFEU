using Microsoft.EntityFrameworkCore;
using ShoeShop.Repository.Data;
using ShoeShop.Repository.Models;

var optionsBuilder = new DbContextOptionsBuilder<ShoeShopDbContext>();
optionsBuilder.UseSqlite("Data Source=shoeshop.db");

using var context = new ShoeShopDbContext(optionsBuilder.Options);

Console.WriteLine("ShoeShop Repository Console Test Application");
Console.WriteLine("===========================================");

// Ensure database is created
context.Database.EnsureCreated();

Console.WriteLine("Database created successfully!");
Console.WriteLine($"Total Shoes: {context.Shoes.Count()}");
Console.WriteLine($"Total Color Variations: {context.ShoeColorVariations.Count()}");
Console.WriteLine($"Total Suppliers: {context.Suppliers.Count()}");
