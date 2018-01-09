using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;
using restep.Core.Collision;


namespace restep.Core
{
    public delegate void OverlapEvent(GameObject other);
    public delegate void ObjectTickEvent(double dt);

    public class GameObject
    {
        private static ulong OBJECT_COUNTER = 0;

        /// <summary>
        /// Location of the object. The location of the collider is linked with this
        /// </summary>
        public virtual Vector2 Position { get; set; }

        /// <summary>
        /// Rotation of the object. This is only used for colliders of type convex and OBB
        /// </summary>
        public virtual float Rotation { get; set; }

        /// <summary>
        /// Angular velocity of the object.
        /// </summary>
        public virtual float AngularVelocity { get; set; }

        /// <summary>
        /// Scale of the object. This is linked to the bounds of several colliders (excluding CircleCollider) 
        /// </summary>
        public virtual Vector2 Scale { get; set; }

        /// <summary>
        /// Direction and speed of the object
        /// </summary>
        public virtual Vector2 Velocity { get; set; }

        /// <summary>
        /// The collider defining the collision bounds of this object
        /// </summary>
        internal Collider ObjectCollider { get; set; }

        public double SpawnTime { get; set; }
        public double LifeTime { get; set; }

        public bool Destroy { get; set; }

        /// <summary>
        /// Tells whether or not this GameObject has a collider
        /// </summary>
        public bool HasCollider
        {
            get
            {
                return ObjectCollider != null;
            }

            set
            {
                if(!value)
                {
                    ObjectCollider = null;
                }
            }
        }

        /// <summary>
        /// Unique ID of this object
        /// </summary>
        public ulong ObjectID { get; protected set; }

        public event OverlapEvent Overlap;

        public event ObjectTickEvent Tick;

        public GameObject(double lifeTime = -1)
        {
            LifeTime = lifeTime;
            ObjectID = OBJECT_COUNTER;
            OBJECT_COUNTER++;

            Scale = Vector2.One;
        }

        /// <summary>
        /// Adds a circle collider to this object
        /// </summary>
        /// <param name="radius">The radius of the circle</param>
        public void AddCircleCollider(float radius)
        {
            ObjectCollider = new CircleCollider(this, radius);
        }

        /// <summary>
        /// Adds an AABB collider to this object
        /// </summary>
        /// <param name="halfBounds">The width and height of the AABB / 2</param>
        public void AddAABBCollider(Vector2 halfBounds)
        {
            ObjectCollider = new AABBCollider(this, halfBounds, true);
        }

        /// <summary>
        /// Adds an OBB collider to this object
        /// </summary>
        /// <param name="halfBounds">The width and height of the OBB / 2</param>
        public void AddOBBCollider(Vector2 halfBounds)
        {
            ObjectCollider = new OBBCollider(this, halfBounds);
        }

        //TODO
        public void AddConvexCollider()
        {

        }

        public bool TestCollision(GameObject other)
        {
            return other.HasCollider && ObjectCollider.TestOverlap(other.ObjectCollider);
        }

        internal virtual void OnOverlap(GameObject other)
        {
            Overlap?.Invoke(other);
        }

        public bool BoundsOverlap(GameObject other)
        {
            if(ObjectCollider == null || other.ObjectCollider == null)
            {
                return false;
            }

            ObjectCollider.UpdateBBox();
            other.ObjectCollider.UpdateBBox();
            return other.HasCollider && ObjectCollider.BBox.TestOverlap(other.ObjectCollider.BBox);
        }

        public void TickObject(double dt)
        {
            Tick?.Invoke(dt);
            if(LifeTime > 0 && CoreThread.Instance.GetEngineTime() > SpawnTime + LifeTime)
            {
                Destroy = true;
            }
        }

        public virtual void DisposeObject()
        {
        }
    }
}
