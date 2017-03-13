using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;

namespace restep.Core.Collision
{
    /// <summary>
    /// Standard Axis-Aligned Bounding-Box, where Pos is the center of the AABB
    /// </summary>
    internal class AABBCollider : Collider
    {
        public override ColliderType Type { get { return ColliderType.CT_AABB; } }

        private Vector2 boundsUnscaled;
        /// <summary>
        /// Describes the bounds of this object along the X and Y axes
        /// </summary>
        public Vector2 HalfBounds
        {
            get
            {
                return boundsUnscaled * Owner.Scale;
            }
            set
            {
                boundsUnscaled = value;
            }
        }

        public AABBCollider(GameObject owner, Vector2 halfBounds, bool createBBox) : base(owner)
        {
            HalfBounds = halfBounds;
            if(createBBox)
            {
                BBox = new AABBCollider(owner, halfBounds, false);
            }
        }

        public override bool TestOverlap(Collider other)
        {
            switch(other.Type)
            {
                case ColliderType.CT_AABB:
                    return testAABB_AABB((AABBCollider)other);
                case ColliderType.CT_CIRCLE:
                    return testAABB_Circle((CircleCollider)other);
                case ColliderType.CT_CONVEX:
                    return other.TestOverlap(this);
                case ColliderType.CT_OBB:
                    return other.TestOverlap(this);
            }
            
            return false;
        }

        private bool testAABB_AABB(AABBCollider other)
        {
            Func<float, float, float, float, bool> checkAxis = (a1, a1d, a2, a2d) =>
            {
                return (a1 >= a2 && a1 <= a2 + a2d) || (a1 < a2 && a1 + a1d >= a2);
            };

            return (checkAxis(Pos.X - HalfBounds.X, 2 * HalfBounds.X, other.Pos.X - other.HalfBounds.X, 2 * other.HalfBounds.X) &&
                checkAxis(Pos.Y - HalfBounds.Y, 2 * HalfBounds.Y, other.Pos.Y - other.HalfBounds.Y, 2 * other.HalfBounds.Y));
        }

        protected bool testAABB_Circle_Internal(Vector2 center, float radius)
        {
            if(TestPoint(center))
            {
                return true;
            }

            Vector2 tl = new Vector2(Pos.X - HalfBounds.X, Pos.Y + HalfBounds.Y), tr = Pos + HalfBounds;
            Vector2 bl = Pos - HalfBounds, br = new Vector2(Pos.X + HalfBounds.X, Pos.Y - HalfBounds.Y);
            
            if ((center - tr).LengthFast < radius || (center - br).LengthFast < radius ||
                    (center - tl).LengthFast < radius || (center - bl).LengthFast < radius)
            {
                return true;
            }

            //check if the center is within an axis-aligned outer area
            if (center.X > tl.X && center.X < br.X)
            {
                if(center.Y > tl.Y)
                {
                    return (new Vector2(center.X - Pos.X, tl.Y - center.Y)).LengthFast < radius;
                }
                else
                {
                    return (new Vector2(center.X - Pos.X, br.Y - center.Y)).LengthFast < radius;
                }
            }
            else if(center.Y < tl.Y && center.Y > br.Y)
            {
                if(center.X < tl.X)
                {
                    return (new Vector2(tl.X - center.X, center.Y - Pos.Y)).LengthFast < radius;
                }
                else
                {
                    return (new Vector2(br.X - center.X, center.Y - Pos.Y)).LengthFast < radius;
                }
            }
            return false;
        }

        private bool testAABB_Circle(CircleCollider other)
        {
            return testAABB_Circle_Internal(other.Pos, other.Radius);
        }

        public override bool TestPoint(Vector2 point)
        {
            return (point.X >= (Pos.X - HalfBounds.X) && point.X <= (Pos.X + HalfBounds.X)) &&
                (point.Y >= (Pos.Y - HalfBounds.Y) && point.Y <= (Pos.Y + HalfBounds.Y));
        }

        internal override bool IsMTCTHandledByThis(ColliderType otherType)
        {
            switch(otherType)
            {
                case ColliderType.CT_AABB:
                    return true;
                case ColliderType.CT_CIRCLE:
                    return true;
                //Opposing collider should treat this as an AABB then an OBB to test
                case ColliderType.CT_OBB:
                    return false;
                //Opposing collider should treat this as another convex
                case ColliderType.CT_CONVEX:
                    return false;
            }
            return false;
        }
    }
}
