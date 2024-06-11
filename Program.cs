using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Book_List.Models;
using Book_List.Utilities;

namespace Book_List
{
    public static class Program
    {
        public static void Main()
        {
            // Build
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Nastavení závislostí
            var serviceProvider = new ServiceCollection()
                .AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")))
                .AddTransient<Library>()
                .AddTransient<Controls>()
                .BuildServiceProvider();

            // Start appky
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<AppDbContext>();

            // Ensure Db vytvořena
            context.Database.EnsureCreated();

            var library = services.GetRequiredService<Library>();
            var controls = services.GetRequiredService<Controls>();

            // Context instance pro Library a Controls
            library.SetContext(context);
            controls.SetLibrary(library);

            // Start appky vyvoláním MainMenu z Controls
            controls.MainMenu();
        }
    }
}