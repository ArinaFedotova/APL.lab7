using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Exception = System.Exception;

namespace Logger
{
    public static class ReportLog
    {
        private static object ReportLock = new object();
        public static string filePath = "/Users/arinafedotova/RiderProjects/Restaurant/Modeling/Report.csv";
        public static DateTime Begin;
        public static DateTime End;
        public static DateTime LastOrderProcessed;
        public static TimeSpan AverageProcessOrderTime;
        public static List<int> CountOfStaff;
        public static int amountOfOrders;
        public static long Memory;
        
        public static void ReportCreate(List<int> StaffCount, int numOfOrders)
        {
            CountOfStaff = StaffCount;
            amountOfOrders = numOfOrders;

            string[] headers = new[] { "Waiter", "Cook", "Chief", "Service time", "Average time for order", "Memory" };
            using (StreamWriter writer = new StreamWriter(filePath))
                writer.WriteLine(string.Join(",", headers));
        }

        public static void logToFileData()
        {
            CalculateAverage();
            var row = new List<string>
            {
                End.Subtract(Begin).ToString(), AverageProcessOrderTime.ToString(),
                CountOfStaff[0].ToString(), CountOfStaff[1].ToString(), CountOfStaff[2].ToString(),
                Memory.ToString()
            };
            
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine(string.Join(",", row));
            }
        }
        
        static void CalculateAverage()
        {
            if (amountOfOrders == 0)
                AverageProcessOrderTime = TimeSpan.Zero;
            
            TimeSpan totalTime = LastOrderProcessed.Subtract(Begin);
            AverageProcessOrderTime = TimeSpan.FromTicks(totalTime.Ticks / amountOfOrders);
        }
        
        public static object GetLock()
        {
            return ReportLock;
        }
    }
}