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
            LoadBaseShader();
        }

        #region ~base shader code~
        //cut down on CPU mat calculations by making mat3
        private const string TEX_SHADER_VTX =
            @"#version 330
              precision highp float;
              layout (location = 0) in vec2 position;
              layout (location = 1) in vec2 texCoord;
              
              out vec2 tcoord0;

              uniform mat3 transform;
              uniform vec2 origin;
              uniform float depth;
              
              void main()
              {
                  tcoord0 = texCoord;
                  vec3 positionTransform = transform * vec3((position - origin), 1.0);
                  gl_Position = vec4(positionTransform.xy, 0.0, 1.0);
              }";


        private const string TEX_SHADER_FRAG =
            @"#version 330
              precision highp float;
              out vec4 fragColor;

              uniform sampler2D diffuse;
              
              in vec2 tcoord0;

              void main()
              {
                  fragColor = texture2D(diffuse, tcoord0);
              }";

        private void LoadBaseShader()
        {
            try
            {
                Shader textureShader = new Shader("textureShader");
                //Shader colorShader = new Shader("colorShader");

                //SURROUND ME WITH TRYCATCH LATER
                textureShader.LoadShader(TEX_SHADER_VTX, TEX_SHADER_FRAG);

                textureShader.AddUniform("transform");
                textureShader.AddUniform("origin");

                textureShader.Enabled = true;

                Framework.RestepGlobals.LoadedShaders.Add(textureShader);
                MessageLogger.LogMessage(MessageLogger.RENDER_LOG, "ShaderStatus", MessageType.Success, "Loaded, compiled, and linked texture shader successfully. All uniforms found.", true);
            }
            catch
            {
                //TODO: handle me (popup?)
            }
        }

        #endregion

        /// <summary>
        /// Override of OpenTK function for rendering of frame
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            RestepRenderer.Instance.OnRender();
            SwapBuffers();

            //System.Console.WriteLine(e.Time); // Get framerate for performance logging

            base.OnRenderFrame(e);
        }
    }
}