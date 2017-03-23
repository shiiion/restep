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

        private float rotationSave;

        internal override void UpdateBBox()
        {
            if (rotationSave != Rotation)
            {
                rotationSave = Rotation;
                Vector2 tl = getRotatedPoint(new Vector2(Pos.X - HalfBounds.X, Pos.Y + HalfBounds.Y), 1), tr = getRotatedPoint(Pos + HalfBounds, 1);
                Vector2 bl = getRotatedPoint(Pos - HalfBounds, 1), br = getRotatedPoint(new Vector2(Pos.X + HalfBounds.X, Pos.Y - HalfBounds.Y), 1);

                Func<float, float, float> max = (lf, rt) => (lf > rt ? lf : rt);
                Func<float, float, float> min = (lf, rt) => (lf < rt ? lf : rt);

                float r = max(max(max(tl.X, tr.X), bl.X), br.X);
                float l = min(min(min(tl.X, tr.X), bl.X), br.X);
                float t = max(max(max(tl.Y, tr.Y), bl.Y), br.Y);
                float b = min(min(min(tl.Y, tr.Y), bl.Y), br.Y);

                BBox.HalfBounds = new Vector2((r - l) / 2.0f, (t - b) / 2.0f);
            }
        }

        public OBBCollider(GameObject owner, Vector2 bounds) : base(owner, bounds, false)
        {
            rotationSave = Rotation + 1;
            BBox = new AABBCollider(owner, bounds, false);
            UpdateBBox();
        }

        /// <summary>
        /// Rotates a point about the center of this OBB clockwise
        /// </summary>
        /// <returns>Point rotated about the center of this OBB</returns>
        private Vector2 getRotatedPoint(Vector2 point, int direction)
        {
            Vector2 relLoc = point - Pos;

            float cosf = (float)Math.Cos(-Rotation * direction);
            float sinf = (float)Math.Sin(-Rotation * direction);

            Vector2 relRot = new Vector2(relLoc.X * cosf - relLoc.Y * sinf, relLoc.X * sinf + relLoc.Y * cosf);

            return relRot + Pos;
        }

        public override bool TestOverlap(Collider other)
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
                    return other.TestOverlap(this);
            }

            return false;
        }

        private bool testOBB_AABB(AABBCollider other)
        {
            {
                Vector2 tl = getRotatedPoint(new Vector2(Pos.X - HalfBounds.X, Pos.Y + HalfBounds.Y), 1), tr = getRotatedPoint(Pos + HalfBounds, 1);
                Vector2 bl = getRotatedPoint(Pos - HalfBounds, 1), br = getRotatedPoint(new Vector2(Pos.X + HalfBounds.X, Pos.Y - HalfBounds.Y), 1);

                if (other.TestPoint(tl) || other.TestPoint(tr) || other.TestPoint(bl) || other.TestPoint(br))
                {
                    return true;
                }
            }

            {
                Vector2 tl = new Vector2(other.Pos.X - other.HalfBounds.X, other.Pos.Y + other.HalfBounds.Y), tr = other.Pos + other.HalfBounds;
                Vector2 bl = other.Pos - other.HalfBounds, br = new Vector2(other.Pos.X + other.HalfBounds.X, other.Pos.Y - other.HalfBounds.Y);

                if (TestPoint(tl) || TestPoint(tr) || TestPoint(bl) || TestPoint(br))
                {
                    return true;
                }
            }
            return false;
        }

        private bool testOBB_OBB(OBBCollider other)
        {
            {
                Vector2 tl = new Vector2(Pos.X - HalfBounds.X, Pos.Y + HalfBounds.Y), tr = Pos + HalfBounds;
                Vector2 bl = Pos - HalfBounds, br = new Vector2(Pos.X + HalfBounds.X, Pos.Y - HalfBounds.Y);

                tl = getRotatedPoint(tl, 1);
                tr = getRotatedPoint(tr, 1);
                bl = getRotatedPoint(bl, 1);
                br = getRotatedPoint(br, 1);

                if (other.TestPoint(tl) || other.TestPoint(tr) || other.TestPoint(bl) || other.TestPoint(br))
                {
                    return true;
                }
            }

            {
                Vector2 tl = new Vector2(other.Pos.X - other.HalfBounds.X, other.Pos.Y + other.HalfBounds.Y), tr = other.Pos + other.HalfBounds;
                Vector2 bl = other.Pos - other.HalfBounds, br = new Vector2(other.Pos.X + other.HalfBounds.X, other.Pos.Y - other.HalfBounds.Y);

                tl = getRotatedPoint(tl, 1);
                tr = getRotatedPoint(tr, 1);
                bl = getRotatedPoint(bl, 1);
                br = getRotatedPoint(br, 1);

                if (TestPoint(tl) || TestPoint(tr) || TestPoint(bl) || TestPoint(br))
                {
                    return true;
                }
            }
            return false;
        }

        private bool testOBB_Circle(CircleCollider other)
        {
            Vector2 rotatedCenter = getRotatedPoint(other.Pos, -1);

            return testAABB_Circle_Internal(rotatedCenter, other.Radius);
        }

        public override bool TestPoint(Vector2 point)
        {
            return base.TestPoint(getRotatedPoint(point, -1));
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
