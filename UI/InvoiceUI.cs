using InvoiceApp.EFCore.Models;
using InvoiceApp.EFCore.Services;
using InvoiceApp.EFCore.Validators;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace InvoiceApp.EFCore.UI
{
    public static class InvoiceUI
    {
        // Add a new invoice with multiple items
        public static async Task AddInvoiceAsync(InvoiceService service)
        {
            // Input first name
            string firstName;
            while (true)
            {
                Console.Write("First name (or type 'cancel' to return): ");
                firstName = Console.ReadLine()?.Trim() ?? "";

                if (firstName.ToLower() == "cancel") return;

                if (!Validator.IsValidName(firstName))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid first name. Name must be at least 3 letters and contain only letters, spaces or hyphens.");
                    Console.ResetColor();
                    continue;
                }

                firstName = FormatName(firstName);
                break;
            }

            // Input last name
            string lastName;
            while (true)
            {
                Console.Write("Last name (or type 'cancel' to return): ");
                lastName = Console.ReadLine()?.Trim() ?? "";

                if (lastName.ToLower() == "cancel") return;

                if (!Validator.IsValidName(lastName))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid last name. Name must be at least 3 letters and contain only letters, spaces or hyphens.");
                    Console.ResetColor();
                    continue;
                }

                lastName = FormatName(lastName);
                break;
            }

            // Input email
            string email;
            while (true)
            {
                Console.Write("Email (or type 'skip' to leave empty): ");
                email = Console.ReadLine()?.Trim() ?? "";

                if (email.ToLower() == "skip") break;

                if (!Validator.IsValidEmail(email))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid email address.");
                    Console.ResetColor();
                    continue;
                }

                break;
            }

            // Input invoice description
            Console.Write("Description: ");
            string description = Console.ReadLine()?.Trim() ?? "";

            // Input invoice date
            DateTime invoiceDate;
            while (true)
            {
                Console.Write("Invoice date (dd-MM-yyyy): ");
                string dateInput = Console.ReadLine()?.Trim() ?? "";

                if (Validator.TryParseInvoiceDate(dateInput, out invoiceDate)) break;

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid date format or unrealistic date.");
                Console.ResetColor();
            }

            // Input invoice items
            var items = new List<InvoiceItem>();
            while (true)
            {
                Console.Write("Item description: ");
                string itemDesc = Console.ReadLine()?.Trim() ?? "";
                if (string.IsNullOrWhiteSpace(itemDesc))
                {
                    Console.WriteLine("Description cannot be empty.");
                    continue;
                }

                Console.Write("Quantity: ");
                string qtyInput = Console.ReadLine()?.Trim() ?? "";
                if (!Validator.IsValidDecimal(qtyInput, out decimal qty))
                {
                    Console.WriteLine("Invalid quantity.");
                    continue;
                }

                Console.Write("Unit price: ");
                string priceInput = Console.ReadLine()?.Trim() ?? "";
                if (!Validator.IsValidDecimal(priceInput, out decimal unitPrice))
                {
                    Console.WriteLine("Invalid unit price.");
                    continue;
                }

                Console.Write("Tax rate (e.g. 0.21 for 21%): ");
                string taxInput = Console.ReadLine() ?? "";
                if (!Validator.IsValidTaxRate(taxInput, out decimal taxRate))
                {
                    Console.WriteLine("Invalid tax rate.");
                    continue;
                }

                items.Add(new InvoiceItem
                {
                    Description = itemDesc,
                    Quantity = qty,
                    UnitPrice = unitPrice,
                    TaxRate = taxRate
                });

                Console.Write("Add another item? (y/n): ");
                string another = Console.ReadLine()?.Trim().ToLower() ?? "n";
                if (another != "y") break;
            }

            // Create invoice object
            string customerName = $"{firstName} {lastName}";
            var invoice = new Invoice
            {
                CustomerName = customerName,
                Description = description,
                InvoiceDate = invoiceDate,
                Items = items
            };

            await service.AddInvoiceAsync(invoice);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Invoice with items added successfully.");
            Console.ResetColor();
        }

        // Show all invoices
        public static async Task ShowAllInvoicesAsync(InvoiceService service)
        {
            var invoices = await service.GetAllInvoicesAsync();

            Console.WriteLine("ID   Name                 Description     Amount        Date");
            Console.WriteLine("--------------------------------------------------------------");

            if (invoices.Count == 0)
            {
                Console.WriteLine("No invoices found.");
                return;
            }

            int counter = 1;
            foreach (var invoice in invoices)
            {
                Console.WriteLine($"{counter.ToString().PadRight(4)} {invoice.CustomerName.PadRight(20)} {invoice.Description.PadRight(15)} {invoice.TotalAmount.ToString("C", new CultureInfo("nl-NL")).PadRight(13)} {invoice.InvoiceDate:dd-MM-yyyy}");
                counter++;
            }
        }

        // Search invoices
        public static async Task SearchInvoicesAsync(InvoiceService service)
        {
            Console.Write("Enter search term: ");
            string search = Console.ReadLine()?.ToLower() ?? "";

            var results = await service.SearchInvoicesAsync(search);

            if (results.Count == 0)
            {
                Console.WriteLine("No matching invoice found.");
                return;
            }

            foreach (var invoice in results)
            {
                Console.WriteLine($"{invoice.Id}. {invoice.CustomerName} - {invoice.Description} - {invoice.TotalAmount.ToString("C", new CultureInfo("nl-NL"))} - {invoice.InvoiceDate:dd-MM-yyyy}");
            }
        }

        // Delete invoice
        public static async Task DeleteInvoiceAsync(InvoiceService service)
        {
            Console.Write("Enter ID to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID.");
                return;
            }

            var invoice = await service.GetInvoiceByIdAsync(id);
            if (invoice == null)
            {
                Console.WriteLine("Invoice not found.");
                return;
            }

            Console.Write($"Are you sure you want to delete invoice {invoice.Id} (Customer: {invoice.CustomerName})? (y/n): ");
            if ((Console.ReadLine()?.Trim().ToLower() ?? "") == "y")
            {
                await service.DeleteInvoiceAsync(id);
                Console.WriteLine("Invoice deleted.");
            }
        }

        // Show invoice statistics
        public static async Task ShowStatsAsync(InvoiceService service)
        {
            int total = await service.GetInvoiceCountAsync();
            decimal totalAmount = await service.GetTotalAmountAsync();
            decimal average = await service.GetAverageAmountAsync();

            Console.WriteLine($"Total invoices: {total}");
            Console.WriteLine($"Total amount: €{totalAmount:F2}");
            Console.WriteLine($"Average amount: €{average:F2}");
        }

        // Sort invoices
        public static async Task SortInvoicesAsync(InvoiceService service)
        {
            Console.WriteLine("Sort by: 1. Customer Name  2. Invoice Date  3. Amount");
            var option = Console.ReadLine();

            var sorted = await service.GetSortedInvoicesAsync(option);

            Console.WriteLine("ID   Name                 Description     Amount        Date");
            Console.WriteLine("--------------------------------------------------------------");

            int counter = 1;
            foreach (var invoice in sorted)
            {
                Console.WriteLine($"{counter.ToString().PadRight(4)} {invoice.CustomerName.PadRight(20)} {invoice.Description.PadRight(15)} {invoice.TotalAmount.ToString("C", new CultureInfo("nl-NL")).PadRight(13)} {invoice.InvoiceDate:dd-MM-yyyy}");
                counter++;
            }
        }

        // Format names with capitalization
        private static string FormatName(string name)
        {
            var words = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < words.Length; i++)
            {
                var word = words[i];
                if (word.Length == 0) continue;

                words[i] = char.ToUpper(word[0]) + word.Substring(1).ToLower();
            }
            return string.Join(' ', words);
        }
    }
}
