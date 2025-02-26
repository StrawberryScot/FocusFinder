using FocusFinderApp.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

string TEST_HOST = "dpg-cuveglij1k6c73ebtoa0-a";
string TEST_PORT = "5432";
string TEST_NAME = "focus_finder_test_db";
string TEST_USER = "focus_finder_test_db_user";
string TEST_PASS = "zuKYttXQJQJrxeO0BcU0jjlADC8PhCD7";

var dbHost = Environment.GetEnvironmentVariable("DATABASE_HOST") ?? TEST_HOST;
var dbPort = Environment.GetEnvironmentVariable("DATABASE_PORT") ?? TEST_PORT;
var dbName = Environment.GetEnvironmentVariable("DATABASE_NAME") ?? TEST_NAME;
var dbUser = Environment.GetEnvironmentVariable("DATABASE_USER") ?? TEST_USER;
var dbPass = Environment.GetEnvironmentVariable("DATABASE_PASS") ?? TEST_PASS;

Console.WriteLine($"Using database: {dbName}");
Console.WriteLine($"On port: {dbPort}");
Console.WriteLine($"As user: {dbUser}");

// 🔹 Build connection string
var connectionString = $"Host={dbHost};Port={dbPort};Username={dbUser};Password={dbPass};Database={dbName}";

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


