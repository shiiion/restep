using System;
using System.Collections.Generic;
using System.Text;
using restep.Graphics;
using OpenTK;

namespace restep.Framework
{
    /// <summary>
    /// Stores global runtime data for this program
    /// </summary>
    internal class RestepGlobals
    {
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
                if(RestepWindow.IsInitialized)
                {
                    RestepWindow.Instance.ClientSize = new System.Drawing.Size((int)contentAreaSize.X, (int)contentAreaSize.Y);
                }
            }
        }
    }
}
