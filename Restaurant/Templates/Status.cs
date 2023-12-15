using System;

namespace Restaurant.Templates
{
    // public static class StatusLevel
    // {
    //     public const string One = "Order";
    //     public const string Second = "Cooking";
    //     public const string Third = "Serving";
    //     public const string Final = "Done";
    // }
    enum StatusLevel
    {
        Ordering,
        Cooking,
        Serving,
        Done
    }
    
    public class Status
    {
        private StatusLevel _orderStatus;

        public Status()
        {
            _orderStatus = StatusLevel.Ordering;
        }

        public string Get()
        {
            return _orderStatus.ToString();
        }

        public string Next()
        {
            Array values = Enum.GetValues(typeof(StatusLevel));

            int currentIndex = Array.IndexOf(values, _orderStatus);
            int nextIndex = (currentIndex + 1) % values.Length;

            _orderStatus = (StatusLevel)values.GetValue(nextIndex);
            return _orderStatus.ToString();
        }
    }
}