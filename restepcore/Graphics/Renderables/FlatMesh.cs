using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using restep.Graphics.Shaders;
using restep.Graphics.Intermediate;
using restep.Framework.Logging;

namespace restep.Graphics.Renderables
{

    public abstract class FlatMesh : IDisposable, IComparable<FlatMesh>
    {
        public static readonly int BUFFER_COUNT = 2;
        private static ulong UID_COUNTER = 0;

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

        /// <summary>
        /// The point within a mesh which the transform will rotate about
        /// </summary>
        public Vector2 Origin { get; set; }

        /// <summary>
        /// Unique identifier for this mesh, will be different for each (incrementally)
        /// </summary>
        public ulong MeshID { get; }

        protected uint vertexArray;
        protected uint[] vertexBuffers;
        protected int vertexCount;
        protected int[] attribSizes;

        /// <summary>
        /// Vertex structure used by ALL shaders for restep
        /// </summary>
        public abstract class BufferData
        {
            public float[] Data;

            //[buffer index][buffer data]

            public static float[][] SeparateArrays(BufferData[] buffers, DataFormat format)
            {
                float[][] ret = new float[format.GetNumAttributes()][];
                int[] attributeIndexCounter = new int[format.GetNumAttributes()];
                for(int a=0;a<ret.Length;a++)
                {
                    ret[a] = new float[buffers.Length * format.GetAttributeSizeAt(a)];
                    attributeIndexCounter[a] = 0;
                }

                for(int a=0;a<buffers.Length;a++)
                {
                    for(int b=0;b<buffers[a].Data.Length;b++)
                    {
                        int attributeIndex = format.GetAttributeIndex(b);
                        ret[attributeIndex][attributeIndexCounter[attributeIndex]] = buffers[a].Data[b];
                        attributeIndexCounter[attributeIndex]++;
                    }
                }
                return ret;
            }
        };

        public FlatMesh()
        {
            Transformation = new Transform(Framework.RestepGlobals.ContentAreaSize);

            MeshID = UID_COUNTER;
            UID_COUNTER++;
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

            RenderWithMeshShaders();
            for (int a = 0; a < Framework.RestepGlobals.LoadedShaders.Count; a++)
            {
                Shader s = Framework.RestepGlobals.LoadedShaders[a];

                if(s.Loaded && s.Enabled)
                {
                    s.UseShader();
                    OnBindGlobalShader(s);
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
            GL.BindVertexArray(vertexArray);
            
            for (int a = 0; a < attribSizes.Length; a++)
            {
                GL.EnableVertexAttribArray(a);
                GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffers[a]);
                GL.VertexAttribPointer(a, attribSizes[a], VertexAttribPointerType.Float, false, 0, 0);
            }

            GL.DrawArrays(PrimitiveType.Triangles, 0, vertexCount);

            for (int a = 0; a < attribSizes.Length; a++)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.DisableVertexAttribArray(a);
            }

            GL.BindVertexArray(0);
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
            //ALL shaders for this game require "transform" and "origin" uniforms
            gs.SetUniformMat3("transform", Transformation.Transformation);
            gs.SetUniformVec2("origin", Origin.X, Origin.Y);
        }

        public abstract void Dispose();

        /// <summary>
        /// Override of comparator to handle depth ordering
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(FlatMesh other)
        {
            return Depth.CompareTo(other.Depth);
        }
    }
}
