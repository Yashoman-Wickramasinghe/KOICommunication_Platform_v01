using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using KOICommunicationPlatform.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity.UI.Services;
using KOICommunicationPlatform.DataAccess;
using KOICommunicationPlatform.Models;
using KOICommunicationPlatform.DataAccess.Repository.IRepository;
using KOICommunicationPlatform.DataAccess.Repository;
using KOICommunicationPlatform.DataAccess.DbInitializer;
using KOICommunicationPlatform.Utilities.EmailSender;
using KOICommunicationPlatform.Utilities.Helper;
using OfficeOpenXml;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container. DI Container
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Register Identity with custom user type
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IDbInitializer, DbInitializer>();
builder.Services.AddScoped<IEmailDispatcher, EmailDispatcher>();
builder.Services.AddScoped<IRegistrationEmailSender, RegistrationEmailSender>();

// Configure authentication and authorization
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login"; // Path to the login page
    options.AccessDeniedPath = "/Identity/Account/AccessDenied"; // Path for access denied page
});

builder.Services.AddRazorPages();
builder.Services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();

// Set the EPPlus License Context
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Ensure Authentication and Authorization middleware are used
app.UseAuthentication();
app.UseAuthorization();

// Seed the database
SeedDatabase();

app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Admin}/{controller=Home}/{action=Index}/{id?}");

// Default route pattern without the area part
// app.MapControllerRoute(
//     name: "default",
//     pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

void SeedDatabase()
{
    using (var scope = app.Services.CreateScope())
    {
        var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
        dbInitializer.Initialize();
    }
}
