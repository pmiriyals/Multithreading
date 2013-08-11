using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace BankApplication
{
    public class BankAccount
    {
        private Mutex _lock = new Mutex();
        public long AccountNumber { get; set; }
        public decimal balance { get; set; }

        public BankAccount(long acctnum) : this(acctnum, 0) { }
        public BankAccount(long acctnum, decimal bal)
        {
            this.AccountNumber = acctnum;
            this.balance = bal;
        }

        public void Credit(decimal amt)
        {
            if (_lock.WaitOne())
            {
                try
                {
                    decimal temp = balance;
                    temp += amt;
                    Random r = new Random();
                    Thread.Sleep(r.Next(0, 20));
                    balance = temp;
                }
                finally
                {
                    _lock.ReleaseMutex();
                }
            }
        }

        public void Debit(decimal amt)
        {
            Credit(-amt);
        }

        public void TransferFrom(BankAccount acct, decimal amt)
        {
            Console.WriteLine("[{0}] Transferring {1:C0} from account {2} to {3}", Thread.CurrentThread.Name, amt, acct.AccountNumber, this.AccountNumber);
            Mutex[] mutex = { this._lock, acct._lock };
            if (WaitHandle.WaitAll(mutex))
            {
                try
                {
                    acct.Debit(amt);
                    this.Credit(amt);
                }
                finally
                {
                    foreach (Mutex m in mutex)
                        m.ReleaseMutex();
                }
            }
        }
    }

    class Program
    {
        public static BankAccount[] ba;
        static bool done = false;

        static void RandomTransfer()
        {
            int max_transfer = 500;
            Random r = new Random();
            while(!done)
            {
                int fromacct = r.Next(0, 10);
                int toacct = r.Next(0, 10);
                int amt = r.Next(0, max_transfer);
                while (fromacct == toacct)
                    toacct = r.Next(0, 10);

                ba[toacct].TransferFrom(ba[fromacct], amt);
            //    Thread.Sleep(r.Next(0, 1000));
            }
        }

        static bool validateTransfer(int num_accts, decimal initial_amt)
        {
            decimal total_cur_amt = 0.0M;
            decimal total_initial_amt = num_accts * initial_amt;
            Console.WriteLine("\nFinal balances = ");
            
            foreach (BankAccount acct in ba)
            {
                Console.WriteLine("Account {0} balance = {1:C0}", acct.AccountNumber, acct.balance);                
                total_cur_amt += acct.balance;
            }
            if (total_cur_amt == total_initial_amt)
            {
                Console.WriteLine("\nAccounts are consistent. Total Balance = {0:C0}", total_cur_amt);
                return true;
            }
            else
            {
                Console.WriteLine("\nAccounts are inconsistent. Total Balance = {0:C0}", total_cur_amt);
                return false;
            }
        }

        static void Main(string[] args)
        {
            int procCnt = Environment.ProcessorCount;
            int num_threads = 10;
            int num_accts = 10;
            decimal initial_amt = 1000.00M;
            ba = new BankAccount[num_accts];
            Thread[] t = new Thread[num_threads];

            for (int i = 0; i < ba.Length; i++)
            {
                ba[i] = new BankAccount(i, initial_amt);
                t[i] = new Thread(new ThreadStart(RandomTransfer));
                t[i].Name = String.Format("TX: {0}", i);                
            }

            for(int i = 0; i<num_threads; i++)
                t[i].Start();

            Thread.Sleep(5000);
            done = true;
            for (int i = 0; i < num_threads; i++)
                t[i].Join();

            //RandomTransfer();
            validateTransfer(num_accts, initial_amt);
            Console.ReadLine();
        }
    }
}
