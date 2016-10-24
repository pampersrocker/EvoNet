using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvoNet
{
    public class Camera
    {
        Vector2 translation;
        private static Camera instanceGameWorld_;
        public static Camera instanceGameWorld
        {
            get
            {
                if(instanceGameWorld_ == null)
                {
                    instanceGameWorld_ = new Camera();
                }
                return instanceGameWorld_;
            }
        }
        public Vector2 Translation
        {
            get { return translation; }
            set
            {
                translation = value;
                matrixNeedsUpdate = true;
            }
        }
        // In Degrees
        float rotation;
        public float Rotation
        {
            get { return rotation; }
            set
            {
                rotation = value;
                matrixNeedsUpdate = true;
            }
        }
        float scale = 1.0f;
        public float Scale
        {
            get { return scale; }
            set
            {
                scale = value;
                matrixNeedsUpdate = true;
            }
        }
        Matrix cachedMatrix;
        public Matrix Matrix
        {
            get
            {
                ConditionalUpdateMatrix();
                return cachedMatrix;
            }
        }
        bool matrixNeedsUpdate;

        public void ConditionalUpdateMatrix()
        {
            if (matrixNeedsUpdate)
            {
                UpdateMatrix();
            }
        }

        public void Move(Vector2 Delta)
        {
            Translation += Delta;
        }

        public void Rotate(float DeltaRotation)
        {
            Rotation += DeltaRotation;
        }

        public void UpdateMatrix()
        {
            cachedMatrix = Matrix.CreateScale(Scale) *
              Matrix.CreateRotationZ(Rotation * Mathf.DEGREETORAD) *
              Matrix.CreateTranslation(Translation.X, Translation.Y, 0);
            matrixNeedsUpdate = false;
        }


    }
}
