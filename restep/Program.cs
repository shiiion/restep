using System;
using System.Timers;
using System.Runtime.InteropServices;
using OpenTK;
using restep.Graphics;
using restep.Framework.Logging;
using restep.Graphics.Renderables;

namespace restep
{
    class Program
    {
        static void Main(string[] args)
        {
            MessageLogger.InitializeRestepLogs();

            RestepWindow.Initialize(800, 800, "test");
            RestepRenderer.Initialize();

            //TODO: autoscale TexturedQuads to their texture dims
            TexturedQuad receptor = new TexturedQuad(TestResources.TestReceptor);
            TexturedQuad arrow = new TexturedQuad(TestResources.TestArrow);

            receptor.Transformation.Scale = new Vector2(64, 64);
            arrow.Transformation.Scale = new Vector2(64, 64);

            receptor.Transformation.Translation = new Vector2(400, 100);
            arrow.Transformation.Translation = new Vector2(400, 400);

            receptor.Transformation.Rotation = -(float)Math.PI / 2;
            arrow.Transformation.Rotation = -(float)Math.PI / 2;

            //depth ordering, first = top
            //TODO: use Depth property to order meshes
            RestepRenderer.Instance.RenderedObjects.Add(arrow);
            RestepRenderer.Instance.RenderedObjects.Add(receptor);

            receptor.Transformation.Origin = new Vector2(0.5f, 0.5f);
            arrow.Transformation.Origin = new Vector2(0.5f, 0.5f);
            
            Timer t = new Timer(8);

            bool leftPressed = false;

            t.Elapsed += (o, e) =>
            {
                arrow.Transformation.Translation = new Vector2(arrow.Transformation.Translation.X, arrow.Transformation.Translation.Y - 2);
                if(arrow.Transformation.Translation.Y < 0)
                {
                    Console.WriteLine("awful! stop playing please!");
                    arrow.Transformation.Translation = new Vector2(400, 400);
                }

                if(GetAsyncKeyState(0x25) != 0)
                {
                    if (!leftPressed)
                    {
                        leftPressed = true;
                        Vector2 diff = Vector2.Subtract(arrow.Transformation.Translation, receptor.Transformation.Translation);
                        if (diff.Length < 30)
                        {
                            Console.WriteLine("good!");
                            arrow.Transformation.Translation = new Vector2(400, 400);
                        }
                        else
                        {
                            Console.WriteLine("bad!");
                        }
                    }
                }
                else
                {
                    leftPressed = false;
                }
            };
            t.Start();
            RestepWindow.Instance.Run(60, 60);
        }

        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(int key);
    }
}
