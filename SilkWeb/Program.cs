//for configuring services,DI,middlewares

using Microsoft.EntityFrameworkCore;
using Silk.DataAccess.Data;
using Silk.DataAccess.Repository;
using Silk.DataAccess.Repository.IRepository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Add connection between DB and Application via Connection String from appsettings.json by adding DBContext service
builder.Services.AddDbContext<ApplicationDbContext>(options=> 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//Category repository implentation registered here
//--builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>(); // now ICategoryRepository no more required becoz Iunitofwork internally calls it

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

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();
