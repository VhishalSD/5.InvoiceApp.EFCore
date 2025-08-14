#nullable enable
using InvoiceApp.EFCore.Data;
using InvoiceApp.EFCore.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvoiceApp.EFCore.Services
{
 
    // Service layer that encapsulates all database operations for invoices.
    // Keeps UI free from EF Core details.
  
    public class InvoiceService
    {
        private readonly InvoiceDbContext _db;

        public InvoiceService(InvoiceDbContext db) => _db = db;

        // Adds a new invoice (with items) and saves changes.
        public async Task AddInvoiceAsync(Invoice invoice)
        {
            _db.Invoices.Add(invoice);
            await _db.SaveChangesAsync();
        }

        // Gets all invoices including their items.
        public async Task<List<Invoice>> GetAllInvoicesAsync()
        {
            return await _db.Invoices
                .Include(i => i.Items)
                .AsNoTracking()
                .ToListAsync();
        }

        // Search by customer name or description (case-insensitive)
        public async Task<List<Invoice>> SearchInvoicesAsync(string term)
        {
            term = term?.ToLower() ?? string.Empty;

            return await _db.Invoices
                .Include(i => i.Items)
                .Where(i =>
                    i.CustomerName.ToLower().Contains(term) ||
                    i.Description.ToLower().Contains(term))
                .AsNoTracking()
                .ToListAsync();
        }

        // Finds a single invoice by ID (with items).
        public async Task<Invoice?> GetInvoiceByIdAsync(int id)
        {
            return await _db.Invoices
                .Include(i => i.Items)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        // Deletes an invoice if it exists.
        public async Task DeleteInvoiceAsync(int id)
        {
            var invoice = await _db.Invoices.FindAsync(id);
            if (invoice != null)
            {
                _db.Invoices.Remove(invoice);
                await _db.SaveChangesAsync();
            }
        }

        // Returns invoice count.
        public Task<int> GetInvoiceCountAsync()
            => _db.Invoices.CountAsync();

        // Sum of all invoice totals (computed in memory after projection).
        public async Task<decimal> GetTotalAmountAsync()
        {
            // Project items and aggregate in DB as much as possible.
            var totals = await _db.Invoices
                .Select(i => new
                {
                    Total = i.Items.Sum(ii => ii.Quantity * ii.UnitPrice * (1 + ii.TaxRate))
                })
                .ToListAsync();

            return totals.Sum(t => t.Total);
        }

        // Average invoice total amount.
        public async Task<decimal> GetAverageAmountAsync()
        {
            int count = await GetInvoiceCountAsync();
            if (count == 0) return 0m;
            decimal total = await GetTotalAmountAsync();
            return total / count;
        }

       
        // Returns invoices sorted by option (1=Name, 2=Date, 3=Amount).
        // Sorting is performed in the database where possible.
        
        public async Task<List<Invoice>> GetSortedInvoicesAsync(string? option)
        {
            IQueryable<Invoice> query = _db.Invoices.Include(i => i.Items);

            switch (option)
            {
                case "1": // Customer Name
                    query = query.OrderBy(i => i.CustomerName);
                    break;
                case "2": // Invoice Date
                    query = query.OrderBy(i => i.InvoiceDate);
                    break;
                case "3": // Amount
                    // Order by computed amount in memory after projection.
                    // EF can't always translate the computed property directly.
                    var list = await query.AsNoTracking().ToListAsync();
                    return list.OrderBy(i => i.TotalAmount).ToList();
                default:
                    break;
            }

            return await query.AsNoTracking().ToListAsync();
        }
    }
}
