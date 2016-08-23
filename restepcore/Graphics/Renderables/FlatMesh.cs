using System;
using OpenTK.Graphics.OpenGL;
using restep.Graphics.Shaders;
using restep.Graphics.Intermediate;
using restep.Framework.Logging;

namespace restep.Graphics.Renderables
{
    internal abstract class FlatMesh : IDisposable
    {
        public static readonly int BUFFER_COUNT = 2;

        public float Depth { get; set; }

        /// <summary>
        /// Tells whether or not the mesh has successfully loaded all of its data
        /// </summary>
        public bool Loaded { get; protected set; } = false;

        /// <summary>
        /// If false, the mesh will ignore the first shader in LoadedShaders
        /// </summary>
        public bool UsingBaseShader { get; set; } = true;

        /// <summary>
        /// Transformation data of the mesh
        /// </summary>
        public Transform Transformation { get; set; }

        protected uint[] IBO;
        protected uint[] VBO;
        protected int indexCount;

        /// <summary>
        /// Vertex structure used by ALL shaders for restep
        /// </summary>
        public struct BufferData
        {
            public BufferData(float x, float y, float u, float v)
            {
                Data = new float[]{ x, y, u, v };
            }

            public float[] Data;

            public static float[] MergeArrays(BufferData[] buffers)
            {
                float[] ret = new float[buffers[0].Data.Length * buffers.Length];
                for(int a=0;a<buffers.Length;a++)
                {
                    for(int b=0;b<buffers[a].Data.Length;b++)
                    {
                        ret[(buffers[a].Data.Length * a) + b] = buffers[a].Data[b];
                    }
                }
                return ret;
            }
        };

        public FlatMesh()
        {
            Transformation = new Transform(Framework.RestepGlobals.ContentAreaSize);
        }

        /// <summary>
        /// Initializes a mesh that has hardcoded vertex data (e.g. TexturedQuad)
        /// </summary>
        /// 
        public abstract void InitMeshVertices();

        //TODO: make me notvirtual? (no abstraction required)
        /// <summary>
        /// Initializes a mesh that gets its vertex data from another source
        /// </summary>
        /// <param name="data">Vertex data to use</param>
        public abstract void InitMeshVertices(VertexData data);


        /// <summary>
        /// Renders the mesh with all enabled shaders
        /// </summary>
        public void Render()
        {
            if(!Loaded)
            {
                MessageLogger.LogMessage(MessageLogger.RENDER_LOG, "FlatMesh", MessageType.Error, $"Failed to render mesh! Mesh has not been loaded. Classtype: {GetType().Name}", true);
                return;
            }
            //always garunteed 1 shader, else the program won't run this 
            Shader baseShader = Framework.RestepGlobals.LoadedShaders[0];
            if(baseShader.Loaded && baseShader.Enabled && UsingBaseShader)
            {
                baseShader.UseShader();
                OnBindGlobalShader(baseShader);
                RenderMesh_Internal();
            }

            RenderWithMeshShaders();

            for (int a = 1; a < Framework.RestepGlobals.LoadedShaders.Count; a++)
            {
                Shader s = Framework.RestepGlobals.LoadedShaders[a];

                if(s.Loaded && s.Enabled)
                {
                    s.UseShader();
                    OnBindGlobalShader(baseShader);
                    RenderMesh_Internal();
                }
            }
        }

        /// <summary>
        /// Called after a shader has been set up and the mesh is ready to be rendered
        /// <para>Override this if your mesh class has special rendering setup to do before any rendering is done</para>
        /// </summary>
        protected virtual void RenderMesh_Internal()
        {
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.DrawElements(BeginMode.Triangles, indexCount, DrawElementsType.UnsignedShort, 0);

            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);
        }

        /// <summary>
        /// Called after the base shader has been used, but before other global shaders have been used
        /// <para>Override this if your mesh class has its own kind of shaders</para>
        /// </summary>
        protected abstract void RenderWithMeshShaders();

        /// <summary>
        /// Called before Render is called. Used to set up rendering a mesh.
        /// </summary>
        public abstract void PreRender();

        /// <summary>
        /// After a global shader (within RestepGlobals) has been bound
        /// </summary>
        /// <param name="gs">Global shader which has been bound</param>
        protected virtual void OnBindGlobalShader(Shader gs)
        {
            gs.SetUniformMat3("transform", Transformation.Transformation);
            gs.SetUniformVec2("origin", Transformation.Origin.X, Transformation.Origin.Y);
        }

        public abstract void Dispose();
    }
}
