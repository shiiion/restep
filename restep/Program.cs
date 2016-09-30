using System;
using System.Runtime.InteropServices;
using OpenTK;
using restep.Graphics;
using restep.Framework.Logging;
using restep.Graphics.Renderables;
using restep.Graphics.Intermediate;

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
            ConvexPolygon cvp = new ConvexPolygon(new VertexData(new ConvexVertexFormat(),
@"v_0.5_1_0
v_0_0.5_0.5
v_1_0.5_0.5
v_0.45_0_1
v_0.55_0_1
v_0.55_0.5_0.5
v_0.45_0_1
v_0.54_0_1
v_0.55_0.5_0.5"));
            TexturedQuad receptor = new TexturedQuad(TestResources.TestReceptor);
            TexturedQuad arrow = new TexturedQuad(TestResources.TestArrow);

            cvp.Transformation.Scale = new Vector2(64, 64);
            receptor.Transformation.Scale = new Vector2(64, 64);
            arrow.Transformation.Scale = new Vector2(64, 64);

            cvp.Transformation.Translation = new Vector2(64, 64);
            receptor.Transformation.Translation = new Vector2(400, 100);
            arrow.Transformation.Translation = new Vector2(400, 400);

            receptor.Transformation.Rotation = -(float)Math.PI / 2;
            arrow.Transformation.Rotation = -(float)Math.PI / 2;

            //depth ordering, first = top
            //TODO: use Depth property to order meshes
            RestepRenderer.Instance.RenderedObjects.Add(arrow);
            RestepRenderer.Instance.RenderedObjects.Add(receptor);
            RestepRenderer.Instance.RenderedObjects.Add(cvp);

            receptor.Origin = new Vector2(0.5f, 0.5f);
            arrow.Origin = new Vector2(0.5f, 0.5f);
            

            bool leftPressed = false;

            RestepWindow.Instance.UpdateFrame += (o, e) =>
            {
                arrow.Transformation.Translation = new Vector2(arrow.Transformation.Translation.X, arrow.Transformation.Translation.Y - 6);
                if (arrow.Transformation.Translation.Y < 0)
                {
                    Console.WriteLine("awful! stop playing please!");
                    arrow.Transformation.Translation = new Vector2(400, 400);
                }

                if (GetAsyncKeyState(0x25) != 0)
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

            RestepWindow.Instance.Run(60, 60);
        }

        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(int key);
    }
}
