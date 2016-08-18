using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using restep.Framework.Logging;

namespace restep.Graphics
{
    /// <summary>
    /// Window which will display all GL rendered components
    /// <para>This class is a singleton, see <see cref="Instance"/> and <see cref="Initialize(int, int, string, GraphicsContextFlags)"/></para>
    /// </summary>
    internal class RestepWindow : GameWindow
    {
        public static bool IsInitialized
        {
            get;
            private set;
        } = false;

        private static RestepWindow instance;
        /// <summary>
        /// Singleton instance of this class
        /// Initialize must be called before trying to access this
        /// </summary>
        public static RestepWindow Instance
        {
            get
            {
                if(IsInitialized)
                {
                    return instance;
                }
                //TODO: log me
                throw new Exception("Restep window has not been initialized yet!");
            }
        }


        private RestepWindow(int width, int height, string title, GraphicsContextFlags gcf)
            : base(width, height, new GraphicsMode(new ColorFormat(8, 8, 8, 8), 16), title, 0, DisplayDevice.Default, 3, 1, GraphicsContextFlags.Default)
        {
            //TODO: move .log files to InfoLog folder
            MessageLogger.OpenLog("restepwinlog", "restep_window.log", true, "To log information about the main restep window");
            string version = GL.GetString(StringName.Version);
            //TODO: log me
            if(!version.StartsWith("3.1"))
            {
                throw new Exception("Was not able to get requested GL version!");
            }
            LoadBaseShader();
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
            IsInitialized = true;
        }

        #region ~base shader code~
        //cut down on CPU mat calculations by making mat3
        private const string BASE_SHADER_VTX =
            @"#version 330
              precision highp float;
              layout (location = 0) in vec2 position;
              layout (location = 1) in vec2 texCoord;
              
              out vec2 tcoord0;

              uniform mat3 transform;
              
              void main()
              {
                  tcoord0 = texCoord;
                  vec3 posResult = transform * vec3(position, 1.0);
                  gl_Position = vec4(posResult.xy, 0.0, 1.0);
              }";


        private const string BASE_SHADER_FRAG =
            @"#version 330
              precision highp float;
              out vec4 fragColor;

              uniform sampler2D diffuse;
              uniform int useTexture;
              uniform vec4 uColor;
              
              in vec2 tcoord0;

              void main()
              {
                  if(useTexture != 0)
                  {
                      fragColor = texture2D(diffuse, tcoord0);
                  }
                  else
                  {
                      fragColor = uColor;
                  }
              }";

        private void LoadBaseShader()
        {
            try
            {
                Shader baseShader = new Shader("baseShader");

                //SURROUND ME WITH TRYCATCH LATER
                baseShader.LoadShader(BASE_SHADER_VTX, BASE_SHADER_FRAG);

                baseShader.AddUniform("transform");
                baseShader.AddUniform("useTexture");
                baseShader.AddUniform("uColor");

                baseShader.Enabled = true;

                Framework.RestepGlobals.LoadedShaders.Add(baseShader);
                MessageLogger.LogMessage("restepwinlog", "ShaderStatus", MessageType.Success, "Loaded, compiled, and linked base shader successfully. All uniforms found.", true);
            }
            catch(Exception e)
            {
                MessageLogger.LogMessage("restepwinlog", "ShaderStatus", MessageType.Error, e.Message, true);
            }
        }

        #endregion

        /// <summary>
        /// Override of OpenTK function for rendering of frame
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            
            base.OnRenderFrame(e);
        }
    }
}