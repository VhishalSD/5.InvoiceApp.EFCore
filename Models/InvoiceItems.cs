namespace InvoiceApp.EFCore.Models
{

    // Represents a single invoice line (product/service).

    public class InvoiceItem
    {
        public int Id { get; set; }

        public string Description { get; set; } = string.Empty;

        // Quantity (units/hours). Must be positive. 
        public decimal Quantity { get; set; }

        // Unit price (net). Must be positive. 
        public decimal UnitPrice { get; set; }

        //  Tax rate as decimal (e.g. 0.21 for 21%). 
        public decimal TaxRate { get; set; }

        //  Total for this line including tax.
        public decimal Total => Quantity * UnitPrice * (1 + TaxRate);

        //  Foreign key to parent invoice. 
        public int InvoiceId { get; set; }

        //  Navigation to parent invoice. 
        public Invoice Invoice { get; set; } = null!;
    }
}
