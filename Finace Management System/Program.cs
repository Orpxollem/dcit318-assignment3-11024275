using System;
using System.Collections.Generic;

namespace Finance_Management_System {

    public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

    public interface ITransactionProcessor
    {
        void Process(Transaction transaction);
    }

    public class BankTransferProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine(">> Processed bank transfer of GHS" + transaction.Amount + " for " + transaction.Category);
        }
    }

    public class MobileMoneyProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine(">> Processed mobile money transfer of GHS" + transaction.Amount + " for " + transaction.Category);
        }
    }

    public class CryptoWalletProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine(">> Processed crypto wallet transfer of GHS" + transaction.Amount + " for " + transaction.Category);
        }
    }

    public class Account
    {
        public string AccountNumber { get; set; }
        public decimal Balance { get; protected set; }

        public Account(string accountNumber, decimal initialBalance)
        {
            AccountNumber = accountNumber;
            Balance = initialBalance;
        }

        public virtual void ApplyTransaction(Transaction transaction)
        {
            Balance -= transaction.Amount;
        }
    }

    public sealed class SavingsAccount : Account
    {
        public SavingsAccount(string accountNumber, decimal initialBalance) : base(accountNumber, initialBalance) { }

        public override void ApplyTransaction(Transaction transaction)
        {
            if (transaction.Amount > Balance)
            {
                Console.WriteLine("Insufficient funds");
            }
            else
            {
                Balance -= transaction.Amount;
                Console.WriteLine("> Transaction successful. New Balance: GHS" + Balance + "\n");
            }
        }
    }

    public class FinanceApp
    {
        private List<Transaction> _transactions = new List<Transaction>();

        public void Run()
        {
            SavingsAccount account = new SavingsAccount("MKK-11024275", 1000);

            Transaction t1 = new Transaction(1, DateTime.Now, 450, "Fees");
            Transaction t2 = new Transaction(2, DateTime.Now, 300, "Books");
            Transaction t3 = new Transaction(3, DateTime.Now, 120, "Investment");

            var mobileMoney = new MobileMoneyProcessor();
            var bank = new BankTransferProcessor();
            var cryptowallet = new CryptoWalletProcessor();

            mobileMoney.Process(t1);
            account.ApplyTransaction(t1);

            bank.Process(t2);
            account.ApplyTransaction(t2);

            cryptowallet.Process(t3);
            account.ApplyTransaction(t3);

            _transactions.AddRange(new[] { t1, t2, t3 });
        }
    }

    public class Program
    {
        public static void Main()
        {
            var app = new FinanceApp();
            app.Run();
        }
    }

}