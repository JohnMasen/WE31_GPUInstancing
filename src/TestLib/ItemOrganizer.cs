using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Framework;
using WaveEngine.Mathematics;
using System.Linq;
using System.Threading;
using TestLib;

namespace TestLib
{
    public class ItemOrganizer : Behavior
    {
        [BindComponent(false,true)]
        public ItemGenerator Generator { get; set; }
        Random r = new Random();
        protected override void Update(TimeSpan gameTime)
        {
            
            
            Vector3 min = -Generator.Size * 0.5f;
            Vector3 max = -min;
            int  count = Generator.ItemsCount;
            int index = 0;
            Generator.GeneratedChildren.AsParallel().ForAll(item =>
            {
                int i = Interlocked.Increment(ref index);
                var mover = item.FindComponent<ItemMover>();
                if (mover == null)
                {
                    return;
                }
                if (mover.IsInposition)
                {
                    //mover.TargetPosition = generateRandomVector3(min, max);
                    mover.TargetPosition = knotPosition(i, count, 0.03f);
                }
            });
            
            //foreach (var item in Generator.GeneratedChildren)
            //{
               
            //}
        }
        private Vector3 knotPosition(int index,int count,float scale)
        {
            float t = MathHelper.Lerp(0f, MathHelper.TwoPi, (float)index / (float)count);
            float a = (float)Math.Sin(t);
            float b = (float)Math.Cos(t);
            float c = (float)Math.Sin(2 * t) ;
            float d = (float)Math.Cos(2 * t) ;
            float e = (float)Math.Sin(3 * t) ;
            float f = (float)Math.Cos(3 * t);

            return new Vector3(
                x: 32 * b - 51 * a - 104 * d - 35 * c + 104 * f - 91 * e,
                y: 94 * b + 41 * a + 113 * d - 68 * f - 124 * e,
                z: 16 * b + 73 * a - 211 * d - 39 * c - 99 * f - 21 * e
                ) * scale;
        }
        private Vector3 generateRandomVector3(Vector3 min, Vector3 max)
        {
            return new Vector3(genRandomF(min.X, max.X), genRandomF(min.Y, max.Y), genRandomF(min.Z, max.Z));
        }

        float genRandomF(float min, float max)
        {
            return MathHelper.Lerp(min, max, (float)r.NextDouble());
        }
    }
}
