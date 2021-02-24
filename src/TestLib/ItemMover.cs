using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Mathematics;

namespace TestLib
{
    public class ItemMover : Behavior
    {
        [BindComponent(false, true)]
        public Transform3D OwnerTransform { get; set; }
        private Vector3 target;
        public Vector3 TargetPosition
        {
            get
            {
                return target;
            }
            set
            {
                target = value;
                IsInposition = false;
                
            }
        }

        public float Speed { get; set; }

        public bool IsInposition { get; private set; } = false;
        public float MinDistance { get; set; } = 0.1f;


        protected override void Update(TimeSpan gameTime)
        {
            if (IsInposition)
            {
                return;
            }
            float md2 = Speed * 0.01f;
            if (Vector3.DistanceSquared(OwnerTransform.LocalPosition, TargetPosition) <= md2)
            {
                OwnerTransform.LocalPosition = TargetPosition;
                IsInposition = true;
            }
            else
            {
                Vector3 dir = Vector3.Normalize(TargetPosition - OwnerTransform.LocalPosition);
                OwnerTransform.LocalPosition += dir*Speed*(float)gameTime.TotalSeconds;
                OwnerTransform.LocalLookAt(TargetPosition, Vector3.Up);
            }
        }
    }
}
