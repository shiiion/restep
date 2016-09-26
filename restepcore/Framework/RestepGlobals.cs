using System.Collections.Generic;
using OpenTK;
using restep.Graphics;
using restep.Graphics.Shaders;

namespace restep.Framework
{
    /// <summary>
    /// Stores global runtime data for this program
    /// </summary>
    internal class RestepGlobals
    {
        //TODO: move this elsewhere
        #region ~base shader code~
        //cut down on CPU mat calculations by making mat3
        public static readonly string TEX_SHADER_VTX =
            @"#version 330
              precision highp float;
              layout (location = 0) in vec2 position;
              layout (location = 1) in vec2 texCoord;
              
              out vec2 tcoord0;

              uniform mat3 transform;
              uniform vec2 origin;
              
              void main()
              {
                  tcoord0 = texCoord;
                  vec3 positionTransform = transform * vec3((position - origin), 1.0);
                  gl_Position = vec4(positionTransform.xy, 0.0, 1.0);
              }";

        public static readonly string TEX_SHADER_FRAG =
            @"#version 330
              precision highp float;
              out vec4 fragColor;

              uniform sampler2D diffuse;
              
              in vec2 tcoord0;

              void main()
              {
                  fragColor = texture2D(diffuse, tcoord0);
              }";

        public static readonly string COLOR_SHADER_VTX =
            @"#version 330
              precision highp float;
              layout (location = 0) in vec2 position;
              layout (location = 1) in float gradient;

              uniform mat3 transform;
              uniform vec2 origin;

              out float grad;

              void main()
              {
                  grad = gradient;
                  vec3 positionTransform = transform * vec3((position - origin), 1.0);
                  gl_Position = vec4(positionTransform.xy, 0.0, 1.0);
              }";

        public static readonly string COLOR_SHADER_FRAG =
            @"#version 330
              precision highp float;
              out vec4 fragColor;

              uniform vec4 colorA;
              uniform vec4 colorB;
              uniform float gradInc;

              in float grad;

              void main()
              {
                  if(fract(grad + gradInc) > 0.5)
                  {
                      fragColor = mix(colorB, colorA, 2 * (fract(grad + gradInc) - 0.5));
                  }
                  else
                  {
                      fragColor = mix(colorA, colorB, 2 * fract(grad + gradInc));
                  }
              }";//              uniform vec4 colorA;mix(colorA, colorB, grdAt)
        //vec3 positionTransform = transform * vec3((position - origin), 1.0);

        #endregion

        /// <summary>
        /// List of shaders which will be used by all meshes (if enabled)
        /// </summary>
        public static List<Shader> LoadedShaders { get; private set; } = new List<Shader>();

        private static Vector2 contentAreaSize;
        /// <summary>
        /// Size of the Content Area of the RestepWindow
        /// <para>Changing this value auto updates the RestepWindow's client size</para>
        /// </summary>
        public static Vector2 ContentAreaSize
        {
            get
            {
                return contentAreaSize;
            }

            set
            {
                contentAreaSize = value;
                if(RestepWindow.Initialized)
                {
                    RestepWindow.Instance.ClientSize = new System.Drawing.Size((int)contentAreaSize.X, (int)contentAreaSize.Y);
                }
            }
        }
    }
}
