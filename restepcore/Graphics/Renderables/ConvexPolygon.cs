using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using restep.Framework.Logging;
using restep.Framework;
using restep.Graphics.Intermediate;
using restep.Graphics.Shaders;


namespace restep.Graphics.Renderables
{
    internal class ConvexVertexFormat : DataFormat
    {
        protected override void InitFormat()
        {
            bufferOrder.Add(LayoutQualifierType.Vec2);
        }
    }

    internal class ConvexPolygon : FlatMesh
    {
        private static Shader colorShader = null;
        
        /// <summary>
        /// Sets the color of both ColorLow and ColorHigh to the same value;
        /// </summary>
        public Vector4 Color { get; set; }

        /// <summary>
        /// This should be auto-called by RestepWindow 
        /// </summary>
        public static void InitClass()
        {
            if (colorShader == null)
            {
                colorShader = new Shader("colorShader");
                colorShader.LoadShader(RestepGlobals.COLOR_SHADER_VTX, RestepGlobals.COLOR_SHADER_FRAG);

                colorShader.AddUniform("transform");
                colorShader.AddUniform("origin");
                colorShader.AddUniform("color");

                colorShader.Enabled = true;

                MessageLogger.LogMessage(MessageLogger.RENDER_LOG, "ConvexPolygon", MessageType.Success, "Loaded, compiled, and linked texture shader successfully. All uniforms found.", true);
            }
            else
            {
                MessageLogger.LogMessage(MessageLogger.RENDER_LOG, "ConvexPolygon", MessageType.Warning, "Attempted to re-intialize ConvexPolygon shader! Make sure no extra calls are made to ConvexPolygon.InitClass.", true);
            }
        }

        public ConvexPolygon(VertexData data)
        {
            InitMeshVertices(data);
        }

        public override void InitMeshVertices()
        {
            MessageLogger.LogMessage(MessageLogger.RENDER_LOG, "ConvexPolygon", MessageType.Warning, "Attempted to load default mesh for a ConvexPolygon.", true);
        }

        public override void InitMeshVertices(VertexData data)
        {
            if(!(data.BufferFormat is ConvexVertexFormat))
            {
                MessageLogger.LogMessage(MessageLogger.RENDER_LOG, "ConvexPolygon", MessageType.Warning, "Attempted to load vertex data for ConvexPolygon in wrong format! Make sure that VertexData.BufferFormat is of type ConvexVertexFormat.", true);
                return;
            }
            //TODO: cleanup old vertices if exist
            vertexBuffers = new uint[1];

            GL.GenVertexArrays(1, out vertexArray);
            GL.BindVertexArray(vertexArray);

            GL.GenBuffers(BUFFER_COUNT, vertexBuffers);
            
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffers[0]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(data.BuffersOut[0].Length * sizeof(float)), data.BuffersOut[0], BufferUsageHint.StaticDraw);

            vertexCount = data.VertexCount;

            attribSizes = data.BufferFormat.GetSizes();

            Loaded = true;

            GL.BindVertexArray(0);
        }
        
        protected override void RenderWithMeshShaders()
        {
            if (colorShader != null && colorShader.Loaded)
            {
                colorShader.UseShader();
                colorShader.SetUniformMat3("transform", Transformation.Transformation);
                colorShader.SetUniformVec2("origin", Origin.X, Origin.Y);
                colorShader.SetUniformVec4("color", Color);

                RenderMesh_Internal();
            }
            else
            {
                MessageLogger.LogMessage(MessageLogger.RENDER_LOG, "TexturedQuad", MessageType.Warning, "Attempted to render a ConvexPolygon before the class has been initialized! Check to make sure that a successful call is made to ConvexPolygon.InitClass!", true);
            }
        }
        
        public override void Dispose()
        {
            if(Loaded)
            {
                GL.DeleteBuffers(vertexBuffers.Length, vertexBuffers);
                GL.DeleteVertexArray(vertexArray);
            }
        }
    }
}
