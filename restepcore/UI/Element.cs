using OpenTK;
using System;
using System.Collections.Generic;
using System.Text;
using restep.Core;

namespace restep.UI
{
    public enum Anchoring
    {
        TopLeft = 0,
        TopCenter = 1,
        TopRight = 2,
        LeftCenter = 3,
        Center = 4,
        RightCenter = 5,
        BottomLeft = 6,
        BottomCenter = 7,
        BottomRight = 8,
        /// <summary>
        /// The anchoring is based off of Vector2 CustomAnchor location
        /// OR
        /// The origin is based off of Vector2 CustomOrigin location
        /// </summary>
        CustomAnchor = 9
    }

    /// <summary>
    /// Base UI Element, no visual part, abstract
    /// </summary>
    abstract class Element
    {
        public List<Element> Children { get; set; }

        public Element Parent { get; set; };

        /// <summary>
        /// Offset from parent's anchor and this origin
        /// </summary>
        public Vector2 Location { get; set; }
        
        public Anchoring Anchor { get; set; }
        public Anchoring Origin { get; set; }

        public Vector2 CustomAnchor { get; set; }
        public Vector2 CustomOrigin { get; set; }

        protected GameObject obj;

        public Vector2 AbsoluteLocation { get { return getAbsoluteLocation(); } }

        public Element(Element parent, Vector2 location = default(Vector2))
        {
            Location = location;
            Parent = parent;
        }

        public void AddChild(Element child)
        {
            Children.Add(child);
            child.Parent = this;
        }

        private Vector2 GetOffset()
        {
            if (Parent == null)
            {
                return Vector2.Zero;
            }
            switch (Anchor)
            {
                default:
                case Anchoring.TopLeft:
                    return GetOffset(Vector2.Zero);
                case Anchoring.TopCenter:
                    return GetOffset(new Vector2(Parent.Size.X / 2.0));
                case Anchoring.TopRight:
                    return GetOffset(new Vector2(Parent.Size.X));
                case Anchoring.LeftCenter:
                    return GetOffset(new Vector2(0, Parent.Size.Y / 2.0));
                case Anchoring.Center:
                    return GetOffset(new Vector2(Parent.Size.X / 2.0, Parent.Size.Y / 2.0));
                case Anchoring.RightCenter:
                    return GetOffset(new Vector2(Parent.Size.X, Parent.Size.Y / 2.0));
                case Anchoring.BottomLeft:
                    return GetOffset(new Vector2(0, Parent.Size.Y));
                case Anchoring.BottomCenter:
                    return GetOffset(new Vector2(Parent.Size.X / 2.0, Parent.Size.Y));
                case Anchoring.BottomRight:
                    return GetOffset(new Vector2(Parent.Size.X, Parent.Size.Y));
                case Anchoring.CustomAnchor:
                    return GetOffset(Parent.CustomAnchor);
            }
        }

        private Vector2 GetOffset(Vector2 anchorOffset)
        {
            switch (Origin)
            {
                default:
                case Anchoring.TopLeft:
                    return anchorOffset;
                case Anchoring.TopCenter:
                    return anchorOffset - new Vector2(Size.X / 2.0);
                case Anchoring.TopRight:
                    return anchorOffset - new Vector2(Size.X);
                case Anchoring.LeftCenter:
                    return anchorOffset - new Vector2(0, Size.Y / 2.0);
                case Anchoring.Center:
                    return anchorOffset - new Vector2(Size.X / 2.0, Size.Y / 2.0);
                case Anchoring.RightCenter:
                    return anchorOffset - new Vector2(Size.X, Size.Y / 2.0);
                case Anchoring.BottomLeft:
                    return anchorOffset - new Vector2(0, Size.Y);
                case Anchoring.BottomCenter:
                    return anchorOffset - new Vector2(Size.X / 2.0, Size.Y);
                case Anchoring.BottomRight:
                    return anchorOffset - new Vector2(Size.X, Size.Y);
                case Anchoring.CustomAnchor:
                    return anchorOffset - CustomOrigin;
            }
        }

        private Vector2 getAbsoluteLocation()
        {
            if(Parent == null)
            {
                return Location;
            }
            else
            {
                return getOffset() + Location;
            }
        }
    }
}
