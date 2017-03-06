﻿using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;

namespace restep.Core.Collision
{
    public enum ColliderType
    {
        CT_AABB,
        CT_OBB,
        CT_CONVEX,
        CT_CIRCLE
    }

    //internal struct OverlapInfo
    //{
    //    public Vector2 overlap;
    //    public Collider thisCollider, otherCollider;
        
    //    public OverlapInfo(Vector2 overlap, Collider t, Collider o)
    //    {
    //        thisCollider = t;
    //        otherCollider = o;
    //        this.overlap = overlap;
    //    }
    //}

    public abstract class Collider
    {
        public GameObject Owner { get; set; }

        /// <summary>
        /// Center of this collider
        /// </summary>
        public Vector2 Pos { get { return Owner.Position; } }

        /// <summary>
        /// Specifies the collision type of this collider
        /// </summary>
        public abstract ColliderType Type { get; }

        /// <summary>
        /// Returns if multitype collision testing is handled by (this) subclass
        /// </summary>
        /// <param name="otherType">Type of collider to test against</param>
        /// <returns></returns>
        internal abstract bool IsMTCTHandledByThis(ColliderType otherType);

        /// <summary>
        /// Tests collision between two colliders (subclasses MUST handle collision between all other subclasses)
        /// </summary>
        /// <param name="other">Other collider</param>
        /// <returns></returns>
        public abstract bool TestCollision(Collider other);

        /// <summary>
        /// Tests if a point is within the collider
        /// </summary>
        /// <param name="point">Location of the point</param>
        /// <returns></returns>
        public abstract bool TestPoint(Vector2 point);

        public Collider(GameObject owner)
        {
            Owner = owner;
        }
    }
}