using System;
using System.Collections.Generic;
using System.Linq;

namespace InvoiceApp.EFCore.Models
{
   
    // Represents an invoice header with customer data and invoice items.
    
    public class Invoice
    {
        public int Id { get; set; }

        //  Full display name, e.g. "John Smith". 
        public string CustomerName { get; set; } = string.Empty;

        //  Optional customer email (may be empty). 
        public string CustomerEmail { get; set; } = string.Empty;

        //  Optional short description / notes. 
        public string Description { get; set; } = string.Empty;

        public DateTime InvoiceDate { get; set; }

        //  Collection of invoice line items. 
        public List<InvoiceItem> Items { get; set; } = new();

        //  Calculated total including tax (sum of items). 
        public decimal TotalAmount => Items.Sum(i => i.Total);
    }
}
