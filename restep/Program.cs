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
            RenderObject test1 = new RenderObject(new Vector2(100, 100));
            RenderObject test2 = new RenderObject(new Vector2(100, 100));
            RenderObject test3 = new RenderObject(new Vector2(100, 100));
            RenderObject test4 = new RenderObject(new Vector2(100, 100));
            RenderObject test5 = new RenderObject(new Vector2(100, 100));
            

            test1.Position = new Vector2(200, 400);
            test2.Position = new Vector2(300, 400);
            test3.Position = new Vector2(400, 400);
            test4.Position = new Vector2(500, 400);
            test5.Position = new Vector2(600, 400);
            test1.Overlap += (b) =>
            {
                //Console.Write("overlap");
            };

            test1.AddOBBCollider(new Vector2(50, 50));
            test2.AddOBBCollider(new Vector2(50, 50));
            test3.AddOBBCollider(new Vector2(50, 50));
            test4.AddOBBCollider(new Vector2(50, 50));
            test5.AddOBBCollider(new Vector2(50, 50));

            MessageLogger.InitializeRestepLogs();

            RestepWindow.Initialize(800, 800, "test");
            RestepRenderer.Initialize();
            

            CoreThread.Instance.AddObject(test1);
            CoreThread.Instance.AddObject(test2);
            CoreThread.Instance.AddObject(test3);
            CoreThread.Instance.AddObject(test4);
            CoreThread.Instance.AddObject(test5);

            TexturedQuad mesh1 = new TexturedQuad("c:/users/shion/documents/UP.png");
            TexturedQuad mesh2 = new TexturedQuad("c:/users/shion/documents/UP.png");
            TexturedQuad mesh3 = new TexturedQuad("c:/users/shion/documents/UP.png");
            TexturedQuad mesh4 = new TexturedQuad("c:/users/shion/documents/UP.png");
            TexturedQuad mesh5 = new TexturedQuad("c:/users/shion/documents/UP.png");

            mesh1.Depth = 0;
            mesh2.Depth = 1;

            RenderInterface.AddPair(mesh1, test1);
            RenderInterface.AddPair(mesh2, test2);
            RenderInterface.AddPair(mesh3, test3);
            RenderInterface.AddPair(mesh4, test4);
            RenderInterface.AddPair(mesh5, test5);

            long time = System.DateTime.Now.Ticks;

            Input.InputManager.AnyKeyHold += (k) =>
            {
                Console.WriteLine(System.DateTime.Now.Ticks - time);
                time = System.DateTime.Now.Ticks;
                switch (k)
                {
                    case OpenTK.Input.Key.Left:
                        test1.Position += new Vector2(-1, 0);
                        break;
                    case OpenTK.Input.Key.Right:
                        test1.Position += new Vector2(1, 0);
                        break;
                    case OpenTK.Input.Key.Up:
                        test1.Position += new Vector2(0, 1);
                        break;
                    case OpenTK.Input.Key.Down:
                        test1.Position += new Vector2(0, -1);
                        break;
                    case OpenTK.Input.Key.End:
                        test1.Rotation += 0.01f;
                        test2.Rotation += 0.04f;
                        test3.Rotation += 0.09f;
                        test4.Rotation += 0.16f;
                        test5.Rotation += 0.25f;
                        break;
                }
            };

            RestepWindow.Instance.Run(60, 60);
        }

    }


}
