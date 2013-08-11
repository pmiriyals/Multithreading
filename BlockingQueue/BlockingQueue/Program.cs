using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;

namespace BlockingQueue
{
    public class bQueue
    {
        private Queue<int> queue = new Queue<int>();
        private const int MAX = 5;

        private void EnQueue(int val)
        {
            lock (queue)
            {                
                while (queue.Count == MAX)
                {
                    Monitor.Wait(queue);                 
                }
                Console.WriteLine("EnQueueing: {0}", val);
                queue.Enqueue(val);
                if (queue.Count == 1)
                {
                    Monitor.PulseAll(queue);
                }
            }
        }

        private int DeQueue()
        {            
            lock (queue)
            {
                Console.WriteLine("Thread at deq start, count = "+ queue.Count);
                while (queue.Count == 0)
                {   
                    Monitor.Wait(queue);                    
                }
                int val = queue.Dequeue();
                if (queue.Count == MAX - 1)
                {
                    Monitor.PulseAll(queue);
                }
                Console.WriteLine("Dequeueing: {0}", val);
                return val;
            }
        }

        public void enQcallback()
        {
            for (int i = 1; i <= 1; i++)
                EnQueue(i);
            Console.WriteLine("Thread exiting enQcallback");
        }

        public void deQcallback()
        {
            for (int i = 1; i <= 1; i++)
                DeQueue();
            Console.WriteLine("Thread exiting deQcallback");
        }

    }
    
    class Program
    {
        static void Main(string[] args)
        {
            bQueue queue = new bQueue();

            Thread enQ = new Thread(new ThreadStart(queue.enQcallback));
            Thread deQ = new Thread(new ThreadStart(queue.deQcallback));
            Console.WriteLine("Main starting deQ");
            deQ.Start();
            Console.WriteLine("Main sleeping for 2 s");
            Thread.Sleep(2000);
            Console.WriteLine("Main starting enQ");
            enQ.Start();
            Console.ReadLine();
        }
    }
}
