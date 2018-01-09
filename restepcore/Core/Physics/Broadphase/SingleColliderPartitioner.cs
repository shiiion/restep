using System;
using System.Collections.Generic;
using System.Text;

namespace restep.Core.Physics.Broadphase
{
    /// <summary>
    /// Object partitioner which only worries about one object
    /// <para>Complexity is N where N = num colliders</para>
    /// </summary>
    public class SingleColliderPartitioner : ObjectPartitioner
    {
        public GameObject PrimaryCollider { get; set; }

        public SingleColliderPartitioner(GameObject colliderTester)
        {
            PrimaryCollider = colliderTester;
        }

        public override void ForEachPossibleCollision(PossibleCollisionDelegate pcCallback)
        {
            if(PrimaryCollider == null)
            {
                return;
            }
            List<GameObject> collisions = new List<GameObject>();

            foreach(GameObject o in colliderList)
            {
                if(o.ObjectID == PrimaryCollider.ObjectID)
                {
                    continue;
                }
                if(pcCallback(PrimaryCollider, o))
                {
                    collisions.Add(o);
                }
            }

            foreach (GameObject o in collisions)
            {
                PrimaryCollider.OnOverlap(o);
                o.OnOverlap(PrimaryCollider);
            }
        }
    }
}
