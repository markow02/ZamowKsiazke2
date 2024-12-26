using ZamowKsiazke.Models;
using Microsoft.EntityFrameworkCore;
using ZamowKsiazke.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using ZamowKsiazke.Services;
using ZamowKsiazke.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Configure services
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Register the DbContext with the dependency injection container
builder.Services.AddDbContext<ZamowKsiazkeContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ZamowKsiazkeContext")));


builder.Services.AddDefaultIdentity<DefaultUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ZamowKsiazkeContext>();


//builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<Cart>(sp => Cart.GetCart(sp));
builder.Services.AddScoped<IOrderService, OrderService>(); // Register IOrderService
builder.Services.AddScoped<IUserActivityService, UserActivityService>(); // Register IUserActivityService
builder.Services.AddScoped<IBookBorrowingService, BookBorrowingService>(); // Register IBookBorrowingService

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    //options.IdleTimeout = TimeSpan.FromSeconds(10);
});

var app = builder.Build();

// Seedowanie danych
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<DefaultUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    try
    {
        // Seed default data
        SeedData.Initialize(services);

        // Create Admin Role if it doesn't exist
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        // Create Admin User if it doesn't exist
        var adminEmail = "admin@example.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            var admin = new DefaultUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "Admin",
                LastName = "User",
                Address = "Admin Address",
                City = "Admin City",
                ZipCode = "00-000"
            };

            var result = await userManager.CreateAsync(admin, "Admin123!");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Store}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
