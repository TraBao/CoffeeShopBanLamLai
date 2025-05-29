using coffeeShop.Models.Interfaces;
using CoffeeShop.Data;
using CoffeeShop.Models.interfaces;
using CoffeeShop.Models.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Dependency Injection
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IShoppingCartRepository>(sp =>
    ShoppingCartRepository.GetCart(sp));

// DbContext
builder.Services.AddDbContext<CoffeeshopDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CoffeeShopDbContextConnection")));

// Session & HTTP Context
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
