using Microsoft.EntityFrameworkCore;
using WebApplication1.Models.Cinema;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<CinemaDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("CinemaCS")));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Producers}/{action=Index}/{id?}");

app.Run();