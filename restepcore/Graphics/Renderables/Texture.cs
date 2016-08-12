using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace restep.Graphics.Renderables
{
    internal class Texture : IDisposable
    {
        public string TexturePath { get; private set; }

        public bool Loaded { get; private set; }

        private int textureHandle;

        
        public Texture(string path = "")
        {
            Loaded = false;
            textureHandle = 0;

            if(!string.IsNullOrWhiteSpace(path))
            {
                LoadTexture(path);
            }
        }

        private void destroyTexture()
        {
            if(textureHandle != 0)
            {
                GL.DeleteTexture(textureHandle);
            }
            textureHandle = 0;
        }

        private void genTexture()
        {
            destroyTexture();

            textureHandle = GL.GenTexture();

            if(textureHandle <= 0)
            {
                throw new Exception("Failed to create texture handle! GL Error code: " + GL.GetError());
            }
        }

        private void setTextureProperties()
        {
            GL.BindTexture(TextureTarget.Texture2D, textureHandle);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        }

        public void LoadTexture(string path)
        {
            Bitmap bmp = null;
            try
            {
                bmp = new Bitmap(path);
                genTexture();
                setTextureProperties();
                BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmpData.Width, bmpData.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmpData.Scan0);

                bmp.UnlockBits(bmpData);

                TexturePath = path;
            }
            catch (Exception e)
            {
                destroyTexture();
                Loaded = false;
                throw e;
            }
            finally
            {
                GL.BindTexture(TextureTarget.Texture2D, 0);
                if (bmp != null)
                {
                    bmp.Dispose();
                }
            }
        }

        public void BindAsPrimaryTexture()
        {
            if(Loaded)
            {
                GL.BindTexture(TextureTarget.Texture2D, textureHandle);
            }
        }

        public void UnbindTexture()
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public void Dispose()
        {
            destroyTexture();
        }

        ~Texture()
        {
            Dispose();
        }
    }
}
