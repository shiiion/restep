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

namespace restep
{
    class Program
    {
        static void Main(string[] args)
        {
            RenderObject testOne = new RenderObject(new Vector2(50, 50));
            RenderObject testTwo = new RenderObject(new Vector2(50, 50));

            testTwo.Rotation = 0;

            testOne.AddAABBCollider(new Vector2(25, 25));
            testTwo.AddCircleCollider(25);

            testOne.Position = new Vector2(50, 200);
            testTwo.Position = new Vector2(50, 200);

            MessageLogger.InitializeRestepLogs();

            RestepWindow.Initialize(800, 800, "test");
            RestepRenderer.Initialize();

            TexturedQuad mesh1 = new TexturedQuad("d:/jpeg/ff.png");
            TexturedQuad mesh2 = new TexturedQuad("d:/jpeg/circle.png");
            
            mesh1.Depth = 0;
            mesh2.Depth = 1;

            RenderInterface.AddPair(mesh1, testOne);
            RenderInterface.AddPair(mesh2, testTwo);

            RestepWindow.Instance.UpdateFrame += (o, e) =>
            {
                testOne.Position = testOne.Position + new Vector2(0.001f, 0.05f);
                if (!testOne.TestCollision(testTwo))
                {
                    Console.WriteLine(testOne.TestCollision(testTwo));
                }
            };

            RestepWindow.Instance.Run(60, 60);
        }

    }


}
