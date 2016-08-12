using System;
using restep.Graphics;

namespace restep
{
    class Program
    {
        static void Main(string[] args)
        {
            RestepWindow rw = new RestepWindow(500, 500, "whee");
            //TEMPORARY 60 UPS 60 FPS
            rw.Run(60, 60);
        }
    }
}
