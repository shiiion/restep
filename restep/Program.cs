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
            RenderObject test1 = new RenderObject(new Vector2(50, 50));
            RenderObject test2 = new RenderObject(new Vector2(50, 50));
            RenderObject test3 = new RenderObject(new Vector2(50, 50));
            RenderObject test4 = new RenderObject(new Vector2(50, 50));
            RenderObject test5 = new RenderObject(new Vector2(50, 50));
            

            test1.Position = new Vector2(350, 400);
            test2.Position = new Vector2(375, 400);
            test3.Position = new Vector2(400, 400);
            test4.Position = new Vector2(425, 400);
            test5.Position = new Vector2(450, 400);

            test1.AddOBBCollider(new Vector2(25, 25));
            test2.AddOBBCollider(new Vector2(25, 25));
            test3.AddOBBCollider(new Vector2(25, 25));
            test4.AddOBBCollider(new Vector2(25, 25));
            test5.AddOBBCollider(new Vector2(25, 25));

            MessageLogger.InitializeRestepLogs();

            RestepWindow.Initialize(800, 800, "test");
            RestepRenderer.Initialize();
            

            CoreThread.Instance.AddObject(test1);
            CoreThread.Instance.AddObject(test2);
            CoreThread.Instance.AddObject(test3);
            CoreThread.Instance.AddObject(test4);
            CoreThread.Instance.AddObject(test5);

            TexturedQuad mesh1 = new TexturedQuad("c:/users/shion/documents/up.png");
            TexturedQuad mesh2 = new TexturedQuad("c:/users/shion/documents/up.png");
            TexturedQuad mesh3 = new TexturedQuad("c:/users/shion/documents/up.png");
            TexturedQuad mesh4 = new TexturedQuad("c:/users/shion/documents/up.png");
            TexturedQuad mesh5 = new TexturedQuad("c:/users/shion/documents/up.png");

            mesh1.Depth = 0;
            mesh2.Depth = 1;

            RenderInterface.AddPair(mesh1, test1);
            RenderInterface.AddPair(mesh2, test2);
            RenderInterface.AddPair(mesh3, test3);
            RenderInterface.AddPair(mesh4, test4);
            RenderInterface.AddPair(mesh5, test5);


            CoreThread.Instance.Tick += (ft, objects) =>
            {
                //objects[0].Position = Input.InputManager.CursorLoc;
                Console.WriteLine(mesh1.Transformation.ScreenSpace);
            };

            RestepWindow.Instance.Run(60, 60);
        }

    }


}
