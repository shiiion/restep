using System;
using System.Collections.Generic;
using System.Text;
using restep.Graphics.Renderables;
using restep.Graphics.Shaders;

namespace restep.Framework.ResourceManagement
{
    internal class TextureResourceServer
    {
        public static Texture ServeTexture(string path)
        {
            Texture servedTexture = ReferenceCounter.GetReference<Texture>(ResourceHash.CreateHash(path));

            if(servedTexture == null)
            {
                servedTexture = new Texture(path);
                if(!servedTexture.Hashed || !servedTexture.Loaded)
                {
                    servedTexture.Dispose();
                    return null;
                }
                ReferenceCounter.AddNewReference(servedTexture);
            }
            else
            {
                servedTexture.RefCount++;
            }

            return servedTexture;
        }

        public static Texture ServeTexture(System.Drawing.Bitmap bmp)
        {
            Texture servedTexture = ReferenceCounter.GetReference<Texture>(ResourceHash.CreateHash(bmp));

            if (servedTexture == null)
            {
                servedTexture = new Texture(bmp);
                if (!servedTexture.Hashed || !servedTexture.Loaded)
                {
                    servedTexture.Dispose();
                    return null;
                }
                ReferenceCounter.AddNewReference(servedTexture);
            }
            else
            {
                servedTexture.RefCount++;
            }

            return servedTexture;
        }
    }
}
