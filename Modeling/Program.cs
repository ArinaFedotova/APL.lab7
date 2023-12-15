using System.Collections.Generic;
using System.Diagnostics;
using Restaurant;
using Restaurant.Templates;

namespace Modeling
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            List<string> orders = new List<string>{ "Soup", "Steak", "Salad", "Fish", "Cake", "Coctail", "Lemonade" };
            List<int> amountOfWorkers = new List<int>{2, 2, 2};
            Logger.ReportLog.ReportCreate(amountOfWorkers, orders.Count);
            Service service = new Service(orders, amountOfWorkers);
            Logger.ReportLog.Memory = Process.GetCurrentProcess().PeakWorkingSet64;
            service.Start();
        }
    }
}