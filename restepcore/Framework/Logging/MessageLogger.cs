using System;
using System.Collections.Generic;

namespace restep.Framework.Logging
{
    /// <summary>
    /// Manages a list of opened LogStreams
    /// </summary>
    internal class MessageLogger
    {
        public static readonly string RENDER_LOG = "renderlog";

        public static void InitializeRestepLogs()
        {
            //TODO: move .log files to InfoLog folder
            //TODO: add more logs
            OpenLog(RENDER_LOG, "restep_rendering.log", true, "To log information about the rendering status of restep");
        }

        //the LogStream abstraction will exist for a purpose one day im sure
        private static Dictionary<string, LogStream> openedLogs = new Dictionary<string, LogStream>();

        /// <summary>
        /// Creates a new instance of LogFile and maps logName to the instance
        /// </summary>
        /// <param name="logName">Name by which to refer to this LogStream</param>
        /// <param name="logPath">Path to the logfile, not required to exist</param>
        /// <param name="append">If false, contents of file are overwritten, otherwise all further log entries are appended to the end</param>
        /// <param name="purpose">Purpose of this logfile (placed at the top of the logfile)</param>
        /// <returns>The new LogFile instance</returns>
        public static LogFile OpenLog(string logName, string logPath, bool append, string purpose = "")
        {
            LogFile newLog = new LogFile(logName, logPath, append, purpose);
            openedLogs.Add(logName, newLog);

            return newLog;
        }

        /// <summary>
        /// See <seealso cref="LogStream.LogMessage(string, MessageType, string, bool)"/>
        /// </summary>
        /// <param name="logName">Logfile name to search for in log hashmap</param>
        public static void LogMessage(string logName, string alias, MessageType type, string message, bool timestamp)
        {
            LogStream value;
            if(openedLogs.TryGetValue(logName, out value))
            {
                value.LogMessage(alias, type, message, timestamp);
            }
        }

        /// <summary>
        /// Closes and removes all references to the LogStream mapped to by logName
        /// </summary>
        /// <param name="logName">Logfile name to search for in log hashmap</param>
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
