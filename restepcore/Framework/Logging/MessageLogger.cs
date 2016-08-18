using System;
using System.Collections.Generic;

namespace restep.Framework.Logging
{
    internal class MessageLogger
    {
        //the LogStream abstraction will exist for a purpose one day im sure
        private static Dictionary<string, LogStream> openedLogs = new Dictionary<string, LogStream>();
        

        public static LogFile OpenLog(string logAlias, string logPath, bool append, string purpose = "")
        {
            LogFile newLog = new LogFile(logAlias, logPath, append, purpose);
            openedLogs.Add(logAlias, newLog);

            return newLog;
        }

        public static void LogMessage(string logName, string alias, MessageType type, string message, bool timestamp)
        {
            LogStream value;
            if(openedLogs.TryGetValue(logName, out value))
            {
                value.LogMessage(alias, type, message, timestamp);
            }
        }

        public static void CloseLog(string logName)
        {
            LogStream value;
            if (openedLogs.TryGetValue(logName, out value))
            {
                value.Dispose();
                openedLogs.Remove(logName);
            }
        }
    }
}
