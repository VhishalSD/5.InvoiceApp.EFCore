using InvoiceApp.EFCore.Data;
using InvoiceApp.EFCore.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace InvoiceApp.EFCore
{

    // Entry point. Creates DbContext, applies migrations, and runs the menu.

    class Program
    {
        static async Task Main()
        {
            Console.Title = "InvoiceApp.EFCore";

            var optionsBuilder = new DbContextOptionsBuilder<InvoiceDbContext>();
            optionsBuilder.UseSqlite("Data Source=invoices.db");

            using var db = new InvoiceDbContext(optionsBuilder.Options);

            // Apply pending migrations automatically on startup.
            await db.Database.MigrateAsync();

            var service = new InvoiceService(db);
            var menu = new UI.MenuManager(service);

            await menu.RunAsync();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Goodbye!");
            Console.ResetColor();
        }
    }
}