using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;

namespace restep.Core.Collision
{
    internal class ObjectPartitioner
    {
        //private class QuadTree
        //{
        //    private List<GameObject> containedObjects;
        //    private QuadTree[] children;
        //    private int depth;
        //    private QuadTree parent;
        //    private Vector2 size;
        //    private Vector2 location;

        //    QuadTree(int depth, QuadTree parent, Vector2 size, Vector2 location)
        //    {
        //        this.depth = depth;
        //        this.parent = parent;
        //        this.size = size;
        //        this.location = location;
        //    }

        //    //x,y = pos, z,w = size
        //    public void Retrieve(Vector4 bounds, ref List<GameObject> outNearest)
        //    {
        //        if(bounds.X )
        //    }
        //}

        private List<GameObject> colliderList = new List<GameObject>();

        public void GetPossibleCollisions(GameObject refObject, ref List<GameObject> outPossibleCollisions)
        {
            foreach(GameObject o in colliderList)
            {
                if(o.ObjectID == refObject.ObjectID)
                {
                    continue;
                }


            }
        }

        public void AddNewObject(GameObject o)
        {
            colliderObjects.Add(o);
        }


    }
}
