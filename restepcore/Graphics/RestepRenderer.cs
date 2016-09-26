using System;
using OpenTK.Graphics.OpenGL;
using System.Collections.Concurrent;
using restep.Framework.Exceptions;
using restep.Framework.Logging;
using restep.Graphics.Renderables;

namespace restep.Graphics
{
    /// <summary>
    /// Primary rendering control for restep
    /// <para>This does not control GL itself, rather what is drawn</para>
    /// </summary>
    internal class RestepRenderer
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
            GL.DepthFunc(DepthFunction.Always);

            GL.DepthRange(0, 0);
        }

        #endregion

        public ConcurrentBag<FlatMesh> RenderedObjects { get; set; }

        private RestepRenderer()
        {
            RenderedObjects = new ConcurrentBag<FlatMesh>();
        }

        public void OnRender()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            foreach (FlatMesh mesh in RenderedObjects)
            {
                mesh.PreRender();
                mesh.Render();
            }
        }
    }
}
