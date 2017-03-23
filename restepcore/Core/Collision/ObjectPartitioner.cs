using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;

namespace restep.Core.Collision
{
    public delegate void PossibleCollisionDelegate(GameObject t, GameObject o);
    //bad ciode
    internal class ObjectPartitioner
    {
        private List<GameObject> colliderList = new List<GameObject>();

        public void GetPossibleCollisions(GameObject refObject, ref List<GameObject> outPossibleCollisions)
        {
            outPossibleCollisions.Clear();

            foreach(GameObject o in colliderList)
            {
                if(o.ObjectID == refObject.ObjectID || !o.HasCollider)
                {
                    continue;
                }

                if(refObject.BoundsOverlap(o))
                {
                    outPossibleCollisions.Add(o);
                }
            }
        }

        public void ForEachPossibleCollision(PossibleCollisionDelegate pcCallback)
        {
            List<GameObject> objectList = new List<GameObject>();
            foreach(GameObject t in colliderList)
            {
                GetPossibleCollisions(t, ref objectList);
                foreach (GameObject o in objectList)
                {
                    pcCallback(t, o);
                }
            }
        }

        public void AddNewObject(GameObject o)
        {
            colliderList.Add(o);
        }
    }
}