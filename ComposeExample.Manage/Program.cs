using ComposeExample.Extensions;
using ComposeExample.MoviesClient;
using Serilog;
using System.ComponentModel;

namespace ComposeExample.Manage
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Lets add serilog
            builder.Host.UseSerilog((context, services, configuration) => configuration.ReadFrom.Configuration(context.Configuration).Enrich.WithProperty("Application", "ComposeExample.Manage"));

            builder.Services.AddSingleton<IConfigurationRoot>(builder.Configuration);

            // Add services to the container.
            builder.Services.AddControllersWithViews()
                .AddJsonOptions(o => o.AddDateOnlySupport());

            TypeDescriptor.AddAttributes(typeof(DateOnly), new TypeConverterAttribute(typeof(DateOnlyTypeConverter)));

            builder.Services.AddMoviesApiClient(builder.Configuration.GetSection("KnownUrls")["Api"]);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}