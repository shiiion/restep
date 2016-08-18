using System;
using restep.Graphics;
using restep.Framework.Logging;

namespace restep
{
    class Program
    {
        static void Main(string[] args)
        {
            //RestepWindow rw = new RestepWindow(500, 500, "whee");
            //TEMPORARY 60 UPS 60 FPS
            //rw.Run(60, 60);
            MessageLogger.OpenLog("test", "test.log", true, "testing logging");
            MessageLogger.LogMessage("test", "LoggerTest", MessageType.Notify, "Test output", true);
            MessageLogger.CloseLog("test");
        }
    }
}
