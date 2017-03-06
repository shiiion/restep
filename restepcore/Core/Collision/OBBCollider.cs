using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;

namespace restep.Core.Collision
{
    internal sealed class OBBCollider : AABBCollider
    {
        public override ColliderType Type { get { return ColliderType.CT_OBB; } }

        /// <summary>
        /// Describes the rotation of this object about its center
        /// </summary>
        public float Rotation { get { return Owner.Rotation; } }

        public OBBCollider(GameObject owner, Vector2 bounds) : base(owner, bounds) {}

        /// <summary>
        /// Rotates a point about the center of this OBB clockwise
        /// </summary>
        /// <returns>Point rotated about the center of this OBB</returns>
        private Vector2 getUnrotatedPointLocation(Vector2 point)
        {
            Vector2 relLoc = point - Pos;

            float cosf = (float)Math.Cos(-Rotation);
            float sinf = (float)Math.Sin(-Rotation);

            Vector2 relRot = new Vector2(relLoc.X * cosf - relLoc.Y * sinf, relLoc.X * sinf + relLoc.Y * cosf);

            return relRot + Pos;
        }

        public override bool TestCollision(Collider other)
        {
            switch(other.Type)
            {
                case ColliderType.CT_AABB:
                    return testOBB_AABB((AABBCollider)other);
                case ColliderType.CT_CIRCLE:
                    return testOBB_Circle((CircleCollider)other);
                case ColliderType.CT_OBB:
                    return testOBB_OBB((OBBCollider)other);
                case ColliderType.CT_CONVEX:
                    return other.TestCollision(this);
            }

            return false;
        }

        private bool testOBB_AABB(AABBCollider other)
        {

            return false;
        }

        private bool testOBB_Circle(CircleCollider other)
        {
            Vector2 rotatedCenter = getUnrotatedPointLocation(other.Pos);

            return testAABB_Circle_Internal(rotatedCenter, other.Radius);
        }

        private bool testOBB_OBB(OBBCollider other)
        {
            return false;
        }

        public override bool TestPoint(Vector2 point)
        {
            return base.TestPoint(getUnrotatedPointLocation(point));
        }

        internal override bool IsMTCTHandledByThis(ColliderType otherType)
        {
            switch (otherType)
            {
                case ColliderType.CT_AABB:
                    return true;
                case ColliderType.CT_CIRCLE:
                    return true;
                case ColliderType.CT_OBB:
                    return true;
                //Opposing collider should treat this as another convex
                case ColliderType.CT_CONVEX:
                    return false;
            }
            return false;
        }
    }
}
