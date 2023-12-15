using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using Restaurant.Templates;

namespace Restaurant.staff
{
    public class Waiter : IStaff
    {
        public int Id;
        public bool IsWork;
        public Thread thread;
        public Queue<Order> takenOrders;
        public Order newOrder;
        private static readonly Random _random = new Random();
        private readonly int processingTime;
        private readonly int deltaTime;
        private readonly object staffLock = new object();
        private readonly object queueOrderLock;
        public DateTime lastWork;
        
        public Waiter(int id, object orderLock, Queue<Order> queueToCook)
        {
            thread = new Thread(Work);
            Id = id;
            IsWork = true;
            takenOrders = queueToCook;
            queueOrderLock = orderLock;
            lastWork = DateTime.Now;
            
            var processTime = ConfigurationManager.AppSettings.Get("ProcessingTime");
            var delta = ConfigurationManager.AppSettings.Get("Delta");
            if (int.TryParse(processTime, out processingTime) && int.TryParse(delta, out deltaTime))
            {
                var modeValue = processingTime;
                var maxValue = processingTime + deltaTime;
                var minValue = processingTime - deltaTime;
                double x = _random.NextDouble();
               processingTime = (int)((x < 0.5) // в min/max части
                    ? minValue + Math.Sqrt(x * (maxValue - minValue) * deltaTime)
                    : maxValue - Math.Sqrt((1 - x) * (maxValue - minValue) * deltaTime));
            }
            
            thread.Start();
        }
        
        public void StartWork(Order order)
        {
            lock (staffLock)
            {
                newOrder = order;
                Monitor.Pulse(staffLock);
            }
            
        }
        public void Work(object obj)
        {
            while (IsWork)
            {
                lock (staffLock)
                {
                    Monitor.Wait(staffLock);
                    if (!IsWork)
                        break;
                    Thread.Sleep(processingTime);
                    newOrder.Status.Next();
                    lock (queueOrderLock)
                    { 
                        takenOrders.Enqueue(newOrder);
                    }
                    lastWork = DateTime.Now;
                    lock (Logger.Logger.GetLogLock())
                    {
                        Logger.Logger.Log($"Waiter {Id.ToString()} processed order {newOrder.Id.ToString()}");
                    }
                    
                }
            }
        }

        public void FinishWork()
        {
            lock (staffLock)
            {
                IsWork = false;
                thread.Join();
                Monitor.Pulse(staffLock);
            }
        }
        
        public bool GetStatus()
        {
            return IsWork;
        }
        
        public object GetStaffLock()
        {
            return staffLock;
        }
        
        public string GetStatusName()
        {
            return "waiter";
        }
        
        public DateTime GetTimeOfLastWork()
        {
            return lastWork;
        }
        
        public int GetId()
        {
            return Id;
        }
    }
}