using ATMApp.Domain.Entities;
using ATMApp.UI;
using System;
using System.Collections.Generic;

namespace ATMApp.App
{
    class Program
    {
        private List<UserAccount> userAccountList;
        private UserAccount selectedAccount;

        public void InitializeData()
        {
            userAccountList = new List<UserAccount>();
        }

        static void Main(string[] args)
        {
           AppScreen.Welcome();
           long cardNumber  = Validator.Convert<long>("your card number");
           Console.WriteLine($"Your card number is {cardNumber}");

           Utility.PressEnterToContinue();
        }
    }
}
