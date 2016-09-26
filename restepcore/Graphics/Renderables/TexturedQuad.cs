using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using restep.Graphics.Intermediate;
using restep.Graphics.Shaders;
using restep.Framework;
using restep.Framework.Logging;

namespace restep.Graphics.Renderables
{
    //TODO optimization: texture store to save GL memory
    //TODO optimization: texture reference counter
    //TODO optimization: texture atlasing

    /// <summary>
    /// Class representing vertices and texture for a textured quad
    /// </summary>
    internal class TexturedQuad : FlatMesh
    {
        private static Shader textureShader = null;

        /// <summary>
        /// This should be auto-called by RestepWindow 
        /// </summary>
        public static void InitClass()
        {
            if(textureShader == null)
            {
                textureShader = new Shader("textureShader");
                textureShader.LoadShader(RestepGlobals.TEX_SHADER_VTX, RestepGlobals.TEX_SHADER_FRAG);

                textureShader.AddUniform("transform");
                textureShader.AddUniform("origin");

                textureShader.Enabled = true;

                MessageLogger.LogMessage(MessageLogger.RENDER_LOG, "TexturedQuad", MessageType.Success, "Loaded, compiled, and linked texture shader successfully. All uniforms found.", true);
            }
            else
            {
                MessageLogger.LogMessage(MessageLogger.RENDER_LOG, "TexturedQuad", MessageType.Warning, "Attempted to re-intialize TexturedQuad shader! Make sure no extra calls are made to TexturedQuad.InitClass.", true);
            }
        }
        
        /// <summary>
        /// Handle to the texture used by this mesh
        /// </summary>
        public Texture QuadTexture { get; set; }
        
        private class QuadBufferData : BufferData
        {
            public QuadBufferData(float x, float y, float u, float v)
            {
                Data = new float[4] { x, y, u, v };
            }
        }

        private class QuadFormat : DataFormat
        {
            protected override void InitFormat()
            {
                bufferOrder.Add(LayoutQualifierType.Vec2);
                bufferOrder.Add(LayoutQualifierType.Vec2);
            }
        }

        /// <summary>
        /// Class defining vertex data used by all textured quads
        /// <para>Data here will be reused for all textured quads, but will be transformed and textured differently for each TexturedQuad</para>
        /// </summary>
        private static class TexturedQuad_Internal
        {
            private static readonly BufferData[] quadVertices =
            {
                new QuadBufferData(0, 0, 0, 0), new QuadBufferData(1, 0, 1, 0), new QuadBufferData(0, 1, 0, 1),
                new QuadBufferData(1, 1, 1, 1), new QuadBufferData(1, 0, 1, 0), new QuadBufferData(0, 1, 0, 1)
            };

            public static uint VertexArray { get; private set; }

            public static uint[] VertexBuffers { get; private set; } = new uint[2];

            public static bool Initialized { get; private set; } = false;

            public static int[] Attribs { get; private set; }

            public static void Init()
            {
                uint vao;
                QuadFormat format = new QuadFormat();

                GL.GenVertexArrays(1, out vao);
                VertexArray = vao;
                GL.BindVertexArray(VertexArray);

                GL.GenBuffers(BUFFER_COUNT, VertexBuffers);

                float[][] bufferData = BufferData.SeparateArrays(quadVertices, format);

                GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBuffers[0]);
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(bufferData[0].Length * sizeof(float)), bufferData[0], BufferUsageHint.StaticDraw);

                GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBuffers[1]);
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(bufferData[1].Length * sizeof(float)), bufferData[1], BufferUsageHint.StaticDraw);

                Attribs = format.GetSizes();

                Initialized = true;
                 
                GL.BindVertexArray(0);
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

            vertexCount = 6;

            vertexArray = TexturedQuad_Internal.VertexArray;
            vertexBuffers = TexturedQuad_Internal.VertexBuffers;

            attribSizes = TexturedQuad_Internal.Attribs;

            Loaded = true;
        }
        
        public override void InitMeshVertices(VertexData data)
        {
            MessageLogger.LogMessage(MessageLogger.RENDER_LOG, "TexturedQuad", MessageType.Warning, "Attempted to load from VertexData for a TexturedQuad, load refused.", true);
        }

        protected override void RenderMesh_Internal()
        {
            //TODO: texture atlasing here (texture bindings hurt)
            QuadTexture.BindAsPrimaryTexture();
            base.RenderMesh_Internal();
        }

        protected override void RenderWithMeshShaders()
        {
            if(textureShader != null && textureShader.Loaded)
            {
                textureShader.UseShader();
                textureShader.SetUniformMat3("transform", Transformation.Transformation);
                textureShader.SetUniformVec2("origin", Origin.X, Origin.Y);

                RenderMesh_Internal();
            }
            else
            {
                MessageLogger.LogMessage(MessageLogger.RENDER_LOG, "TexturedQuad", MessageType.Warning, "Attempted to render a TexturedQuad before the class has been initialized! Check to make sure that a successful call is made to texturedQuad.InitClass!", true);
            }
        }

        public override void PreRender() { }

        protected override void OnBindGlobalShader(Shader gs) { }

        public override void Dispose()
        {
            //TODO: decrement reference counter (on implementation)
        }
    }
}
