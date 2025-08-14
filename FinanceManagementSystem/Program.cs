using System;
using System.Collections.Generic;

namespace FinanceManagementSystem
{
    // A "record" is a special type that stores data in a simple way
    public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

    // This is like a rule for all transaction processors
    public interface ITransactionProcessor
    {
        void Process(Transaction transaction);
    }

    // For bank transfers
    public class BankTransferProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine("BANK TRANSFER: Sent " + transaction.Amount + " for " + transaction.Category);
        }
    }

    // For mobile money payments
    public class MobileMoneyProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine("MOBILE MONEY: Sent " + transaction.Amount + " for " + transaction.Category);
        }
    }

    // For crypto payments
    public class CryptoWalletProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine("CRYPTO WALLET: Sent " + transaction.Amount + " for " + transaction.Category);
        }
    }

    // Base account (general)
    public class Account
    {
        // Property for account number
        public string AccountNumber { get; }

        // Property for balance, can only be changed here or in child classes
        public decimal Balance { get; protected set; }

        // Constructor (runs when we make a new account)
        public Account(string accountNumber, decimal initialBalance)
        {
            AccountNumber = accountNumber;
            Balance = initialBalance;
        }

        // Virtual means this method can be changed in a child class
        public virtual void ApplyTransaction(Transaction transaction)
        {
            Balance = Balance - transaction.Amount; // subtract money
            Console.WriteLine("Applied transaction. New balance: " + Balance);
        }
    }

    // SavingsAccount (special kind of account)
    public sealed class SavingsAccount : Account
    {
        public SavingsAccount(string accountNumber, decimal initialBalance) 
            : base(accountNumber, initialBalance)
        {
        }

        // Change the ApplyTransaction method to check for enough money
        public override void ApplyTransaction(Transaction transaction)
        {
            if (transaction.Amount > Balance)
            {
                Console.WriteLine("Not enough money for this transaction of " + transaction.Amount);
            }
            else
            {
                Balance = Balance - transaction.Amount;
                Console.WriteLine("Transaction of " + transaction.Amount + " done. New balance: " + Balance);
            }
        }
    }

    // The main app that runs everything
    public class FinanceApp
    {
        // Store all transactions in a list
        private List<Transaction> transactions = new List<Transaction>();

        public void Run()
        {
            // Step 1: Make an account with 1000 starting balance
            SavingsAccount account = new SavingsAccount("SA-001", 1000m);
            Console.WriteLine("Account " + account.AccountNumber + " created with balance " + account.Balance);

            // Step 2: Make some transactions
            Transaction t1 = new Transaction(1, DateTime.Now, 150.75m, "Groceries");
            Transaction t2 = new Transaction(2, DateTime.Now, 300.00m, "Utilities");
            Transaction t3 = new Transaction(3, DateTime.Now, 800.00m, "Entertainment");

            // Step 3: Make processors
            ITransactionProcessor p1 = new MobileMoneyProcessor();
            ITransactionProcessor p2 = new BankTransferProcessor();
            ITransactionProcessor p3 = new CryptoWalletProcessor();

            // Step 4: Process and apply each transaction
            p1.Process(t1);
            account.ApplyTransaction(t1);
            transactions.Add(t1);

            p2.Process(t2);
            account.ApplyTransaction(t2);
            transactions.Add(t2);

            p3.Process(t3);
            account.ApplyTransaction(t3);
            transactions.Add(t3);

            // Step 5: Show all transactions at the end
            Console.WriteLine("\nAll Transactions:");
            foreach (Transaction t in transactions)
            {
                Console.WriteLine("ID: " + t.Id + " | Amount: " + t.Amount + " | Category: " + t.Category + " | Date: " + t.Date);
            }
        }
    }

    // Program starts here
    public class Program
    {
        public static void Main()
        {
            FinanceApp app = new FinanceApp();
            app.Run();
        }
    }
}
