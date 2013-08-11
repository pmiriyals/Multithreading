using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace DataPartitioning
{
    public class ArrayProcessor
    {
        public double[] data;
        public int si;
        public int ei;
        public double sum {get; set;}

        public ArrayProcessor(double[] data, int si, int ei)
        {
            this.data = data;
            this.si = si;
            this.ei = ei;
            sum = 0.0;
        }

        public void computesum()
        {
            for (int i = si; i <= ei; i++)
                sum += data[i];
        }
    }
    
    class Program
    {
        public static double[] getdata()
        {
            double[] data = new double[50000000];
            for (int i = 0; i < data.Length; i++)
                data[i] = i;
            return data;
        }

        static void Main(string[] args)
        {
            int corecount = Environment.ProcessorCount;
            double[] data = getdata();

            singlethread(corecount, data);
            //multithread(corecount, data);
            Console.ReadLine();
        }

        private static void multithread(int corecount, double[] data)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            ArrayProcessor[] ap = new ArrayProcessor[corecount];
            Thread[] threads = new Thread[corecount];
            int indexesperthread = data.Length / corecount;
            int leftoverindex = data.Length % corecount;

            for (int i = 0; i < corecount; i++)
            {
                int firstindex = i * indexesperthread;
                int lastindex = firstindex + indexesperthread - 1;

                if (i == corecount - 1)
                {
                    lastindex += leftoverindex;
                }

                ap[i] = new ArrayProcessor(data, firstindex, lastindex);
                threads[i] = new Thread(new ThreadStart(ap[i].computesum));
                threads[i].Start();
            }

            double sum = 0.0;
            for (int i = 0; i < corecount; i++)
            {
                threads[i].Join();  //wait for thread to exit
                sum += ap[i].sum;
            }

            sw.Stop();
            Console.WriteLine("Processor Count = {0}\nTotal sum = {1}\nTotal time taken = {2}", corecount, sum, sw.ElapsedMilliseconds);
        }

        private static void singlethread(int corecount, double[] data)
        {
            ArrayProcessor ap = new ArrayProcessor(data, 0, data.Length - 1);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            ap.computesum();
            sw.Stop();
            Console.WriteLine("Processor count = {2}\nTotal sum = {0} \nTotal time taken = {1}", ap.sum, sw.ElapsedMilliseconds, corecount);
        }
    }
}
