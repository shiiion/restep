using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;

namespace restep.Core.Physics.Broadphase
{
    public delegate bool PossibleCollisionDelegate(GameObject t, GameObject o);
    //bad ciode
    public abstract class ObjectPartitioner
    {
        protected List<GameObject> colliderList = new List<GameObject>();
        
        public abstract void ForEachPossibleCollision(PossibleCollisionDelegate pcCallback);

        public void AddNewObject(GameObject o)
        {
            colliderList.Add(o);
        }

        public void RemoveObject(GameObject o)
        {
            colliderList.Remove(o);
        }
    }
}