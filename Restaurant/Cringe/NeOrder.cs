using System;
using System.Collections.Generic;
using System.Configuration;
using System.Collections.Specialized;
using System.Threading;

namespace Restaurant
{
    public class NeOrder
    {
        public delegate void curData(int id);
        public event curData? NewOrder;

        public int id;
        // private Queue<Action> ordersQueue = new Queue<Action>();
        private readonly int emitFrequency; // частота генерации событий в миллисекундах
        private static readonly Random _random = new Random();
        private readonly int processingTime;
        
        
        public NeOrder(int id)
        {
            var fixTime = ConfigurationManager.AppSettings.Get("EmittFrequency");
            if (int.TryParse(fixTime, out emitFrequency))
                emitFrequency += _random.Next();
            
            var processTime = ConfigurationManager.AppSettings.Get("ProcessingTime");
            if (int.TryParse(processTime, out processingTime))
                processingTime += _random.Next();
            
            this.id = id;
        }

        public void OnEvent(int id)
        {
            int randomDelay = _random.Next(0, processingTime); // случайная задержка до генерации следующего события
            Thread.Sleep(randomDelay);
            NewOrder?.Invoke(id);
        }

        public void GeneratingOrders()
        {
            while (true)
            {
                OnEvent(id);
                Thread.Sleep(emitFrequency + _random.Next());
            }
        }
    }
}