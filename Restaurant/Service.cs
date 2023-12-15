using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Timers;
using Restaurant.Managers;
using Restaurant.staff;
using Restaurant.Templates;
using Logger;

namespace Restaurant
{
    public class Service
    {
        private System.Timers.Timer aTimer;
        
        public Hopper allQueues;
        
        private static readonly Random _random = new Random();
        private static readonly int emitFrequency = int.Parse(ConfigurationManager.AppSettings.Get("EmittFrequency"));

        private int amoumtOfEachStaff = 2;
        private int amountOfOrders;
        public DateTime LastOperationWithOrder;
        
        public Service(List<string> listOrders, List<int> amountOfStaff)
        {
            amountOfOrders = listOrders.Count;
            aTimer = new System.Timers.Timer();
            lock (Logger.Logger.GetLogLock())
            {
                Logger.Logger.Log("The application started");
            }
            SetTimer();
            allQueues = new Hopper();
            
            Queue<Order> order1 = new Queue<Order>();
            Queue<Order> order2 = new Queue<Order>();
            Queue<Order> order3 = new Queue<Order>();

            allQueues.StaffAndLevel.Add(0, new Tuple<List<IStaff>, Queue<Order>>(new List<IStaff>(), order1));
            allQueues.StaffAndLevel.Add(1, new Tuple<List<IStaff>, Queue<Order>>(new List<IStaff>(), order2));
            allQueues.StaffAndLevel.Add(2, new Tuple<List<IStaff>, Queue<Order>>(new List<IStaff>(), order3));
            
            for (int lvl = 0; lvl < 3; lvl ++)
            {
                for (int i = 0; i < amountOfStaff[lvl]; i++)
                {
                    AddStaffAndQueue(lvl, allQueues.StaffAndLevel[lvl].Item1);
                }
                    
            }
            
            for (int i = 0; i < listOrders.Count; i++)
            {
                Thread.Sleep(emitFrequency);
                lock (Logger.Logger.GetLogLock())
                {
                    Logger.Logger.Log($"Emmiter generated an order {i}: {listOrders[i]}");
                }
                
                allQueues.StaffAndLevel[0].Item2.Enqueue(new Order(i, listOrders[i]));
            }
            aTimer.Start();
        }
        
        private void AddStaffAndQueue(int level, List<IStaff> staffList)
        {
            IStaff staffMember = null;
            switch (level)
            {
                case 0:
                    staffMember = new staff.Waiter(staffList.Count + 1, allQueues.Level1Lock,
                        allQueues.StaffAndLevel[level+1].Item2);
                    break;
                case 1:
                    staffMember = new staff.Cook(staffList.Count + 1, allQueues.Level2Lock, 
                        allQueues.StaffAndLevel[level+1].Item2);
                    break;
                case 2:
                    staffMember = new staff.Chief(staffList.Count + 1, allQueues.Level3Lock);
                    break;
            }
            staffList.Add(staffMember);
        }
        private void SetTimer()
        {
            aTimer = new System.Timers.Timer(_random.Next(0, emitFrequency));
            aTimer.Elapsed += OnTimeEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
            lock (Logger.ReportLog.GetLock())
            {
                Logger.ReportLog.Begin = DateTime.Now;
            }
            lock (Logger.Logger.GetLogLock())
            {
                Logger.Logger.Log("Timer started");
            }
            
        }

        private void OnTimeEvent(Object source, ElapsedEventArgs e)
        {
            Start();
        }
        public void Start()
        {
            if (allQueues.StaffAndLevel.Count == 0)
            {
                lock (Logger.Logger.GetLogLock())
                {
                    Logger.Logger.Log("End Program");
                }
                aTimer.Elapsed -= OnTimeEvent;
                aTimer.Stop();
                Logger.ReportLog.End = DateTime.Now;
                Console.WriteLine("Num");
                Console.WriteLine(Process.GetCurrentProcess().Threads.Count);
                return;
            }
            
            foreach (Tuple<List<IStaff>, Queue<Order>> pair in allQueues.StaffAndLevel.Values)
            {
                foreach (IStaff oneWorker in pair.Item1)
                {
                    lock (allQueues.GetLock(pair))
                    {
                        if (pair.Item2.Count == 0)
                            break;
                    }

                    var isFree = Monitor.TryEnter(oneWorker.GetStaffLock());
                    if (isFree)
                    {
                        Monitor.Exit(oneWorker.GetStaffLock());
                        lock (allQueues.GetLock(pair)) 
                        {
                            if (pair.Item2.Count > 0)
                            {
                                oneWorker.StartWork(pair.Item2.Dequeue());
                                Logger.ReportLog.LastOrderProcessed = DateTime.Now;
                            }
                        }
                    }
                }
            }
            allQueues.FreeWorker();
        }
    }
}