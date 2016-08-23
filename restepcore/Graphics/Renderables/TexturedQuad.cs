using System;
using OpenTK.Graphics.OpenGL;
using restep.Graphics.Intermediate;
using restep.Graphics.Shaders;
using restep.Framework.Logging;

namespace restep.Graphics.Renderables
{
    //TODO optimization: texture store to save GL memory
    //TODO optimization: texture reference counter

    /// <summary>
    /// Class representing vertices and texture for a textured quad
    /// </summary>
    internal class TexturedQuad : FlatMesh
    {
        /// <summary>
        /// Handle to the texture used by this mesh
        /// </summary>
        public Texture QuadTexture { get; set; }
        
        /// <summary>
        /// Class defining vertex data used by all textured quads
        /// <para>Data here will be reused for all textured quads, but will be transformed and textured differently for each TexturedQuad</para>
        /// </summary>
        private static class TexturedQuad_Internal
        {
            private static readonly BufferData[] quadVertices =
            {
                new BufferData(0, 0, 0, 0), new BufferData(1, 0, 1, 0),
                new BufferData(0, 1, 0, 1), new BufferData(1, 1, 1, 1)
            };

            private static readonly ushort[] quadIndices =
                { 0, 1, 2, 1, 3, 2 };

            public static uint VAO { get; private set; }

            public static uint[] VBO { get; private set; } = new uint[2];

            public static bool Initialized { get; private set; } = false;

            public static void Init()
            {
                uint vao;
                GL.GenVertexArrays(1, out vao);
                VAO = vao;
                GL.BindVertexArray(VAO);

                GL.GenBuffers(BUFFER_COUNT, VBO);

                float[] bufferArray = BufferData.MergeArrays(quadVertices);

                GL.BindBuffer(BufferTarget.ArrayBuffer, VBO[0]);
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(bufferArray.Length * sizeof(float)), bufferArray, BufferUsageHint.StaticDraw);
                
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, VBO[1]);
                GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(quadIndices.Length * sizeof(ushort)), quadIndices, BufferUsageHint.StaticDraw);
            }
        }

        public TexturedQuad(string texPath = "")
        {
            QuadTexture = new Texture(texPath);
            InitMeshVertices();
        }

        public TexturedQuad(System.Drawing.Bitmap bitmap)
        {
            QuadTexture = new Texture(bitmap);
            InitMeshVertices();
        }

        public override void InitMeshVertices()
        {
            if (!TexturedQuad_Internal.Initialized)
            {
                TexturedQuad_Internal.Init();
            }

            indexCount = 6;
            
            VBO = TexturedQuad_Internal.VBO;

            Loaded = true;
        }
        
        public override void InitMeshVertices(VertexData data)
        {
            MessageLogger.LogMessage(MessageLogger.RENDER_LOG, "TexturedQuad", MessageType.Warning, "Attempted to load from VertexData for a TexturedQuad, load refused.", true);
        }

        protected override void RenderMesh_Internal()
        {
            QuadTexture.BindAsPrimaryTexture();
            base.RenderMesh_Internal();
        }

        protected override void RenderWithMeshShaders() { }

        public override void PreRender() { }

        protected override void OnBindGlobalShader(Shader gs)
        {
            base.OnBindGlobalShader(gs);
        }

        public override void Dispose()
        {
            //TODO: dispose texture
        }
    }
}
