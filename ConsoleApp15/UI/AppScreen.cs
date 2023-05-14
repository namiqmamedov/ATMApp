using ATMApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

        internal static UserAccount UserLoginForm()
        {
            UserAccount tempUserAccount = new UserAccount();

            tempUserAccount.CardNumber = Validator.Convert<long>("your card number.");
            tempUserAccount.CardPin = Convert.ToInt32(Utility.GetSecretInput("Enter your card PIN"));
            return tempUserAccount;
        }


        internal static void LoginProgress() 
        {
            Console.WriteLine("Checking card number and PIN...");
            Utility.PrintDotAnimation();    
        }

        internal static void PrintLockScreen()
        {
            Console.Clear();
            Utility.PrintMessage("Your account is locked. Please go to the nearest branch to unlock your account.",true);
            Utility.PressEnterToContinue();
            Environment.Exit(1);
        }
    }
    
}
