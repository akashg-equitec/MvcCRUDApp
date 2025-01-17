using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace CrudApp.Models
{
    public static class ErrorLogger
    {
        private static readonly string LogFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ErrorLogs.txt");

        public static void LogError(Exception ex)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(LogFilePath, true))
                {
                    writer.WriteLine($"Time: {DateTime.Now}");
                    writer.WriteLine($"Message: {ex.Message}");
                    writer.WriteLine($"StackTrace: {ex.StackTrace}");
                    if (ex.InnerException != null)
                    {
                        writer.WriteLine($"InnerException: {ex.InnerException.Message}");
                    }
                    writer.WriteLine(new string('-', 50));
                }
            }
            catch (Exception loggingEx)
            {
                // Handle potential errors when writing to the log file (fallback to other mechanisms if needed)
            }
        }
    }

}