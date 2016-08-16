using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace restep.Graphics.Renderables
{
    internal abstract class FlatMesh
    {
        /// <summary>
        /// If false, the mesh will ignore the first shader in LoadedShaders
        /// </summary>
        public bool UsingBaseShader { get; set; }

        protected uint VAO;
        protected int VAODrawCount;


        public void Render()
        {
            foreach(Shader s in Framework.RestepGlobals.LoadedShaders)
            {
                if(!s.Loaded || !s.Enabled || (!UsingBaseShader &&)
                {
                    continue;
                }
                s.UseShader();
                RenderMesh_Internal();
            }
            RenderWithMeshShaders();
        }

        public abstract void PreRender();

        protected virtual void RenderMesh_Internal()
        {
            GL.BindVertexArray(VAO);
            GL.DrawArrays(PrimitiveType.Triangles, 0, VAODrawCount);
            GL.BindVertexArray(0);
        }

        protected abstract void RenderWithMeshShaders();
    }
}
