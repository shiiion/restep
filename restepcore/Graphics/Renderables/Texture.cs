using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace restep.Graphics.Renderables
{
    /// <summary>
    /// Represents a texture, both on disk and in GL
    /// </summary>
    internal class Texture : IDisposable
    {
        /// <summary>
        /// Path to the texture on disk
        /// </summary>
        public string TexturePath { get; private set; }

        /// <summary>
        /// Whether or not the texture has been loaded into GL memory
        /// </summary>
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

        /// <summary>
        /// Attempts to load the texture in one go from path
        /// <para>Throws Exception on failure</para>
        /// </summary>
        /// <param name="path">Path to the texture on disk</param>
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

        /// <summary>
        /// Binds this texture as the primary sampler (diffuse) only if loaded
        /// </summary>
        public void BindAsPrimaryTexture()
        {
            if(Loaded)
            {
                GL.BindTexture(TextureTarget.Texture2D, textureHandle);
            }
        }

        /// <summary>
        /// Binds the null texture as the primary sampler
        /// </summary>
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
