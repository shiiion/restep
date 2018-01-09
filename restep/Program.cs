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
using System.Threading.Tasks;
using System.Media;


namespace restep
{
    class Program
    {
        private static float ang(Vector2 a, Vector2 b)
        {
            //|a||b|cos theta = a*b

            return (float)Math.Acos(Vector2.Dot(a, b) / (a.Length * b.Length));
        }

        static void mainLoop()
        {
            TexturedQuad q = new TexturedQuad(TestResources.TestArrow);
            TexturedQuad q2 = new TexturedQuad(TestResources.TestReceptor);
            TexturedQuad q3 = new TexturedQuad("resource/circle.png");
            TexturedQuad q4 = new TexturedQuad("resource/line.png");

            RenderObject arrow1 = new RenderObject(new Vector2(50, 50));
            RenderObject arrow2 = new RenderObject(new Vector2(50, 50));
            RenderObject circle = new RenderObject(new Vector2(200, 200));
            RenderObject line = new RenderObject(new Vector2(200, 200));


            RenderInterface.AddPair(q, arrow1);
            RenderInterface.AddPair(q2, arrow2);
            RenderInterface.AddPair(q3, circle);
            RenderInterface.AddPair(q4, line);

            arrow1.Position = new Vector2(200, 200);
            arrow2.Position = new Vector2(400, 200);
            circle.Position = new Vector2(400, 400);
            line.Position = new Vector2(400, 510);
            line.Rotation = (float)Math.PI / 2.0f;

            CoreThread.Instance.Tick += (a, b) =>
            {
                if(Input.InputManager.GetMouseButtonState(OpenTK.Input.MouseButton.Left) == Input.ControlState.HOLD)
                {
                    Vector2 mouseLoc = Input.InputManager.CursorLoc;
                    Vector2 diff = mouseLoc - (new Vector2(400, 400));
                    float angle = -ang(diff, new Vector2(1, 0));
                    if(diff.Y < 0.0f)
                    {
                        angle = ((float)Math.PI * 2.0f) - angle;
                    }

                    Vector2 diffNorm = diff.Normalized();

                    line.Position = new Vector2(400, 400) + 110 * diffNorm;
                    line.Rotation = angle;
                }
            };



            RestepWindow.Instance.Run(60, 60);
        }

        static void Main(string[] args)
        {
            MessageLogger.InitializeRestepLogs();

            RestepWindow.Initialize(800, 800, "Test Run");
            RestepRenderer.Initialize();
            mainLoop();
        }

    }


}
