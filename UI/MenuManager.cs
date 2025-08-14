using InvoiceApp.EFCore.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace InvoiceApp.EFCore.UI
{
  
    // Shows the main menu loop. Passes actions to InvoiceUI.
    
    public class MenuManager
    {
        private readonly InvoiceService _service;

        public MenuManager(InvoiceService service) => _service = service;

        public async Task RunAsync()
        {
            bool exit = false;

            while (!exit)
            {
                Console.Clear();
                ShowMenu();
                Console.Write("Choose an option: ");
                string input = Console.ReadLine() ?? "";

                if (!new[] { "0", "1", "2", "3", "4", "5", "6" }.Contains(input))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid input. Please enter a number from 0 to 6.");
                    Console.ResetColor();
                    Console.WriteLine("Press Enter to try again...");
                    Console.ReadLine();
                    continue;
                }

                Console.Clear();
                switch (input)
                {
                    case "1":
                        await InvoiceUI.AddInvoiceAsync(_service);
                        break;
                    case "2":
                        await InvoiceUI.ShowAllInvoicesAsync(_service);
                        break;
                    case "3":
                        await InvoiceUI.SearchInvoicesAsync(_service);
                        break;
                    case "4":
                        await InvoiceUI.DeleteInvoiceAsync(_service);
                        break;
                    case "5":
                        await InvoiceUI.ShowStatsAsync(_service);
                        break;
                    case "6":
                        await InvoiceUI.SortInvoicesAsync(_service);
                        break;
                    case "0":
                        exit = true;
                        break;
                }
            }
        }

        private void ShowMenu()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=== INVOICE MANAGER ===");
            Console.ResetColor();
            Console.WriteLine("1. Add invoice");
            Console.WriteLine("2. Show all invoices");
            Console.WriteLine("3. Search invoice");
            Console.WriteLine("4. Delete invoice");
            Console.WriteLine("5. Show statistics");
            Console.WriteLine("6. Sort invoices");
            Console.WriteLine("0. Exit");
            Console.WriteLine();
        }
    }
}
