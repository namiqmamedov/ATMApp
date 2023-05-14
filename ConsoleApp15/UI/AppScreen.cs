using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATMApp.UI
{
    public static class AppScreen
    {
        internal static void Welcome()
        {
            Console.Clear();
            Console.Title = "My ATM App";
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine("\n\n--------------Welcome to My ATM App--------------\n\n");

            Console.WriteLine("Please insert your ATM card");
            Console.WriteLine("Note: Actual ATM machine will accept and validate a physical ATM card," +
                "read the card number and validate it.");

            Utility.PressEnterToContinue();
        }
    }
    
}
