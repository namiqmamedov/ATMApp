using ATMApp.Domain;
using ATMApp.Domain.Entities;
using ATMApp.Domain.Entities.Interfaces;
using ATMApp.Domain.Enums;
using ATMApp.UI;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ATMApp.App
{
    class Program : IUserLogin, IUserAccountActions,ITransaction
    {
        private List<UserAccount> userAccountList;
        private UserAccount selectedAccount;
        private List<Transaction> _listOfTransactions;

        public void Run()
        {
            AppScreen.Welcome();
            CheckUserCardNumAndPassword();
            AppScreen.WelcomeCustomer(selectedAccount.FullName);
            AppScreen.DisplayAppMenu();
            ProcessMenuOption();

        }

        public void InitializeData()
        {
            userAccountList = new List<UserAccount>
            {
                new UserAccount
                {
                    ID = 1,FullName = "Namiq Mamedov",AccountNumber = 2382,CardNumber = 4169,
                    CardPin = 201616,AccountBalance = 294891,IsLocked = false
                },
                new UserAccount
                {
                    ID = 2,FullName = "Ricardo Gonzalez",AccountNumber = 8588,CardNumber = 415872183971232,
                    CardPin = 199797,AccountBalance = 1274398129,IsLocked = false
                },
                new UserAccount
                {
                    ID = 3,FullName = "Marina Soarez",AccountNumber = 7141,CardNumber = 4169318247129321,
                    CardPin = 646564,AccountBalance = 1241294891,IsLocked = false
                }
            };
             
            _listOfTransactions = new List<Transaction>();
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


                    if (isCorrectLogin == false)
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
            }
        }

        private void ProcessMenuOption()
        {
            switch(Validator.Convert<int>("an option:"))
            {
                case (int)AppMenu.CheckBalance:
                    CheckBalance();
                    break;
                case (int)AppMenu.PlaceDeposit:
                    PlaceDeposit();
                    break;
                case (int)AppMenu.MakeWithdrawal:
                    Console.WriteLine("Making withdrawal...");
                    break;
                case (int)AppMenu.InternalTransfer:
                    Console.WriteLine("Making internal transfer...");
                    break;
                case (int)AppMenu.ViewTransaction:
                    Console.WriteLine("Viewing transactions...");
                    break;
                case (int)AppMenu.Logout:
                    AppScreen.LogOutProgress();
                    Utility.PrintMessage("You have successfully logout.Please collect your ATM card.");
                    Run();
                    break;
                default:
                    Utility.PrintMessage("Invalid Option.",false);
                    break;
            }
        }

        public void CheckBalance()
        {
            Utility.PrintMessage($"Your account balance is: {Utility.FormatAmount(selectedAccount.AccountBalance)}");
        }

        public void PlaceDeposit()
        {
            Console.WriteLine("Only multiples of 500 and 1000 USD allowed");
            var transaction_amt = Validator.Convert<int>($"amount {AppScreen.cur}");

            Console.WriteLine("\nChecking and Counting bank notes.");
            Utility.PrintDotAnimation();
            Console.WriteLine("");

            if(transaction_amt <= 0)
            {
                Utility.PrintMessage("Amount needs to be greater than zero",false);
                return;
            }
            if(transaction_amt % 500 != 0)
            {
                Utility.PrintMessage($"Enter deposit amount in multiples of 500 or 1000. Try again.",false);
                return; 
            }
            if (PreviewBankNotesCount(transaction_amt) == false)
            {
                Utility.PrintMessage($"You have cancelled your action.", false);
                return;
            }

            // bind transaction details to transaction object
            InsertTransaction(selectedAccount.ID, TransactionType.Deposit, transaction_amt, "" /*< description */);

            // update acc balance

            selectedAccount.AccountBalance += transaction_amt;

            Utility.PrintMessage($"Your deposit of {Utility.FormatAmount(transaction_amt)} was successfull", true);


        }

        public void MakeWithDraw()
        {
            throw new NotImplementedException();
        }

        private bool PreviewBankNotesCount(int amount)
        {
            int thousandNotesCount = amount / 1000;
            int fiveHundredNotesCount = (amount % 1000) / 500;

            Console.WriteLine("\nSummary");
            Console.WriteLine("---------");
            Console.WriteLine($"{AppScreen.cur}1000 X {thousandNotesCount} = {1000 * thousandNotesCount}");
            Console.WriteLine($"{AppScreen.cur}500 X {fiveHundredNotesCount} = {500 * fiveHundredNotesCount } ");
            Console.WriteLine($"Total amount: {Utility.FormatAmount(amount)}\n\n");

            int opt = Validator.Convert<int>("1 to confirm");
            return opt.Equals(1);
        }

        public void InsertTransaction(long _UserBankAccountID, TransactionType _tranType, decimal _tranAmount, string _desc)
        {
            var transaction = new Transaction()
            {
                TransactionID = Utility.GetTransactionID(),
                UserBankAccountID = _UserBankAccountID,
                TransactionDate = DateTime.Now,
                TransactionType = _tranType,
                TransactionAmount = _tranAmount,
                Description = _desc
            };

            _listOfTransactions.Add(transaction);
        }

        public void ViewTransaction()
        {
            throw new NotImplementedException();
        }
    }
}
