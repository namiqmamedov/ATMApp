using ATMApp.Domain.Entities;
using ATMApp.Domain.Entities.Interfaces;
using ATMApp.UI;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ATMApp.App
{
    class Program : IUserLogin
    {
        private List<UserAccount> userAccountList;
        private UserAccount selectedAccount;

        public void InitializeData()
        {
            userAccountList = new List<UserAccount>
            {
                new UserAccount
                {
                    ID = 1,FullName = "Namiq Mamedov",AccountNumber = 2382,CardNumber = 416981237814419,
                    CardPin = 2016,AccountBalance = 294891,IsLocked = false
                },
                new UserAccount
                {
                    ID = 2,FullName = "Ricardo Gonzalez",AccountNumber = 8588,CardNumber = 415872183971232,
                    CardPin = 1997,AccountBalance = 1274398129,IsLocked = false
                },
                new UserAccount
                {
                    ID = 3,FullName = "Marina Soarez",AccountNumber = 7141,CardNumber = 4169318247129321,
                    CardPin = 6465,AccountBalance = 1241294891,IsLocked = false
                }
            };
        }

        public void CheckUserCardNumAndPassword()
        {
            bool isCorrectLogin = false;
            while (isCorrectLogin == false) 
            {
                UserAccount inputAccount = AppScreen.UserLoginForm();
                AppScreen.LoginProgress();
                foreach (UserAccount account in userAccountList)
                {
                    selectedAccount = account;
                    if (inputAccount.CardNumber.Equals(selectedAccount.CardNumber )) 
                    {
                        selectedAccount.TotalLogin++;

                        if(inputAccount.CardPin.Equals(selectedAccount.CardPin ))
                        {
                            selectedAccount = account;
                            if(selectedAccount.IsLocked || selectedAccount.TotalLogin > 3) 
                            {
                                AppScreen.PrintLockScreen();
                            }
                            else
                            {
                                selectedAccount.TotalLogin = 0;
                                isCorrectLogin = true;
                                break;
                            }
                        }
                    }
                }
            }
            
            if(isCorrectLogin == false) 
            {
                Utility.PrintMessage("\n Invalid card number or PIN.", false);
                selectedAccount.IsLocked = selectedAccount.TotalLogin == 3;
                if (selectedAccount.IsLocked)
                {
                    AppScreen.PrintLockScreen();
                }
            }
            Console.Clear();
        }
        public void Welcome()
        {
            Console.WriteLine($"Welcome back, {selectedAccount.FullName}");
        }
    }
}
