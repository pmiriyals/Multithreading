using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace UsingEvents
{
    class Program
    {
        static AutoResetEvent autoevent;
        static ManualResetEvent manevent;

        static void ManThread()
        {
            Console.WriteLine("worker thread {0}, now waiting on events", Thread.CurrentThread.Name);
            manevent.WaitOne();
            Console.WriteLine("worker thread {0}, reactivated, now exiting", Thread.CurrentThread.Name);
        }

        static void threadproc()
        {
            Console.WriteLine("worker thread started, now waiting on events");
            manevent.WaitOne();
            Console.WriteLine("worker thread reactivated, now exiting");
        }
        
        static void Main(string[] args)
        {
            autoevent = new AutoResetEvent(false);
            manevent = new ManualResetEvent(false);

            Thread[] t = new Thread[5];

            for (int i = 0; i < 5; i++)
            {
                t[i] = new Thread(new ThreadStart(ManThread));
                t[i].Name = i.ToString();
            }

            Console.WriteLine("Main thread starting worker threads");
            for (int i = 0; i < 5; i++)
            {
                t[i].Start();
            }

            Console.WriteLine("Main thread sleeping for 2 s");
            Thread.Sleep(2000);
            manevent.Set();
            //Thread t = new Thread(new ThreadStart(threadproc));
            //Console.WriteLine("Main thread starting worker thread");
            
            //t.Start();
            //Console.WriteLine("Main thread sleeping for 2 s");
            //Thread.Sleep(2000);
            //Console.WriteLine("Main thread signaling to release blocked threads");
            //autoevent.Set();
            //if (!t.Join(2000))
            //{
            //    Console.WriteLine("Main thread inside if, after timeout to abort thread");
            //    t.Abort();
            //}           
            
            Console.ReadLine();
        }
    }
}
