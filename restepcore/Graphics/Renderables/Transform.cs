﻿using OpenTK;

namespace restep.Graphics.Renderables
{
    /// <summary>
    /// Represents a 2-D transformation, and gives a resulting 3x3 matrix
    /// <para>All shaders will use a 3x3 transformation matrix rather than 4x4 or 2x2</para>
    /// </summary>
    public sealed class Transform
    {
        private Matrix3 tmat;
        private Vector2 translation;
        /// <summary>
        /// The position of the resulting transformation
        /// Modifying this value invalidates the transformation cache
        /// </summary>
        public Vector2 Translation
        {
            get
            {
                return translation;
            }

            set
            {
                translation = value;
                refreshResult = true;
                CreateTranslation(translation - baseTranslation, out tmat);
            }
        }

        private Vector2 baseTranslation;
        public Vector2 BaseTranslation
        {
            get
            {
                return baseTranslation;
            }

            set
            {
                baseTranslation = value;
                refreshResult = true;
                CreateTranslation(translation - baseTranslation, out tmat);
            }
        }

        private Matrix3 rmat;
        private float rotation;
        /// <summary>
        /// The rotation of the resulting transformation
        /// Modifying this value invalidates the transformation cache
        /// </summary>
        public float Rotation
        {
            get
            {
                return rotation;
            }

            set
            {
                rotation = value;
                refreshResult = true;
                //Z axis rotation is equivalent to 2-D matrix rotation
                Matrix3.CreateRotationZ(rotation, out rmat);
            }
        }

        private Matrix3 smat;
        private Vector2 scale;
        private Vector2 baseScale;
        /// <summary>
        /// The scale of the resulting transformation
        /// Modifying this value invalidates the transformation cache
        /// </summary>
        public Vector2 Scale
        {
            get
            {
                return scale;
            }

            set
            {
                scale = value;
                refreshResult = true;
                Matrix3.CreateScale(scale.X * baseScale.X, scale.Y * baseScale.Y, 1, out smat);
            }
        }

        public Vector2 BaseScale
        {
            get
            {
                return baseScale;
            }

            set
            {
                baseScale = value;
                refreshResult = true;
                Matrix3.CreateScale(scale.X * baseScale.X, scale.Y * baseScale.Y, 1, out smat);
            }
        }

        private Matrix3 scrmat;
        private Vector2 screenSpace;
        /// <summary>
        /// Defines the size of the screen
        /// Modifying this value invalidates the transformation cache
        /// </summary>
        public Vector2 ScreenSpace
        {
            get
            {
                return screenSpace;
            }

            set
            {
                screenSpace = value;
                refreshResult = true;
                CreateScreenspace(screenSpace, out scrmat);
            }
        }

        private bool refreshResult;
        private Matrix3 cachedTransform;
        /// <summary>
        /// Get the resultant transformation
        /// </summary>
        public Matrix3 Transformation
        {
            get
            {
                //if our cached matrix is out of date
                if(refreshResult)
                {
                    updateTransformCache();
                }
                return cachedTransform;
            }
        }
        

        public Transform(Vector2 screenSpace)
        {
            Translation = Vector2.Zero;
            Rotation = 0;
            Scale = Vector2.One;
            BaseScale = Vector2.One;
            ScreenSpace = screenSpace;
        }

        private void updateTransformCache()
        {
            //transform order: screenspace * (translation * (rotation * scale))
            cachedTransform = Matrix3.Mult(scrmat, Matrix3.Mult(tmat, Matrix3.Mult(rmat, smat)));
            cachedTransform.Transpose();
            refreshResult = false;
        }

        private void CreateScreenspace(Vector2 screenDims, out Matrix3 outTransform)
        {
            /* 2-D screenspace from matrix 3 (mix of scale and translation)
            [2/W   0    -1]
            [0    2/H   -1]
            [0     0     1]
            */
            outTransform.Row0 = new Vector3((2 / screenDims.X), 0, -1);
            outTransform.Row1 = new Vector3(0, (2 / screenDims.Y), -1);
            outTransform.Row2 = new Vector3(0, 0, 1);
        }

        private void CreateTranslation(Vector2 translate, out Matrix3 outTranslate)
        {
            /* 2-D translation from Matrix3:
            [1     0     X]
            [0     1     Y]
            [0     0     1]
            */
            outTranslate.Row0 = new Vector3(1, 0, translate.X);
            outTranslate.Row1 = new Vector3(0, 1, translate.Y);
            outTranslate.Row2 = new Vector3(0, 0, 1);
        }
    }
}
