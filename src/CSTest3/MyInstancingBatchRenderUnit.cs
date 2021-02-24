using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Common.Graphics;
using WaveEngine.Framework.Graphics;
using WaveEngine.Mathematics;

namespace CSTest3
{
    public class MyInstancingBatchRenderUnit: MeshRenderUnit
    {
        /// <summary>The instancing world array.</summary>
        public Matrix4x4[] InstancingWorld;
        /// <summary>The instancing world offset.</summary>
        public int InstancingWorldOffset;
        private uint[] dynamicCBufferOffsets;
        /// <summary>
        /// Initializes a new instance of the <see cref="T:WaveEngine.Framework.Graphics.Instancing.InstancingBatchRenderUnit" /> class.
        /// </summary>
        /// <param name="instanceCount">The instance count.</param>
        public MyInstancingBatchRenderUnit(uint instanceCount = 1)
          : base(instanceCount)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:WaveEngine.Framework.Graphics.Instancing.InstancingBatchRenderUnit" /> class.
        /// </summary>
        /// <param name="info">The mesh info.</param>
        /// <param name="instanceCount">The instance count.</param>
        public MyInstancingBatchRenderUnit(RenderMeshInfo info, uint instanceCount = 1)
          : base(info, instanceCount)
        {
        }

        /// <inheritdoc />
        public override bool PassIsAvailable(DrawContext drawContext, int passId) => this.Info.MaterialResources[drawContext.ID].PassIsAvailable(passId);

        /// <inheritdoc />
        public override void Prepare(CommandBuffer commandBuffer, DrawContext drawContext)
        {
            base.Prepare(commandBuffer, drawContext);
        }

        /// <inheritdoc />
        public override void Render(CommandBuffer commandBuffer, DrawContext drawContext, int passId = 0)
        {
            MaterialResourcesCacheEntry materialResource = this.Info.MaterialResources[drawContext.ID];
            GraphicsPipelineState pipeline = materialResource.Pipelines[passId];
            commandBuffer.SetGraphicsPipelineState(pipeline);
            //force refresh resource
            //materialResource.PassResources[passId].MakeResourceSetDirty();
            
            ResourceSet resourceSet = materialResource.PassResources[passId].ResourceSet;
            commandBuffer.SetResourceSet(resourceSet, constantBufferOffsets: this.dynamicCBufferOffsets);
            commandBuffer.SetVertexBuffers(this.Info.Mesh.Buffers, this.Info.Mesh.Offsets);
            uint instanceCount = this.InstanceCount * (uint)drawContext.MultiviewEyeCount;
            IndexBuffer indexBuffer = this.Info.Mesh.IndexBuffer;
            if (indexBuffer != null)
            {
                commandBuffer.SetIndexBuffer(indexBuffer.Buffer, indexBuffer.IndexFormat, (uint)this.Info.Mesh.IndexBuffer.Offset);
                
                if (instanceCount > 1U)
                    commandBuffer.DrawIndexedInstanced((uint)this.Info.Mesh.ElementCount, instanceCount, (uint)this.Info.Mesh.IndexOffset, (uint)this.Info.Mesh.VertexOffset);
                else
                    commandBuffer.DrawIndexed((uint)this.Info.Mesh.ElementCount, (uint)this.Info.Mesh.IndexOffset, (uint)this.Info.Mesh.VertexOffset);
            }
            else if (instanceCount > 1U)
                commandBuffer.DrawInstanced((uint)this.Info.Mesh.ElementCount, instanceCount, (uint)this.Info.Mesh.VertexOffset);
            else
                commandBuffer.Draw((uint)this.Info.Mesh.ElementCount, (uint)this.Info.Mesh.VertexOffset);
        }
    }
}
