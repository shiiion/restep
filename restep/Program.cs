using System;
using System.Runtime.InteropServices;
using OpenTK;
using restep.Graphics;
using restep.Framework.Logging;
using restep.Graphics.Renderables;
using restep.Graphics.Intermediate;
using restep.Core.Collision;
using restep.Core;
using restep.Interface.Render;
using System.Threading;


namespace restep
{
    class Program
    {
        static void Main(string[] args)
        {
            RenderObject testOne = new RenderObject(new Vector2(50, 50));
            RenderObject testTwo = new RenderObject(new Vector2(50, 50));

            testTwo.Rotation = 0;

            testOne.AddOBBCollider(new Vector2(25, 25));
            testTwo.AddOBBCollider(new Vector2(25, 25));

            testOne.Position = new Vector2(50, 200);
            testTwo.Position = new Vector2(50, 200);

            MessageLogger.InitializeRestepLogs();

            RestepWindow.Initialize(800, 800, "test");
            RestepRenderer.Initialize();

            CoreThread.Instance.AddObject(testOne);
            CoreThread.Instance.AddObject(testTwo);

            TexturedQuad mesh1 = new TexturedQuad("c:/users/shion/documents/up.png");
            TexturedQuad mesh2 = new TexturedQuad("c:/users/shion/documents/up.png");

            mesh1.Depth = 0;
            mesh2.Depth = 1;

            RenderInterface.AddPair(mesh1, testOne);
            RenderInterface.AddPair(mesh2, testTwo);

            Input.InputManager.AnyKeyPress += (k) =>
            {
                Console.Write(k.ToString());
            };

            CoreThread.Instance.Tick += (ft, objects) =>
            {
                objects[0].Position = new Vector2(800, 400);
            };

            RestepWindow.Instance.Run(60, 60);
        }

    }


}
