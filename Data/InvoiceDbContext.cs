using InvoiceApp.EFCore.Models;
using Microsoft.EntityFrameworkCore;

namespace InvoiceApp.EFCore.Data
{
    // EF Core DbContext for invoices and items.
    
    public class InvoiceDbContext : DbContext
    {
        public DbSet<Invoice> Invoices { get; set; } = null!;
        public DbSet<InvoiceItem> InvoiceItems { get; set; } = null!;

        public InvoiceDbContext(DbContextOptions<InvoiceDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // One-to-many: Invoice -> Items, cascade delete.
            modelBuilder.Entity<Invoice>()
                .HasMany(i => i.Items)
                .WithOne(ii => ii.Invoice)
                .HasForeignKey(ii => ii.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
