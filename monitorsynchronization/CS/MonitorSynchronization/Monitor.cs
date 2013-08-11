
using System;
using System.Threading;


class Resource {
   public void Access(Int32 threadNum) {
      lock (this) {
         Console.WriteLine("Start Resource access (Thread={0})", threadNum);
         Thread.Sleep(500);
         Console.WriteLine("Stop  Resource access (Thread={0})", threadNum);
      }
   }
}


class App {
   const Int32 numThreadCount = 5;
   static Int32 numAsyncOps = numThreadCount;
   static AutoResetEvent asyncOpsAreDone = new AutoResetEvent(false);
   static Resource res = new Resource();

   public static void Main() {
      for (Int32 threadNum = 0; threadNum < numThreadCount; threadNum++) {
         ThreadPool.QueueUserWorkItem(new WaitCallback(UpdateResource), threadNum);
      }

      asyncOpsAreDone.WaitOne();
      Console.WriteLine("All operations have completed.");
      Console.ReadLine();
   }


   // The callback method's signature MUST match that of a System.Threading.TimerCallback 
   // delegate (it takes an Object parameter and returns void)
   static void UpdateResource(Object state) {
      res.Access((Int32) state);
      if (Interlocked.Decrement(ref numAsyncOps) == 0)
         asyncOpsAreDone.Set();
   }
}
