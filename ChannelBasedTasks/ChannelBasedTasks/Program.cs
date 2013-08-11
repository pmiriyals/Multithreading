using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ChannelBasedTasks
{
    public class bQueue<T> where T : Task
    {
        private Queue<T> queue = new Queue<T>();

        public void Enqueue(T val)
        {
            lock (queue)
            {
                queue.Enqueue(val);
                Console.WriteLine("Enqueuing: {0}", val.ToString());
                if (queue.Count == 1)
                {
                    Monitor.PulseAll(queue);
                }
            }
        }

        public void Dequeue(Object obj)
        {
            lock (queue)
            {
                while (queue.Count == 0)
                {
                    Monitor.Wait(queue);
                }

                T task = queue.Dequeue();
                task.Execute();
                //return task;
            }            
        }
    }

    public class Task
    {
        public int ChannelID { get; set; }
        public string taskName { get; set; }

        public Task(string name, int cid)
        {
            this.taskName = name;
            this.ChannelID = cid;
        }

        public override string ToString()
        {
            return taskName;
        }

        public void Execute()
        {
            Console.WriteLine("{0} executed (belongs to {1})", taskName, ChannelID);
        }
    }
    
    class Program
    {
        static void driver(Task task)
        {
            Dictionary<int, bQueue<Task>> dict = new Dictionary<int, bQueue<Task>>();
            if (!dict.ContainsKey(task.ChannelID))
            {
                bQueue<Task> queue = new bQueue<Task>();
                dict.Add(task.ChannelID, queue);
                ThreadPool.QueueUserWorkItem(queue.Dequeue);
            }

            dict[task.ChannelID].Enqueue(task);
        }
        
        static void Main(string[] args)
        {
            Task[] task = new Task[10];
            int cid = 1;
            for (int i = 0; i < 10; i++)
            {
                if (i > 3)
                    cid = 2;
                if (i > 6)
                    cid = 3;
                
                task[i] = new Task(String.Format("Task {0}", i), cid);  
            }

            foreach (Task t in task)
            {
                driver(t);
            }
            Thread.Sleep(2000);
            Console.ReadLine();
        }
    }
}
