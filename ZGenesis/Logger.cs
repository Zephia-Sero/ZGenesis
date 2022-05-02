using System;
using System.IO;
using HarmonyLib;

namespace ZGenesis {
    public static class Logger {
        public enum LogLevel {
            ESSENTIAL,
            DEBUG,
            INFO,
            WARNING,
            ERROR,
            FATAL
        }
        private static StreamWriter logSW;
        private static StreamWriter latestlogSW;
        private static readonly string date = null;
        static Logger() {
            date = DateTime.Now.ToString("_yyyy-MM-dd_HH-mm-ss");
            FileLog.logPath = "logs/harmony.log";
            Open(FileMode.Create);
        }
        public static void Log(string modname, string message, params object[] args) {
            Log(LogLevel.INFO, modname, message, args);
        }
        public static void Log(LogLevel level, string modname, string message, params object[] args) {
            string time = DateTime.Now.ToString("HH:mm:ss+fff");
            logSW.WriteLine(
                $"{time} <{level}> " + new string(' ',9-level.ToString().Length) + $"[{modname}] {string.Format(message,args)}"
            );
            latestlogSW.WriteLine(
                $"{time} <{level}> " + new string(' ', 9 - level.ToString().Length) + $"[{modname}] {string.Format(message, args)}"
            );
            Reload();
        }
        public static void Flush() {
            logSW.Flush();
            latestlogSW.Flush();
        }
        public static void Reload() {
            Flush();
            Close();
            Open(FileMode.Append);
        }
        public static void Close() {
            logSW.Close();
            latestlogSW.Close();
        }
        public static void Open(FileMode fm) {
            FileStream fs = File.Open("logs/zgenesis" + date + ".log",fm);
            FileStream fs2 = File.Open("logs/zgenesis_latest.log",fm);
            logSW = new StreamWriter(fs);
            latestlogSW = new StreamWriter(fs2);
        }
    }
}