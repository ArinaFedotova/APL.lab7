using System;
using System.Collections.Generic;

namespace Restaurant
{
    public class Kitchen
    {
        public delegate void OrderHandler(NeOrder neOrder);
        // public event NeOrder newOrder;

        public Kitchen()
        {
            ICook chief = new Chief{id = 1};
            ICook cook1 = new Cook{id = 1};
            ICook cook2 = new Cook{id = 2};

            // newOrder += cook1.HandleOrder();
            // newOrder += cook2.HandleOrder;
            // newOrder += chief.HandleOrder;
        }
        
        public void ProcessOrders(Queue<NeOrder> orderQueue)
        {
            while (orderQueue.Count > 0)
            {
                NeOrder neOrder = orderQueue.Dequeue();

                // Cook1?.Invoke(neOrder);
                // Cook2?.Invoke(neOrder);
                // Chef?.Invoke(neOrder);
            }
        }
        
        
    }
}