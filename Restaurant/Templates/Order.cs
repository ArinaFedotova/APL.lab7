using System;

namespace Restaurant.Templates
{
    public class Order
    {
        public Status Status { get; private set; }
        public int Id;
        public string dish;
        public DateTime RecieveTime;
        public Order(int id, string dishPicked)
        {
            Status = new Status();
            Id = id;
            dish = dishPicked;
            RecieveTime = DateTime.Now;
        }
    }
}