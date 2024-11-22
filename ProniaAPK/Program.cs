using Microsoft.EntityFrameworkCore;
using ProniaAPK.DAL;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDBContext>(opt =>

    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default"))

    );

var app = builder.Build();
app.UseStaticFiles();

app.MapControllerRoute(
    "default",
    "{controller=home}/{action=index}/{id?}"
    );
app.Run();



