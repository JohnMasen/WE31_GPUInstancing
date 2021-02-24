using WaveEngine.Common.Graphics;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Managers;
using WaveEngine.Framework.Services;
using WaveEngine.Mathematics;

namespace CSTest3
{
    public class MyScene : Scene
    {
		public override void RegisterManagers()
        {
        	base.RegisterManagers();
        	this.Managers.AddManager(new WaveEngine.Bullet.BulletPhysicManager3D());
            Managers.FindManager<RenderManager>().FindRenderFeature<MeshRenderFeature>().RegisterMeshProcessor(new MyInstancingMeshProcessor());
            //var r = Managers.FindManager<RenderManager>();
            //var feature = r.FindRenderFeature<MeshRenderFeature>();
            //var ir = new MyInstancingMeshProcessor();
            //feature.RegisterMeshProcessor(ir);

        }

        protected override void CreateScene()
        {
        }
    }

    public class myspy: WaveEngine.Framework.Graphics.Instancing.InstancingMeshProcessor
    {
        public override bool CouldBatch(RenderObjectInfo objectInfo1, RenderObjectInfo objectInfo2)
        {
            return base.CouldBatch(objectInfo1, objectInfo2);
        }
    }
}