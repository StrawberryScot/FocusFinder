using FocusFinderApp.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Default to using the development database
var defaultDbName = "focus_finder";

// 🔹 If the user passes a DATABASE_NAME, override it
var dbName = Environment.GetEnvironmentVariable("DATABASE_NAME") ?? defaultDbName;
Console.WriteLine($"Using database: {dbName}");

// 🔹 Build connection string
var connectionString = $"Host=localhost;Username=postgres;Password=1234;Database={dbName}";

// 🔹 Register DbContext with DI
builder.Services.AddDbContext<FocusFinderDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


