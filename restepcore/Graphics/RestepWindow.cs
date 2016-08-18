using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using restep.Framework.Logging;

namespace restep.Graphics
{
    internal class RestepWindow : GameWindow
    {
        public static bool IsInitialized
        {
            get;
            private set;
        } = false;

        private static RestepWindow instance;
        public static RestepWindow Instance
        {
            get
            {
                if(IsInitialized)
                {
                    return instance;
                }
                throw new Exception("Restep window has not been initialized yet!");
            }
        }


        private RestepWindow(int width, int height, string title, GraphicsContextFlags gcf)
            : base(width, height, new GraphicsMode(new ColorFormat(8, 8, 8, 8), 16), title, 0, DisplayDevice.Default, 3, 1, GraphicsContextFlags.Default)
        {
            MessageLogger.OpenLog("restepwinlog", "restep_window.log", true, "To log information about the main restep window");
            string version = GL.GetString(StringName.Version);
            if(!version.StartsWith("3.1"))
            {
                throw new Exception("Was not able to get requested GL version!");
            }
            LoadBaseShader();
        }

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

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            
            base.OnRenderFrame(e);
        }
    }
}