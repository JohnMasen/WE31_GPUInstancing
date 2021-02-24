using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using WaveEngine.Common.Graphics;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Graphics.Effects;
using WaveEngine.Framework.Services;
using WaveEngine.Mathematics;
using Buffer = WaveEngine.Common.Graphics.Buffer;

namespace CSTest3
{
    public class ComputeMeshRenderer : Drawable3D
    {
        private List<IDisposable> disposables = new List<IDisposable>();

        [BindService(true)]
        private GraphicsContext context;

        [BindService(true)]
        private GraphicsPresenter graphicsPresenter;

        private Effect cs;
        private MycomputeShader ct;
        public Effect ComputeShader
        {
            get
            {
                return cs;
            }
            set
            {
                if (cs != value)
                {
                    cs = value;
                    refreshResources();
                }
            }
        }
        [BindComponent(false,true)]
        private MeshComponent model;

        private MaterialComponent material;
        [BindComponent(false,true)]
        public MaterialComponent Material
        {
            get
            {
                return material; 
            }
            set 
            { 
                material = value;
                refreshResources();
            }
        }


        public MeshComponent Model
        {
            get { return model; }
            set { model = value; }
        }
        Vector3[] tmp = new Vector3[64];
        
        Buffer dataBuffer;
        public override void Draw(DrawContext drawContext)
        {
            var buffer = graphicsPresenter.GraphicsCommandQueue.CommandBuffer();
            buffer.Begin();

            ct.Run(buffer, 8, 1, 1);
            //buffer.DrawInstanced
            material.Material.Prepare(buffer);
            // add mesh render support from MeshRenderUnit.cs
            //buffer.SetVertexBuffer(0, model.Meshes[0].VertexBuffers[0].Buffer, 0);
            //buffer.SetIndexBuffer( model.Meshes[0].IndexBuffer.Buffer, model.Meshes[0].IndexBuffer.IndexFormat,(uint)model.Meshes[0].IndexBuffer.Offset);
            
            buffer.End();
            buffer.Commit();
            graphicsPresenter.GraphicsCommandQueue.Submit();
            graphicsPresenter.GraphicsCommandQueue.WaitIdle();

            
        }

        private void refreshResources()
        {
            
            closeExistingResources();
            if (material==null || model==null || cs==null||context==null)
            {
                return;
            }
            
            ct = new MycomputeShader(cs);
            traceDisposable(ct.ComputeTask);
            
            BufferDescription bd = new BufferDescription();
            bd.CpuAccess = ResourceCpuAccess.None;
            bd.Usage = ResourceUsage.Default;
            bd.Flags = BufferFlags.ShaderResource | BufferFlags.UnorderedAccess | BufferFlags.BufferStructured;
            
            bd.StructureByteStride = 12;
            bd.SizeInBytes =(uint) (tmp.Length* bd.StructureByteStride);
            dataBuffer= context.Factory.CreateBuffer<Vector3>(null, ref bd);
            
            ct.Time = 2f;
            ct.Stride = 8;
            
            ct.Data = dataBuffer;
            
            //Buffer=
            //ct.TextureSlots

        }
        protected override void Start()
        {
            base.Start();
            refreshResources();
        }

        private void closeExistingResources()
        {
            disposables.ForEach(x => x.Dispose());
            disposables.Clear();
        }

        private void traceDisposable(IDisposable obj)
        {
            disposables.Add(obj);
        }
        public override void Destroy()
        {
            closeExistingResources();
            base.Destroy();
        }
    }
}
