using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using WaveEngine.Common.Graphics;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Graphics.Instancing;
using WaveEngine.Mathematics;
using System.Linq;
namespace CSTest3
{
    internal struct InstancingBatchSetting
    {
        public int TransformOffset;
        public uint InstanceCount;
        public MyInstancingBatchRenderUnit RenderMesh;
    }
    public class MyInstancingMeshProcessor: MeshProcessor
    {
        private const int DefaultWorldTransformCount = 32;
        private static readonly uint MatrixSize = (uint)Unsafe.SizeOf<Matrix4x4>();
        /// <summary>The world transform array.</summary>
        public Matrix4x4[] worldTransforms;
        /// <summary>The world transform count.</summary>
        public int worldTransformCount;
        private const int DefaultBatchCount = 32;
        private InstancingBatchSetting[] batches;
        private int batchCount;
        private WaveEngine.Common.Graphics.Buffer instanceBuffer;
        private uint bufferCapacity;
        /// <summary>Indicates if the buffer is dirty.</summary>
        public bool BufferDirty;

        /// <summary>
        /// Gets the priority of this processor. Processors with higher values will be executed earlier.
        /// </summary>
        public override float Priority => 2f;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:WaveEngine.Framework.Graphics.Instancing.InstancingMeshProcessor" /> class.
        /// </summary>
        public MyInstancingMeshProcessor()
        {
            this.worldTransforms = new Matrix4x4[32];
            this.batches = new InstancingBatchSetting[32];
        }

        /// <inheritdoc />
        public override void Reset()
        {
            this.batchCount = 0;
            this.worldTransformCount = 0;
        }

        /// <inheritdoc />
        public override bool CouldRenderObject(RenderObjectInfo renderObjectInfo) => false;

        /// <inheritdoc />
        public override bool CouldBatch(RenderObjectInfo objectInfo1, RenderObjectInfo objectInfo2)
        {
            return objectInfo1 is RenderMeshInfo renderMeshInfo1 && objectInfo2 is RenderMeshInfo renderMeshInfo2 && (renderMeshInfo1.Material.AllowInstancing && renderMeshInfo1.Material == renderMeshInfo2.Material) && renderMeshInfo1.Mesh == renderMeshInfo2.Mesh;
        }

        /// <inheritdoc />
        public override RenderUnit EmitBatch(
          int startIndex,
          int elementCount,
          DrawContext drawContext,
          MeshProcessor.RenderObjectInfoAccessor renderObjectInfoAccessor)
        {
            ArrayHelpers.EnsureCapacityPo2<InstancingBatchSetting>(ref this.batches, this.batchCount + 1);
            ArrayHelpers.EnsureCapacityPo2<Matrix4x4>(ref this.worldTransforms, this.worldTransformCount + elementCount);
            ref InstancingBatchSetting local = ref this.batches[this.batchCount++];
            local.InstanceCount = (uint)elementCount;
            local.TransformOffset = this.worldTransformCount;

            RenderMeshInfo renderMeshInfo = renderObjectInfoAccessor(startIndex) as RenderMeshInfo;
            this.worldTransforms[this.worldTransformCount++] = Matrix4x4.Transpose(renderMeshInfo.Transform);

            for (int index = 1; index < elementCount; ++index)
            {
                var m= renderObjectInfoAccessor(startIndex + index).Transform;
                this.worldTransforms[this.worldTransformCount++] = Matrix4x4.Transpose(m); 
            }
                
            
            local.RenderMesh = local.RenderMesh ?? new MyInstancingBatchRenderUnit();
            //renderMeshInfo.Material.TextureSlots[10].StructuredBuffer = this.instanceBuffer;
            local.RenderMesh.Info = renderMeshInfo;
            local.RenderMesh.InstanceCount = (uint)elementCount;
            local.RenderMesh.InstancingWorldOffset = local.TransformOffset;
            return (RenderUnit)local.RenderMesh;
        }

        /// <inheritdoc />
        public override unsafe void Collect()
        {
            base.Collect();
            if (this.batchCount == 0)
                return;
            uint newBufferCapacity = (uint)this.worldTransformCount * MyInstancingMeshProcessor.MatrixSize;
            this.EnsureBufferCapacity(newBufferCapacity);

            for (int index = 0; index < this.batchCount; ++index)
            {
                this.batches[index].RenderMesh.InstancingWorld = this.worldTransforms;
                batches[index].RenderMesh.Info.Material.TextureSlots[10].StructuredBuffer = this.instanceBuffer;
                if (BufferDirty)
                {
                    foreach (var item in batches[index].RenderMesh.Info.MaterialResources.Values)
                    {
                        foreach (var pr in item.PassResources)
                        {
                            pr?.MakeResourceSetDirty();
                        }
                    }
                }
            }
            void* pointer = this.GraphicsContext.MapMemory((GraphicsResource)this.instanceBuffer, MapMode.Write).Data.ToPointer();
            fixed (Matrix4x4* matrix4x4Ptr = &this.worldTransforms[0])
            {
                int num = (int)newBufferCapacity;
                Unsafe.CopyBlock(pointer, (void*)matrix4x4Ptr, (uint)num);
            }
            this.GraphicsContext.UnmapMemory((GraphicsResource)this.instanceBuffer);
        }

        /// <inheritdoc />
        protected override void Destroy()
        {
            base.Destroy();
            this.DestroyBuffers();
            Array.Clear((Array)this.batches, 0, this.batches.Length);
        }

        /// <summary>
        /// Ensure that the GPU buffer has the specified capacity.
        /// </summary>
        /// <param name="newBufferCapacity">The capacity.</param>
        private void EnsureBufferCapacity(uint newBufferCapacity)
        {
            if (this.bufferCapacity >= newBufferCapacity)
                return;
            this.DestroyBuffers();
            this.bufferCapacity = newBufferCapacity;
            BufferDescription description = new BufferDescription(this.bufferCapacity, BufferFlags.BufferStructured| BufferFlags.ShaderResource, ResourceUsage.Dynamic,cpuAccess:ResourceCpuAccess.Write);
            description.StructureByteStride = (int)MatrixSize;
            this.instanceBuffer = this.GraphicsContext.Factory.CreateBuffer(ref description, "worldTransforms");
            this.BufferDirty = true;
        }

        /// <summary>Destroy all buffers.</summary>
        private void DestroyBuffers() => this.instanceBuffer?.Dispose();
    }
}

