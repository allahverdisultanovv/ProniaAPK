using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProniaAPK.DAL;
using ProniaAPK.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDBContext>(opt =>

    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddIdentity<AppUser, IdentityRole>(opt =>
{

    opt.Password.RequireNonAlphanumeric = false;
    opt.Password.RequiredLength = 8;



    opt.User.RequireUniqueEmail = true;


    opt.Lockout.AllowedForNewUsers = true;
    opt.Lockout.MaxFailedAccessAttempts = 2;
    opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);
}).AddEntityFrameworkStores<AppDBContext>().AddDefaultTokenProviders();

var app = builder.Build();


app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();


app.MapControllerRoute(
    "admin",
    "{area:exists}/{controller=home}/{action=index}/{id?}"
    );
app.MapControllerRoute(
    "default",
    "{controller=home}/{action=index}/{id?}"
    );
app.Run();



