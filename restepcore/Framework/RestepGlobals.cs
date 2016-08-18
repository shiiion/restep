using System;
using System.Collections.Generic;
using System.Text;
using restep.Graphics;
using OpenTK;

namespace restep.Framework
{
    internal class RestepGlobals
    {
        public static List<Shader> LoadedShaders { get; private set; } = new List<Shader>();

        private static Vector2 contentAreaSize;
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
