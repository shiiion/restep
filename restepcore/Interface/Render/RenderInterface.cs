using System;
using System.Collections.Concurrent;
using System.Text;
using restep.Graphics.Renderables;
using restep.Core;

namespace restep.Interface.Render
{
    /// <summary>
    /// Threadsafe interface between core and renderer
    /// </summary>
    public class RenderInterface
    {
        private static ConcurrentBag<MeshObjectTuple> tupleList = new ConcurrentBag<MeshObjectTuple>();

        /// <summary>
        /// Checks all tuples within the tuple list for mismatches and proceeds to update
        /// </summary>
        public static void UpdateAll()
        {
            lock(CoreThread.Instance.CoreLock)
            {
                lock (tupleList)
                {
                    foreach (MeshObjectTuple tuple in tupleList)
                    {
                        if (tuple.Mismatch())
                        {
                            tuple.Validate();
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Simultaneously creates a MeshObject pair and ensures the mesh has been added to the renderer
        /// </summary>
        /// <param name="mesh">Mesh to link to the game object</param>
        /// <param name="obj">Game object controlling the mesh</param>
        public static void AddPair(FlatMesh mesh, RenderObject obj)
        {
            lock(tupleList)
            {
                mesh.Origin = new OpenTK.Vector2(0.5f, 0.5f);
                Graphics.RestepRenderer.Instance.AddMesh(mesh);
                foreach(MeshObjectTuple tuple in tupleList)
                {
                    if(tuple.Object.ObjectID == obj.ObjectID)
                    {
                        tuple.MeshList.Add(mesh);
                        return;
                    }
                }
                tupleList.Add(new MeshObjectTuple(obj, mesh));
            }
        }

        public static void RemovePair(RenderObject obj)
        {
            lock(tupleList)
            {
                MeshObjectTuple remove = null;
                foreach(MeshObjectTuple tuple in tupleList)
                {
                    if(tuple.Object.ObjectID == obj.ObjectID)
                    {
                        remove = tuple;
                        break;
                    }
                }
                if (remove != null)
                {
                    foreach (FlatMesh m in remove.MeshList)
                    {
                        Graphics.RestepRenderer.Instance.RemoveMesh(m);
                        m.Dispose();
                    }
                }
            }
        }

        public static void OnResize(OpenTK.Vector2 newSize)
        {
            lock(tupleList)
            {
                foreach (MeshObjectTuple tuple in tupleList)
                {
                    foreach (FlatMesh m in tuple.MeshList)
                    {
                        m.Transformation.ScreenSpace = newSize;
                    }
                }
            }
        }
    }
}
