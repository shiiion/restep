using System;
using OpenTK.Graphics.OpenGL;
using restep.Graphics.Intermediate;

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

            private static readonly int[] quadIndices =
                { 0, 1, 2, 3, 1, 2 };
            
            public static uint[] VBO { get; private set; }

            public static bool Initialized { get; private set; } = false;

            public static void Init()
            {
                GL.GenBuffers(BUFFER_COUNT, VBO);
                
                GL.BindBuffer(BufferTarget.ArrayBuffer, VBO[0]);
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(16), quadVertices, BufferUsageHint.StaticDraw);

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, VBO[1]);
                GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(24), quadIndices, BufferUsageHint.StaticDraw);
            }
        }

        public TexturedQuad(string texPath = "")
        {
            QuadTexture = new Texture(texPath);
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

        //TODO: implementme
        public override void InitMeshVertices(VertexData data)
        {
        }

        protected override void RenderMesh_Internal()
        {
            QuadTexture.BindAsPrimaryTexture();
            base.RenderMesh_Internal();
        }

        protected override void RenderWithMeshShaders()
        {

        }

        public override void PreRender()
        {
        }

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
