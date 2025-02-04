//for configuring services,DI,middlewares

using Microsoft.EntityFrameworkCore;
using Silk.DataAccess.Data;
using Silk.DataAccess.Repository;
using Silk.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Silk.Utility;
using Microsoft.AspNetCore.Identity.UI.Services;
using Stripe;
using Silk.DataAccess.DbInitializer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Add connection between DB and Application via Connection String from appsettings.json by adding DBContext service
builder.Services.AddDbContext<ApplicationDbContext>(options=> 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));   // to bind secret configs from appsettings.json via static c file i.e. StripeSettings.cs get/set properties

builder.Services.AddRazorPages();    // Identity Scaffolding added therfore routing required for razor pages

builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();


builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});

//FB login integration - facebook.developers
builder.Services.AddAuthentication().AddFacebook(option =>
{
    option.AppId = "904692448200469";
    option.AppSecret = "7ce6e1eb5d5e860701dea6d6a101ad61";
});

//configuring session in .net core
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(100);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddScoped<IDbInitializer, IDbInitializer>();
//Category repository implentation registered here
//--builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>(); // now ICategoryRepository no more required becoz Iunitofwork internally calls it
builder.Services.AddScoped<IEmailSender, EmailSender>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); //wwwroot folder static files
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();                   // session usage in request pipeine
SeedDatabaseInitializer();          // will seed roles,admin user,pending migration for the first time when Prod app started.
app.MapRazorPages();                // Identity Scaffolding added therfore routing required for razor pages
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();

void SeedDatabaseInitializer()
{
    using (var scope = app.Services.CreateScope())
    {
        var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
        dbInitializer.Initialize();
    }
}
