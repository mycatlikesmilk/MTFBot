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
        private static StreamWriter writer;

        public enum LogLevel
        {
            SUCCES,
            FAIL,
            INFO,
            WARNING,
            FATAL,
            ERROR
        }

        public static void StopLog()
        {
            if (writer != null)
                writer.Close();
        }

        public static void WriteLine(string message, LogLevel level = LogLevel.INFO)
        {
            if (CurrentSessionFile == null)
                CurrentSessionFile = $"Logs/{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.txt";

            if (!Directory.Exists("Logs"))
                Directory.CreateDirectory("Logs");

            if (writer == null)
            {
                writer = new StreamWriter(CurrentSessionFile);
            }

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
            Console.ResetColor();
            writer.Write(line);
        }
    }
}
