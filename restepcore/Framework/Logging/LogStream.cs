using System;
using System.IO;

namespace restep.Framework.Logging
{
    /// <summary>
    /// Represents a type of log message
    /// </summary>
    public enum MessageType
    {
        ///<summary>Message describes successful execution of important task</summary>
        Success,

        ///<summary>Message provides information useful for debug purposes such as the state of something</summary>
        Notify,

        ///<summary>Message provides warnings which could signal possible issues</summary>
        Warning,

        ///<summary>Message describes error which forced execution to change path</summary>
        Error,
    }

    /// <summary>
    /// Abstraction for logging streams (can be used for file, message box, console, ingame UI, etc)
    /// All initialization should be done in c-tor
    /// </summary>
    internal abstract class LogStream : IDisposable
    {
        /// <summary>
        /// Has log been loaded
        /// </summary>
        public bool Loaded
        {
            get; protected set;
        }

        /// <summary>
        /// Log's name to be identified in MessageLogger
        /// </summary>
        public string Name
        {
            get; protected set;
        }

        public LogStream(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Writes a formatted log string to the stream output
        /// </summary>
        /// <param name="alias">A unique identifier to display what code alerted this log entry</param>
        /// <param name="type">Type of message/severity of the message</param>
        /// <param name="message">Message to be displayed</param>
        /// <param name="timestamped">Whether or not to add the timestamp to the log entry</param>
        public abstract void LogMessage(string alias, MessageType type, string message, bool timestamped);

        public abstract void Dispose();
    }

    /// <summary>
    /// Logging stream for files
    /// </summary>
    internal sealed class LogFile : LogStream
    {
        private string logFilePath;

        /// <summary>
        /// Max size of logfile before contents are cleaned (after successive opening of said log)
        /// default value 10 MB
        /// </summary>
        public static long LogfileMaxSize = 10000000;

        public LogFile(string name, string path, bool append, string purpose = "")
            : base(name)
        {
            logFilePath = path;
            bool writeHeader = false;
            bool fileExists;

            if (!(fileExists = File.Exists(path)) || !append)
            {
                writeHeader = true;
            }

            //file too large? overwrite all contents
            if (fileExists && !checkFileSize(path))
            {
                writeHeader = true;
                append = false;
            }

            StreamWriter fileWriter = new StreamWriter(path, append);

            Loaded = true;

            if (writeHeader)
            {
                fileWriter.WriteLine($"{@"--//Restep Logfile\\--"}\r\nPurpose: {purpose}\r\nLogfile name : {Name}\r\n");
            }
            fileWriter.Close();

            logStatusMessage("Beginning log session");
        }

        private bool attemptStreamReopen(out StreamWriter streamOut)
        {
            try
            {
                streamOut = new StreamWriter(logFilePath, true);
                return true;
            }
            catch
            {
                streamOut = null;
                return false;
            }
        }

        //async only for fileIO
        public async override void LogMessage(string alias, MessageType type, string message, bool timestamped)
        {
            StreamWriter fileWriter;
            Loaded = attemptStreamReopen(out fileWriter);

            if (Loaded)
            {
                string timeString = (timestamped ? $" @ {DateTime.Now.ToString("HH:mm:ss.fff")}" : "");

                await fileWriter.WriteLineAsync($"[{type.ToString()} - {alias}{timeString}] : {message}");
                fileWriter.Close();
            }
        }

        private void logStatusMessage(string message)
        {
            LogMessage("LogStatus", MessageType.Notify, message, true);
        }

        public override void Dispose()
        {
            if (Loaded)
            {
                logStatusMessage("Ending log session\r\n");
                Loaded = false;
            }
        }

        //true = good size, false = too large
        private bool checkFileSize(string path)
        {
            return (new FileInfo(path).Length) < LogfileMaxSize;
        }
    }
}
