using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObserverDesignPattern
{

    public class BaggageInfo
    {
        public int flightnum { get; set; }
        public int carouselnum { get; set; }
        public string flightFrom { get; set; }

        public BaggageInfo(int flightnum, int carouselnum, string flightFrom)
        {
            this.flightnum = flightnum;
            this.carouselnum = carouselnum;
            this.flightFrom = flightFrom;
        }
    }

    public class BaggageHandler : IObservable<BaggageInfo>
    {
        private List<IObserver<BaggageInfo>> observers;
        private List<BaggageInfo> flights;

        public BaggageHandler()
        {
            observers = new List<IObserver<BaggageInfo>>();
            flights = new List<BaggageInfo>();
        }

        public IDisposable Subscribe(IObserver<BaggageInfo> observer)
        {
            if (!observers.Contains(observer))
            {
                observers.Add(observer);
                foreach (BaggageInfo bi in flights)
                    observer.OnNext(bi);
            }
            return new Unsubsriber<BaggageInfo>(observers, observer);
        }

        public void BaggageStatus(int flightnum)
        {
            BaggageStatus(flightnum, String.Empty, 0);
        }

        public void BaggageStatus(int flightnum, String from, int carousel)
        {
            BaggageInfo bi = new BaggageInfo(flightnum, carousel, from);
            List<BaggageInfo> toremove = new List<BaggageInfo>();
            if (carousel == 0)
            {                
                foreach (BaggageInfo binfo in flights)
                {
                    if (bi.flightnum == flightnum)
                    {
                        toremove.Add(binfo);
                        foreach (IObserver<BaggageInfo> observer in observers)
                            observer.OnNext(binfo);
                    }
                }
            }
            else
            {                
                flights.Add(bi);
                foreach (IObserver<BaggageInfo> obi in observers)
                    obi.OnNext(bi);
            }
            foreach (BaggageInfo bin in toremove)
                flights.Remove(bin);
        }

        public void LastBaggageClaimed()
        {
            foreach (IObserver<BaggageInfo> bi in observers)
                bi.OnCompleted();

            observers.Clear();            
        }
    }

    public class Unsubsriber<T> : IDisposable
    {
        private IObserver<T> observer;
        private List<IObserver<T>> lstObservers;

        public Unsubsriber(List<IObserver<T>> observers, IObserver<T> observer)
        {
            this.lstObservers = observers;
            this.observer = observer;
        }

        public void Dispose()
        {
            if (lstObservers.Contains(observer))
                lstObservers.Remove(observer);
        }
    }

    public class ArrivalsMonitor : IObserver<BaggageInfo>
    {
        public string name { get; set; }
        public List<int> flightInfos = new List<int>();
        private IDisposable cancellation;

        public ArrivalsMonitor(string name)
        {
            this.name = name;
        }

        public virtual void Subscribe(BaggageHandler provider)
        {
            cancellation = provider.Subscribe(this);
        }

        public virtual void Unsubscribe()
        {
            cancellation.Dispose();
            flightInfos.Clear();
        }

        public virtual void OnCompleted()
        {
            flightInfos.Clear();
        }

        public virtual void OnError(Exception e)
        {
            throw new NotImplementedException();
        }

        public virtual void OnNext(BaggageInfo bi)
        {
            List<int> toremove = new List<int>();
            if (bi.carouselnum == 0)
            {
                foreach (int flight in flightInfos)
                {
                    if (flight == bi.flightnum)
                    {
                        toremove.Add(flight);
                        Console.WriteLine("Removing flight = " + flight);                        
                    }
                }
            }
            else
            {
                flightInfos.Add(bi.flightnum);
                Console.WriteLine("Num = {0}\tCarNum = {1}\tLoc = {2}\tMonitor = {3}", bi.flightnum, bi.carouselnum, bi.flightFrom, this.name);             
            }
            foreach (int i in toremove)
                flightInfos.Remove(i);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            BaggageHandler provider = new BaggageHandler();
            ArrivalsMonitor observer1 = new ArrivalsMonitor("BCM");
            ArrivalsMonitor observer2 = new ArrivalsMonitor("Security");

            provider.BaggageStatus(712, "Detroit", 3);
            observer1.Subscribe(provider);
            provider.BaggageStatus(712, "Kalamazoo", 3);
            provider.BaggageStatus(400, "New York-Kennedy", 1);
            provider.BaggageStatus(712, "Detroit", 3);
            observer2.Subscribe(provider);
            provider.BaggageStatus(511, "San Francisco", 2);
            provider.BaggageStatus(712);
            observer2.Unsubscribe();
            provider.BaggageStatus(400);
            provider.LastBaggageClaimed();
            Console.ReadLine();
        }
    }
}
