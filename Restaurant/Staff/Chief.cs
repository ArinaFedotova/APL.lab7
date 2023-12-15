using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using Restaurant.Templates;

namespace Restaurant.staff
{
    public class Chief : IStaff
    {
        public int Id;
        public bool IsWork;
        public Thread thread;
        public Queue<Order> finishedOrders;
        public Order servingOrder;
        private static readonly Random _random = new Random();
        private readonly int processingTime;
        private readonly int deltaTime;
        private readonly object staffLock = new object();
        private readonly object queueOrderLock;
        private DateTime lastWork;
        
        public string status = StatusLevel.Serving.ToString();
        
        public Chief(int id, object orderLock)
        {
            thread = new Thread(Work);
            Id = id;
            IsWork = true;
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
                servingOrder = order;
                Monitor.Pulse(staffLock);
            }
            
        }

        public void Work(object obj)
        {
            while (IsWork)
            {
                //Оператор lock гарантирует, что в любой момент времени только один поток выполняет свой текст.
                lock (staffLock)
                {
                    Monitor.Wait(staffLock);
                    if (!IsWork)
                        break;
                    Thread.Sleep(processingTime);
                    servingOrder.Status.Next();
                    lastWork = DateTime.Now;
                    lock (Logger.Logger.GetLogLock())
                    {
                        Logger.Logger.Log($"Chief {Id.ToString()} processed order {servingOrder.Id.ToString()}");
                    }
                }
                
            }
        }
        
        public void FinishWork()
        {
            lock (staffLock)
            {
                IsWork = false;
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
            return "chief";
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