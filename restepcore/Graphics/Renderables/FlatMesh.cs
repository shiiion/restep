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

        public Transform Transformation { get; set; }

        protected uint VAO;
        protected int VAODrawCount;

        public FlatMesh()
        {
            Transformation = new Transform(Framework.RestepGlobals.ContentAreaSize);
        }

        public void Render()
        {
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

        protected virtual void RenderMesh_Internal()
        {
            GL.BindVertexArray(VAO);
            GL.DrawArrays(PrimitiveType.Triangles, 0, VAODrawCount);
            GL.BindVertexArray(0);
        }

        protected abstract void RenderWithMeshShaders();

        public abstract void PreRender();

        protected virtual void OnBindGlobalShader(Shader gs)
        {
            gs.SetUniformMat3("transform", Transformation.Transformation);
        }
    }
}
