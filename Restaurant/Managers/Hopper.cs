using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using Restaurant.staff;
using Restaurant.Templates;

namespace Restaurant.Managers
{
    public class Hopper
    {
        private int timeBreak = 20;
        
        public Dictionary<int, Tuple<List<IStaff>, Queue<Order>>> StaffAndLevel;
        
        public readonly object Level1Lock = new object();
        public readonly object Level2Lock = new object();
        public readonly object Level3Lock = new object();

        public Hopper()
        {
            StaffAndLevel = new Dictionary<int, Tuple<List<IStaff>, Queue<Order>>>();
        }

        public void FreeWorker()
        {
            var inactiveWorkers = new List<IStaff>();
            foreach (var keyValuePair in StaffAndLevel)
            {
                foreach (var worker in keyValuePair.Value.Item1)
                {
                    var IsFree = Monitor.TryEnter(worker.GetStaffLock());
                    if (IsFree)
                    {
                        Monitor.Exit(worker.GetStaffLock());
                        if (worker.GetTimeOfLastWork().AddSeconds(timeBreak) < DateTime.Now)
                        {
                            inactiveWorkers.Add(worker);
                            lock (Logger.Logger.GetLogLock())
                            {
                                Logger.Logger.Log($"Deactivate {worker.GetStatusName()} " +
                                                      $"{worker.GetId().ToString()}");
                            }
                        }
                    }
                }
            }  
            foreach (var keyValuePair in StaffAndLevel)
            {
                keyValuePair.Value.Item1.RemoveAll(worker => inactiveWorkers.Contains(worker));
                if (keyValuePair.Value.Item1.Count == 0)
                {
                    StaffAndLevel.Remove(keyValuePair.Key);
                }
            }
    
        }

        public object GetLock(Tuple<List<IStaff>, Queue<Order>> pair)
        {
            if (pair.Item1[0] is Restaurant.staff.Waiter)
                return Level1Lock;
            if (pair.Item1[0] is Restaurant.staff.Cook)
                return Level2Lock;
            if (pair.Item1[0] is Restaurant.staff.Chief)
                return Level3Lock;
            throw new Exception("Non locker");
        }
    }
}