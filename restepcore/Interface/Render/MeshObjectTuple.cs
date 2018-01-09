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
        public List<FlatMesh> MeshList { get; private set; } = new List<FlatMesh>();

        public MeshObjectTuple(RenderObject obj, FlatMesh mesh)
        {
            Object = obj;
            obj.ActiveMeshID = mesh.MeshID;
            MeshList.Add(mesh);
        }

        public bool Mismatch()
        {
            return Object.Invalidated;
        }

        public void Validate()
        {
            foreach (FlatMesh Mesh in MeshList)
            {
                if (Mesh.MeshID == Object.ActiveMeshID)
                {
                    Mesh.Transformation.Translation = Object.Position;
                    Mesh.Transformation.Rotation = Object.Rotation;
                    Mesh.Transformation.Scale = Object.Scale;
                    Mesh.Transformation.BaseScale = Object.ImageScale;
                    Mesh.Transparency = Object.Transparency;
                }
                else
                {
                    Mesh.Transformation.BaseScale = new OpenTK.Vector2(0, 0);
                }
            }
            Object.Invalidated = false;
        }
    }
}
