using System;
using restep.Framework.Logging;

namespace restep.Framework.Exceptions
{
    class LoggedException : Exception
    {
        public LoggedException(string errMsg, string logName, string alias) 
            : base (errMsg)
        {
            MessageLogger.LogMessage(logName, alias, MessageType.Error, errMsg, true);
        }
    }
}
