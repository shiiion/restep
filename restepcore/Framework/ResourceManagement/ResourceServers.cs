using System;
using System.Collections.Generic;
using System.Text;
using restep.Graphics.Renderables;
using restep.Graphics.Shaders;

namespace restep.Framework.ResourceManagement
{
    internal class ShaderResourceServer
    {
        public static Shader ServeShader(string name, string path)
        {
            Shader servedShader = ReferenceCounter.GetReference<Shader>(name + path);

            if(servedShader == null)
            {
                servedShader = new Shader(name, path);

                if(!servedShader.Hashed || !servedShader.Loaded)
                {
                    servedShader.Dispose();
                    return null;
                }
                ReferenceCounter.AddNewReference(servedShader);
            }
            else
            {
                servedShader.RefCount++;
            }

            return servedShader;
        }

        public static Shader ServeShader(string name, string vsData, string fsData)
        {
            Shader servedShader = ReferenceCounter.GetReference<Shader>(name + vsData + fsData);

            if (servedShader == null)
            {
                servedShader = new Shader(name);

                servedShader.LoadShader(vsData, fsData);

                if (!servedShader.Hashed || !servedShader.Loaded)
                {
                    servedShader.Dispose();
                    return null;
                }
                ReferenceCounter.AddNewReference(servedShader);
            }
            else
            {
                servedShader.RefCount++;
            }

            return servedShader;
        }
    }

    internal class TextureResourceServer
    {
        public static Texture ServeTexture(string path)
        {
            Texture servedTexture = ReferenceCounter.GetReference<Texture>(path);

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
            Texture servedTexture = ReferenceCounter.GetReference<Texture>(bmp);

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
