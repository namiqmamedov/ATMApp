using ATMApp.Domain;
using ATMApp.Domain.Entities;
using ATMApp.Domain.Entities.Interfaces;
using ATMApp.Domain.Enums;
using ATMApp.UI;
using ConsoleTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ATMApp.App
{
    class Program : IUserLogin, IUserAccountActions,ITransaction
    {
        private List<UserAccount> userAccountList;
        private UserAccount selectedAccount;
        private List<Transaction> _listOfTransactions;
        private const decimal minimumKeptAmount = 500;


        public void Run()
        {
            AppScreen.Welcome();
            CheckUserCardNumAndPassword();
            AppScreen.WelcomeCustomer(selectedAccount.FullName);
            while (true)
            {
                AppScreen.DisplayAppMenu();
                ProcessMenuOption();
            }
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
                    MakeWithDraw();
                    break;
                case (int)AppMenu.InternalTransfer:
                    var internalTransfer = AppScreen.InternalTransferForm();
                    ProcessInternalTransfer(internalTransfer); 
                    break;
                case (int)AppMenu.ViewTransaction:
                    ViewTransaction();
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
            InsertTransaction(selectedAccount.ID, TransactionType.Deposit, transaction_amt, "" /* "" is description */);

            // update acc balance

            selectedAccount.AccountBalance += transaction_amt;

            Utility.PrintMessage($"Your deposit of {Utility.FormatAmount(transaction_amt)} was successfull", true);


        }

        public void MakeWithDraw()
        {
            var transaction_amt = 0;
            int selectedAmount = AppScreen.SelectAmount();
            if(selectedAmount == -1)
            {
                selectedAmount = AppScreen.SelectAmount();
            }
            else if(selectedAmount != 0)
            {
                transaction_amt = selectedAmount;
            }
            else
            {
                transaction_amt = Validator.Convert<int>($"amount {AppScreen.cur}");
            }
            
            // input validation 

            if(transaction_amt <= 0)
            {
                Utility.PrintMessage("Amount needs to be greater than zero.Try again",false);
                return;
            }
            if(transaction_amt % 500 != 0)
            {
                Utility.PrintMessage("You can only withdraw amount in multiples of 500 or 1000 usd. Try again.", false);
                return;
            }

            // business logic validations 

            if(transaction_amt > selectedAccount.AccountBalance) 
            {
                Utility.PrintMessage($"Withdrawal failed. Your balance is too low to withdraw ${Utility.FormatAmount(transaction_amt)}",false);
                return;
            }

            if((selectedAccount.AccountBalance - transaction_amt) < minimumKeptAmount)
            {
                Utility.PrintMessage($"Withdarawal failed. Your account needs to have " + $"minimum {Utility.FormatAmount(minimumKeptAmount)}", false);
                 return;
            }

            // bind withdarawal details to transaction object 

            InsertTransaction(selectedAccount.ID, TransactionType.Withdrawal, -transaction_amt, "");

            // update acc balance

            selectedAccount.AccountBalance -= transaction_amt;

            // success msg

            Utility.PrintMessage($"You have successfully withdraw" + $"{Utility.FormatAmount(transaction_amt)}.",true); 
            
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
            var filteredTransactionList = _listOfTransactions.Where(t => t.UserBankAccountID == selectedAccount.ID).ToList();

            // check if there's an transaction 

            if(filteredTransactionList.Count > 0)
            {
                Utility.PrintMessage("You have no transaction yet.", true);
            }
            else
            {
                var table = new ConsoleTable("ID", "Transaction Date","Type","Descriptions","Amount" + AppScreen.cur);
                foreach (var tran in filteredTransactionList)
                {
                    table.AddRow(tran.TransactionID, tran.TransactionDate, tran.TransactionType, tran.Description, tran.TransactionAmount);
                }
                table.Options.EnableCount = false;
                table.Write();
                Utility.PrintMessage($"You have {filteredTransactionList.Count} transaction(s)",true);
            }

        }

        private void ProcessInternalTransfer(InternalTransfer internalTransfer) 
        {
            if(internalTransfer.TransferAmount <= 0)
            {
                Utility.PrintMessage("Amound needs to be more than zero. Try again.", false);
                    return;  
            }
            // check sender's account balance

            if(internalTransfer.TransferAmount > selectedAccount.AccountBalance) 
            {
                Utility.PrintMessage($"Transfer failed. You do not have enough balance $ to transfer {Utility.FormatAmount(internalTransfer.TransferAmount)}",false);
                return;     
            }

            // check the minimum kept amount 

            if((selectedAccount.AccountBalance - internalTransfer.TransferAmount) < minimumKeptAmount) 
            {
                Utility.PrintMessage($"Transfer failed. Your account needs to have minimum $ {Utility.FormatAmount(minimumKeptAmount)}",false);
                return;
            }

            var selectedBankAccountReceiver = (from userAcc in userAccountList
                                               where userAcc.AccountNumber == internalTransfer.RecipientBankAccountNumber
                                               select userAcc).FirstOrDefault();

            if(selectedBankAccountReceiver == null) 
            {
                Utility.PrintMessage("Transfer failed. Receiver bank account number is invalid.",false);
                return;
            }
            
            // check receiver's name 

            if(selectedBankAccountReceiver.FullName != internalTransfer.RecipientBankAccounName)
            {
                Utility.PrintMessage("Transfer failed. Recipient;s  bank account name is invalid",false);
                return;
            }

            // add transaction to transactions record sender

            InsertTransaction(selectedAccount.ID,TransactionType.Transfer,-internalTransfer.TransferAmount,"Transfered " + $"to {selectedBankAccountReceiver.AccountNumber} ({selectedBankAccountReceiver.FullName})");

            // update sender's account balance

            selectedAccount.AccountBalance -= internalTransfer.TransferAmount;

            // add transaction record-receiver
            InsertTransaction(selectedBankAccountReceiver.ID, TransactionType.Transfer, internalTransfer.TransferAmount, "Transfered from " +
                $"{selectedAccount.AccountNumber}({selectedAccount.FullName})");

            // update reciver's account

            selectedBankAccountReceiver.AccountBalance += internalTransfer.TransferAmount;

            // print success message 

            Utility.PrintMessage($"You have successfully transfered" + $"{Utility.FormatAmount(internalTransfer.TransferAmount)} to " + $"{internalTransfer.RecipientBankAccounName}",true);
        }
    }
}
