using System;
using System.Collections.Generic;
using System.Text;
using restep.Graphics.Renderables;

namespace restep.Framework.ResourceManagement
{
    internal class TextureResourceServer
    {
        public static Texture ServeTexture(string path)
        {
            Texture existingTexture = ReferenceCounter.GetReference<Texture>(path);

            if(existingTexture == null)
            {
                existingTexture = new Texture(path);
                if()
            }
        }

        public static Texture ServeTexture(System.Drawing.Bitmap bmp)
        {
            Texture existingTexture = ReferenceCounter.GetReference<Texture>(bmp);
        }
    }
}
