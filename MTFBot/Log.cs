using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTFBot
{
    internal static class Log
    {
        private static string CurrentSessionFile;

        public enum LogLevel
        {
            SUCCES,
            FAIL,
            INFO,
            WARNING,
            FATAL,
            ERROR
        }

        public static void WriteLine(string message, LogLevel level = LogLevel.INFO)
        {
            if (CurrentSessionFile == null)
                CurrentSessionFile = $"Logs/{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.log";

            if (!Directory.Exists("Logs"))
                Directory.CreateDirectory("Logs");

            switch (level)
            {
                case LogLevel.INFO:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogLevel.ERROR:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;
                case LogLevel.FAIL:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogLevel.FATAL:
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    break;
                case LogLevel.SUCCES:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case LogLevel.WARNING:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
            }

            string line = $"[{DateTime.Now:yyyy.MM.dd HH:mm:ss}] [{level.ToString()}] {message}\n";
            Console.Write(line);

            using (FileStream stream = new FileStream(CurrentSessionFile, FileMode.Append))
            {
                stream.Write(Encoding.UTF8.GetBytes(line), 0, Encoding.UTF8.GetBytes(line).Length);
            }

            Console.ResetColor();
        }
    }
}
