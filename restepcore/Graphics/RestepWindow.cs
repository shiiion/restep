using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;


namespace restep.Graphics
{
    internal class RestepWindow : GameWindow
    {
        private const string BASE_SHADER_VTX = 
            @"#version 330
              precision highp float;
              layout (location = 0) in vec2 position;
              layout (location = 1) in vec2 texCoord;
              
              out vec2 tcoord0;

              uniform mat4 transform;
              
              void main()
              {
                  tcoord0 = texCoord;
                  gl_Position = transform * vec4(position, 0.0, 1.0);
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

        public RestepWindow(int width, int height, string title, GraphicsContextFlags gcf = GraphicsContextFlags.Default)
            : base(width, height, new GraphicsMode(new ColorFormat(8, 8, 8, 8), 16), title, 0, DisplayDevice.Default, 3, 1, GraphicsContextFlags.Default)
        {
            string version = GL.GetString(StringName.Version);
            if(!version.StartsWith("3.1"))
            {
                throw new Exception("Was not able to get requested GL version!");
            }
            LoadBaseShader();
        }

        private void LoadBaseShader()
        {
            Shader baseShader = new Shader();
            
            //SURROUND ME WITH TRYCATCH LATER
            baseShader.LoadShader(BASE_SHADER_VTX, BASE_SHADER_FRAG);

            baseShader.AddUniform("transform");
            baseShader.AddUniform("useTexture");
            baseShader.AddUniform("uColor");

            baseShader.Enabled = true;

            Framework.RestepGlobals.LoadShader()
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            
            base.OnRenderFrame(e);
        }
    }
}