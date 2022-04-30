using System;
using System.IO;

namespace ZGenesis {
    public static class Logger {
        public enum LogLevel {
            ESSENTIAL,
            INFO,
            WARNING,
            ERROR,
            FATAL
        }
        private static readonly StreamWriter logSW;
        private static readonly StreamWriter latestlogSW;
        static Logger() {
            FileStream fs = File.OpenWrite("logs/zgenesis" + DateTime.Now.ToString("_yyyy-MM-dd_HH-mm-ss") + ".log");
            FileStream fs2 = File.OpenWrite("logs/zgenesis_latest.log");
            logSW = new StreamWriter(fs);
            latestlogSW = new StreamWriter(fs2);
        }
        public static void Log(string modname, string message, params object[] args) {
            Log(LogLevel.INFO, modname, message, args);
        }
        public static void Log(LogLevel level, string modname, string message, params object[] args) {
            string time = DateTime.Now.ToString("HH:mm:ss+fff");
            logSW.WriteLine(
                $"{time} <{level}> [{modname}] {string.Format(message,args)}"
            );
            latestlogSW.WriteLine(
                $"{time} <{level}> [{modname}] {string.Format(message, args)}"
            );
            logSW.Flush();
            latestlogSW.Flush();
        }
    }
}