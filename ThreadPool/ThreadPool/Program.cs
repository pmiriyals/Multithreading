using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ThreadPool
{
    class Program
    {
        class Sample
        {
            public void ThreadProc(Object stateInfo)
            {                
                Console.WriteLine("ThreadProc executed by {0} thread, id = {1}", stateInfo, Thread.CurrentThread.ManagedThreadId);
                Thread.Sleep(2000);
            }
        }
        
        static void Main(string[] args)
        {
            Sample s = new Sample();
            Object x = new Object();
            x = 10;
            System.Threading.ThreadPool.QueueUserWorkItem(s.ThreadProc, x);
            //Thread.Sleep(1000);
            Console.WriteLine("Processors = {0}", Environment.ProcessorCount);
            Console.WriteLine("{0} Main thread exiting", Thread.CurrentThread.ManagedThreadId);
            Thread.Sleep(2000);
            //Console.ReadLine();
        }
    }
}
