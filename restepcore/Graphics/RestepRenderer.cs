using System;
using OpenTK.Graphics.OpenGL;
using System.Collections.Concurrent;
using System.Collections.Generic;
using restep.Framework.Exceptions;
using restep.Framework.Logging;
using restep.Graphics.Renderables;
using restep.Framework;

namespace restep.Graphics
{
    /// <summary>
    /// Primary rendering control for restep
    /// <para>This does not control GL itself, rather what is drawn</para>
    /// </summary>
    public class RestepRenderer
    {
        #region ~singleton data~
        private static RestepRenderer instance;
        /// <summary>
        /// Singleton instance of this class
        /// <para>Initialize must be called before accessing this</para>
        /// </summary>
        public static RestepRenderer Instance
        {
            get
            {
                if(Initialized)
                {
                    return instance;
                }
                throw new LoggedException("Failed to get instance as renderer has not been initialized yet!", MessageLogger.RENDER_LOG, "RestepRenderer");
            }
        }

        public static bool Initialized { get; set; } = false;
        
        /// <summary>
        /// Initializes the singleton instance of this class
        /// </summary>
        public static void Initialize()
        {
            if(!RestepWindow.Initialized)
            {
                throw new LoggedException("Failed to initialize renderer as RestepWindow has not been initialized yet!", MessageLogger.RENDER_LOG, "RestepRenderer");
            }
            instance = new RestepRenderer();
            Initialized = true;

            GL.ClearColor(0, 0, 0, 0);
            //GL.FrontFace(FrontFaceDirection.Cw);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.AlphaTest);

            GL.Disable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Never);

            GL.DepthRange(0, 0);

            GL.Enable(EnableCap.Multisample);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            instance.initPostProcessBuffer();
        }

        private void initPostProcessBuffer()
        {
            GL.GenFramebuffers(1, out sceneFramebuffer);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, sceneFramebuffer);

            FramebufferErrorCode error;
            if((error = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer)) != FramebufferErrorCode.FramebufferComplete)
            {
                //TODO: handle me
            }
            
            GL.GenTextures(1, out framebufferTexture);
            GL.BindTexture(TextureTarget.Texture2D, framebufferTexture);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, RestepWindow.Instance.Width,
                RestepWindow.Instance.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);

            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0,
                TextureTarget.Texture2D, framebufferTexture, 0);

            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            //TODO: handle resize of window (includes deleting framebuffer)

            GL.GenVertexArrays(1, out fbTexVArray);
            GL.BindVertexArray(fbTexVArray);

            GL.GenBuffers(1, out fbTexVertices);
            GL.BindBuffer(BufferTarget.ArrayBuffer, fbTexVertices);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(12 * sizeof(float)), new float[] 
            {  -1, -1,
                1, -1,
               -1, 1,
                1, 1,
                1, -1,
               -1, 1 }, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            GL.BindVertexArray(0);
        }

        #endregion
        
        private List<FlatMesh> renderedObjects;
        private Shaders.Shader postBaseShader;

        public float RenderingTimeSlice { get; private set; }

        private int sceneFramebuffer, framebufferTexture;
        private uint fbTexVArray, fbTexVertices;

        private bool updateDepth;

        private RestepRenderer()
        {
            renderedObjects = new List<FlatMesh>();
            postBaseShader = new Shaders.Shader("POST_TEST", "");
            postBaseShader.LoadShader(RestepGlobals.FXAA_VTX, RestepGlobals.FXAA_FRAG);
            postBaseShader.Enabled = true;

            postBaseShader.AddUniform("FXAA_SPAN_MAX");
            postBaseShader.AddUniform("FXAA_REDUCE_MUL");
            postBaseShader.AddUniform("FXAA_REDUCE_MIN");
            postBaseShader.AddUniform("frameBufferSize");
        }

        /// <summary>
        /// Tells rendering thread to reorder all meshes in list
        /// TODO: optimize
        /// </summary>
        public void InvalidateDepth()
        {
            updateDepth = true;
        }

        public bool MeshExists(FlatMesh mesh)
        {
            foreach(FlatMesh m in renderedObjects)
            {
                if(mesh.MeshID == m.MeshID)
                {
                    return true;
                }
            }
            return false;
        }

        public void AddMesh(FlatMesh mesh)
        {
            lock (renderedObjects)
            {
                if (!MeshExists(mesh))
                {
                    renderedObjects.Add(mesh);
                }
            }

            InvalidateDepth();
        }

        //this is only ever called when renderedObjects is locked
        private void reorderMeshes()
        {
            renderedObjects.Sort();
            updateDepth = false;
        }

        public void Render(float deltaTime)
        {
            Interface.Render.RenderInterface.UpdateAll();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, sceneFramebuffer);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            RenderingTimeSlice = deltaTime;
            lock(renderedObjects)
            {
                if (updateDepth)
                {
                    reorderMeshes();
                }

                foreach (FlatMesh mesh in renderedObjects)
                {
                    mesh.PreRender();
                    mesh.Render();
                }
            }
        }

        public void RenderPost(float deltaTime)
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            postBaseShader.UseShader();
            //GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.BindTexture(TextureTarget.Texture2D, framebufferTexture);

            postBaseShader.SetUniformFloat("FXAA_SPAN_MAX", 4);
            postBaseShader.SetUniformFloat("FXAA_REDUCE_MUL", 1.0f / 8.0f);
            postBaseShader.SetUniformFloat("FXAA_REDUCE_MIN", 1.0f / 128.0f);
            postBaseShader.SetUniformVec3("frameBufferSize", RestepWindow.Instance.Width, RestepWindow.Instance.Height, 0);

            GL.BindVertexArray(fbTexVArray);
            GL.EnableVertexAttribArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, fbTexVertices);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 0, 0);

            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DisableVertexAttribArray(0);
            GL.BindVertexArray(0);
        }
    }
}
