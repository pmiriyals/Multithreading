using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace FibonacciThreadPool
{
    public class Fibonacci
    {
        private int n;
        private ManualResetEvent doneevent;

        public Fibonacci(int n, ManualResetEvent doneevent)
        {
            this.n = n;
            this.doneevent = doneevent;
        }

        public void FibCallBack(object state)
        {            
            Int32 threadindex = Int32.Parse(state.ToString());
            Console.WriteLine("\nThread {0} calculating fibonacci({1})", threadindex, n);
            Console.WriteLine("Thread {0} Calculated Fib({1}) = {2}", threadindex, n, FibN(n));
            Console.WriteLine("Thread {0} signaling done\n", threadindex);
            
            doneevent.Set();
        }

        private int FibN(int n)
        {
            int a = 0;
            int b = 1;
            int c;
            for (int i = 2; i <= n; i++)
            {
                c = a + b;
                a = b;
                b = c;
            }
            return b;
        }
    }

    class Program
    {
        static int Fib(int n)
        {
            int a = 0;
            int b = 1;
            int c;
            for (int i = 2; i <= n; i++)
            {
                c = a + b;
                a = b;
                b = c;
            }
            return b;
        }
        
        static void Main(string[] args)
        {
            int max = 64;
            Fibonacci[] f = new Fibonacci[max];
            ManualResetEvent[] doneEvents = new ManualResetEvent[max];

            Random r = new Random();
            int n;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < max; i++)
            { 
                doneEvents[i] = new ManualResetEvent(false);
                f[i] = new Fibonacci(46, doneEvents[i]);
                ThreadPool.QueueUserWorkItem(new WaitCallback(f[i].FibCallBack), i);
            }
            
            WaitHandle.WaitAll(doneEvents);
            sw.Stop();
            sw.Reset();
            Console.WriteLine("Total time elapsed for threads = " + sw.ElapsedMilliseconds);
            sw.Start();
            for (int i = 0; i < max; i++)
            {
                Console.WriteLine(" Fib = " + Fib(46));
            }
            sw.Stop();
            Console.WriteLine("Total time elapsed for single thread = " + sw.ElapsedMilliseconds);
            Console.ReadLine();
        }
    }
}
