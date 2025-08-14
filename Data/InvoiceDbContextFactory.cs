using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace InvoiceApp.EFCore.Data
{
   
    // Design-time factory for creating the DbContext (used by EF migrations).
    // Ensures the same database file name across the entire app.
   
    public class InvoiceDbContextFactory : IDesignTimeDbContextFactory<InvoiceDbContext>
    {
        public InvoiceDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<InvoiceDbContext>();
            optionsBuilder.UseSqlite("Data Source=invoices.db");
            return new InvoiceDbContext(optionsBuilder.Options);
        }
    }
}
