using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using TimeOffManager.Data;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();

       

        // Configure Entity Framework Core with PostgreSQL
        builder.Services.AddDbContext<URDbContext>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("URDbContext"));
        });

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


        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Login}/{action=Index}/{id?}");

        app.Run();
    }
}
