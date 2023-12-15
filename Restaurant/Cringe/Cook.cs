using System;

namespace Restaurant
{
    public class Cook : ICook
    {
        public int id { get; set; }

        public void HandleOrder(NeOrder neOrder)
        {
            Console.WriteLine($"Cook {id.ToString()} is handling order {neOrder.id.ToString()}");
        }
    }
    
    public class Chief : ICook
    {
        public int id { get; set; }

        public void HandleOrder(NeOrder neOrder)
        {
            Console.WriteLine($"Chief {id.ToString()} is handling order {neOrder.id.ToString()}");
        }
    }
}