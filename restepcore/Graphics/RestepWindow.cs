using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using restep.Framework.Logging;
using restep.Framework.Exceptions;
using restep.Graphics.Shaders;

namespace restep.Graphics
{
    /// <summary>
    /// Window which will display all GL rendered components
    /// <para>This class is a singleton, see <see cref="Instance"/> and <see cref="Initialize(int, int, string, GraphicsContextFlags)"/></para>
    /// </summary>
    internal class RestepWindow : GameWindow
    {
        #region ~singleton data~
        public static bool Initialized { get; private set; } = false;

        private static RestepWindow instance;
        /// <summary>
        /// Singleton instance of this class
        /// <para>Initialize must be called before trying to access this</para>
        /// </summary>
        public static RestepWindow Instance
        {
            get
            {
                if(Initialized)
                {
                    return instance;
                }
                throw new LoggedException("Failed to get instance as window has not been initialized yet!", MessageLogger.RENDER_LOG, "RestepWindow");
            }
        }

        /// <summary>
        /// Initialize the static instance of RestepWindow
        /// </summary>
        /// <param name="width">(client area?)width of the window</param>
        /// <param name="height">(client area?)height of the window</param>
        /// <param name="title">Title of the window</param>
        /// <param name="gcf">GL Graphics context flags</param>
        public static void Initialize(int width, int height, string title, GraphicsContextFlags gcf = GraphicsContextFlags.Default)
        {
            instance = new RestepWindow(width, height, title, gcf);
            Initialized = true;
        }
        #endregion

        private RestepWindow(int width, int height, string title, GraphicsContextFlags gcf)
            : base(width, height, new GraphicsMode(new ColorFormat(8, 8, 8, 8), 16), title, 0, DisplayDevice.Default, 3, 1, GraphicsContextFlags.Default)
        {
            string version = GL.GetString(StringName.Version);

            if(!version.StartsWith("3.1"))
            {
                throw new LoggedException("Was not able to get requested GL version!", MessageLogger.RENDER_LOG, "RestepWindow");
            }

            Framework.RestepGlobals.ContentAreaSize = new Vector2(width, height);
            LoadBaseShaders();
        }

        private void LoadBaseShaders()
        {
            try
            {
                Renderables.TexturedQuad.InitClass();
                Renderables.ConvexPolygon.InitClass();
            }
            catch
            {
                //TODO: handle me (popup?)
            }
        }

        /// <summary>
        /// Override of OpenTK function for rendering of frame
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRenderFrame(FrameEventArgs e)
        {

            System.Console.WriteLine(e.Time); // Get framerate for performance logging

            base.OnRenderFrame(e);
            RestepRenderer.Instance.OnRender();
            SwapBuffers();
        }
    }
}