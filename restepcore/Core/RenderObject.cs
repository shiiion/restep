using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;

namespace restep.Core
{
    public class RenderObject : GameObject
    {
        public bool Invalidated { get; set; } = true;
        
        public override Vector2 Position
        {
            get { return base.Position; }

            set
            {
                Invalidated = true;
                base.Position = value;
            }
        }

        public override float Rotation
        {
            get { return base.Rotation; }

            set
            {
                Invalidated = true;
                base.Rotation = value;
            }
        }

        private ulong activeMeshID;
        public ulong ActiveMeshID
        {
            get { return activeMeshID; }

            set
            {
                Invalidated = true;
                activeMeshID = value;
            }
        }

        public override Vector2 Scale
        {
            get { return base.Scale; }

            set
            {
                Invalidated = true;
                base.Scale = value;
            }
        }

        private Vector2 imageScale;
        public Vector2 ImageScale
        {
            get { return imageScale; }

            set
            {
                Invalidated = true;
                imageScale = value;
            }
        }

        private float transparency;
        public float Transparency
        {
            get { return transparency; }

            set
            {
                Invalidated = true;
                transparency = value;
            }
        }

        public RenderObject(Vector2 imgScale, float lifeTime = -1) : base(lifeTime)
        {
            ImageScale = imgScale;
            transparency = 1;
        }

        public override void DisposeObject()
        {
            Interface.Render.RenderInterface.RemovePair(this);
        }
    }
}
