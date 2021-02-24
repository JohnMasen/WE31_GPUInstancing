using System;
using System.Collections.Generic;

using System.Text;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Framework;
using WaveEngine.Mathematics;
using System.Linq;
using WaveEngine.Framework.Graphics;

namespace TestLib
{
    public class ItemGenerator : Component
    {
        [BindComponent(false, true)]
        public MaterialComponent MaterialSource { get; set; }

        [BindComponent(false, true)]
        public MeshComponent MeshSource { get; set; }

        private Vector3 size;

        public Vector3 Size
        {
            get { return size; }
            set { size = value; refreshItems(); }
        }


        private const string TAG_GENERATED = "generated";
        Random r = new Random();
        private int itemsCount;

        public int ItemsCount
        {
            get { return itemsCount; }
            set { itemsCount = value; refreshItems(); }
        }

        protected override bool ShouldBeActivated => MaterialSource?.Material != null && MeshSource.Model != null;

        protected override bool OnAttached()
        {
            return base.OnAttached();
        }
        protected override void Start()
        {
            base.Start();
            refreshItems();
        }
        private void refreshItems()
        {
            if (Application.Current.IsEditor)
            {
                return;
            }
            if (this.IsActivated)
            {
                clearItems();
                generateItems();
            }
        }

        private void generateItems()
        {
            for (int i = 0; i < ItemsCount; i++)
            {
                Owner.AddChild(generateItem());
            }
        }

        private Entity generateItem()
        {
            Entity result = new Entity() { Tag = TAG_GENERATED };
            result.Flags = HideFlags.DontSave | HideFlags.DontShow;
            result.AddComponent(new Transform3D());
            result.AddComponent(new MeshComponent() { Model = MeshSource.Model });
            result.AddComponent(new MaterialComponent() { Material = MaterialSource.Material });
            result.AddComponent(new MeshRenderer());
            result.AddComponent(new ItemMover() {Speed=2f,TargetPosition= generateRandomVector3(-Size * 0.5f, Size * 0.5f) });
            return result;
        }



        private Vector3 generateRandomVector3(Vector3 min, Vector3 max)
        {
            return new Vector3(genRandomF(min.X, max.X), genRandomF(min.Y, max.Y), genRandomF(min.Z, max.Z));
        }

        float genRandomF(float min, float max)
        {
            return MathHelper.Lerp(min, max, (float)r.NextDouble());
        }
        protected void clearItems()
        {
            Owner.ChildEntities.Where(x => x.Tag == TAG_GENERATED).Select(x => x.Id).ToList().ForEach(id => Owner.RemoveChild(id));
        }

        public IEnumerable<Entity> GeneratedChildren
        {
            get
            { return Owner.FindChildrenByTag(TAG_GENERATED); }
        }

    }
}
