using System;
using System.Collections.Generic;
using System.Text;
using restep.Core;
using restep.Graphics.Renderables;


namespace restep.Interface.Render
{
    /// <summary>
    /// Tuple of Mesh and Game Object utilized by RenderInterface
    /// </summary>
    internal class MeshObjectTuple
    {
        public RenderObject Object { get; private set; }
        public FlatMesh Mesh { get; private set; }

        public MeshObjectTuple(RenderObject obj, FlatMesh mesh)
        {
            Object = obj;
            Mesh = mesh;
        }

        public bool Mismatch()
        {
            return Object.Invalidated;
        }

        public void Validate()
        {
            Object.Invalidated = false;
        }
    }
}
