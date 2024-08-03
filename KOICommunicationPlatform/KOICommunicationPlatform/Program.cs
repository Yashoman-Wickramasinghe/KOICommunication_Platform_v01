using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using KOICommunicationPlatform.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity.UI.Services;
using KOICommunicationPlatform.DataAccess;
using KOICommunicationPlatform.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container. DI Container
builder.Services.AddControllersWithViews();


builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")
    ));

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<ApplicationDbContext>();
//builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddDefaultTokenProviders()
//    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
//builder.Services.AddScoped<IDbInitializer, DbInitializer>();
//builder.Services.AddSingleton<IEmailSender, EmailSender>();


builder.Services.AddRazorPages();
builder.Services.AddScoped<IClientRepository, ClientRepository>();

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
//SeedDatabase();
app.UseAuthentication();;

app.UseAuthorization();
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Admin}/{controller=Home}/{action=Index}/{id?}");

// --Default code : pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
//void SeedDatabase()
//{
//    using (var scope = app.Services.CreateScope())
//    {
//        var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
//        dbInitializer.Initialize();
//    }
//}