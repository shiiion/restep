using System;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;
using restep.Framework.Exceptions;
using restep.Framework.Logging;
using restep.Framework.ResourceManagement;

namespace restep.Graphics.Renderables
{
    /// <summary>
    /// Represents a texture, both on disk and in GL
    /// </summary>
    internal class Texture : CountableResource, IDisposable
    {
        /// <summary>
        /// Path to the texture on disk
        /// </summary>
        public string TexturePath { get; private set; }

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

        public Texture(Bitmap bmp)
        {
            Loaded = false;
            textureHandle = 0;

            LoadTexture(bmp);
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
                throw new LoggedException($"Failed to create texture handle! GL Error code: {GL.GetError()}", MessageLogger.RENDER_LOG, "Texture");
            }
        }

        private void setTextureProperties()
        {
            GL.BindTexture(TextureTarget.Texture2D, textureHandle);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        }

        /// <summary>
        /// Attempts to load the texture directly from a bitmap
        /// <para>Throws Exception on failure</para>
        /// </summary>
        /// <param name="image"></param>
        public void LoadTexture(Bitmap image)
        {
            try
            {
                genTexture();
                setTextureProperties();
                BitmapData bmpData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmpData.Width, bmpData.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmpData.Scan0);

                image.UnlockBits(bmpData);

                TexturePath = "@Resource";

                Loaded = true;
            }
            catch (LoggedException e)
            {
                destroyTexture();
                Loaded = false;
                throw e;
            }
            catch (Exception e)
            {
                MessageLogger.LogMessage(MessageLogger.RENDER_LOG, "Texture", MessageType.Error, e.Message, true);
                destroyTexture();
                Loaded = false;
                throw e;
            }
            finally
            {
                GL.BindTexture(TextureTarget.Texture2D, 0);
            }
            IdentifierHash = image.GetHashCode();
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
                LoadTexture(bmp);
            }
            finally
            {
                if (bmp != null)
                {
                    bmp.Dispose();
                }
            }
            IdentifierHash = path.GetHashCode();
        }

        /// <summary>
        /// Binds this texture as the primary sampler (diffuse) only if loaded
        /// </summary>
        public void BindAsPrimaryTexture()
        {
            if(Loaded)
            {
                GL.ActiveTexture(TextureUnit.Texture0);
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

        public override void OnDestroy()
        {
            Dispose();
        }

        public void Dispose()
        {
            destroyTexture();
        }
    }
}
