using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SimpleDemo
{
    class Program
    {
        //This is a shared resoruces. All 11 threads counting the main thread will have access to this.
        static int sum = 0;
        //static volatile int[] a = new int[100000];
        //Critical Section. The state of the sum will depend on the specific order of the threads accessing it.
        //The final output of the program is undefined. And the reason for this is the cause of race condition.
        static void addone()
        {
            Thread.Sleep(new Random().Next(0, 1000));
            //Interlocked.Add(ref sum, 10);
            Interlocked.Increment(ref sum); //1. Get the address of a memory location into a register (ecx, holds address of sum variable in memory)
                                            //2. Put another value into a register (eax, holds value = 1, which will be added in step 3)
                                            // 3. In 1 uninterruptable instruction perform a read modified write to the location in step 1. Thread safe.
                                            // 4. This is processor independent.
            Console.WriteLine("Sum incremented by thread = {0} and sum = {1}", Thread.CurrentThread.Name, sum);            
        }

        static void Main(string[] args)
        {
            Thread[] t = new Thread[10];
            

            for (int i = 0; i < 10; i++)
            {
                t[i] = new Thread(new ThreadStart(addone));
                t[i].Name = i.ToString();
                t[i].Start();
            }

            //1. if one thread is sleeping, the process remains executing since by default the background thread property is set to false
            t[9].IsBackground = true;   //2. setting this to true will make thread 9 as background thread, hence even if 9 is sleeping the process finishes

            for (int i = 0; i < t.Length; i++)
                t[i].Join();    //3. But if I call Join method on 9th thread, the main thread will wait at this point until 9th thread finishes

            Console.ReadLine();
        }
    }
}
