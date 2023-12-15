using System;
using System.IO;

namespace Logger
{
    public static class Logger
    {
        private static string filePath = "/Users/arinafedotova/RiderProjects/Restaurant/Modeling/log.txt";
        private static object loggerLock = new object();
        public static void Log(string message)
        {
            var formedMessage = $"{DateTime.Now:HH:mm:ss} - {message}";
            Console.WriteLine(formedMessage);
            LogToFile(formedMessage);
        }

        private static void LogToFile(string message)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"PATH {filePath}");
                    throw new Exception("File does not exist");
            }
            if (Path.GetExtension(filePath) != ".txt")
                throw new Exception("Not TXT file");
                
            using (StreamWriter writer = new StreamWriter(filePath, true)) 
            { 
                writer.WriteLine(message);
            }
        }

        public static object GetLogLock()
        {
            return loggerLock;
        }
    }
}