using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Graphics.VertexFormats;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Components.Primitives;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Graphics.Effects;
using WaveEngine.Mathematics;

namespace CSTest3
{
    [StructLayout(LayoutKind.Explicit)]
    public struct VPData
    {
        [FieldOffset(0)]
        /// <summary>Vertex position.</summary>
        public Vector3 Position;
        /// <summary>Vertex normal.</summary>
        [FieldOffset(12)]
        public Vector3 Normal;
        /// <summary>Vertex normal.</summary>
        [FieldOffset(24)]
        public Vector3 Tangent;
        /// <summary>Vertex color.</summary>
        [FieldOffset(28)]
        public Color Color;
        /// <summary>Vertex texture coordinate.</summary>
        [FieldOffset(36)]
        public Vector2 TexCoord;
        [FieldOffset(44)]
        /// <summary>Vertex texture coordinate.</summary>
        public Vector2 TexCoord2;
    }
        public class ComputeMeshTest : PlaneMesh
    {
        private Effect computeEffect;
        private ComputeTask ct;
        public Effect ComputeEffect
        {
            get { return computeEffect; }
            set
            {
                computeEffect = value;
                refreshComputeTask();
            }
        }

        protected override bool OnAttached()
        {
            var v = base.OnAttached();
            var p = new WaveEngine.Components.Primitives.Plane(this.Normal, this.Width, this.Height, this.TwoSides, this.UVHorizontalFlip, this.UVVerticalFlip, this.UTile, this.YTile, this.InitialU, this.InitialV);
            var xxx = p.CreateMesh();
            var mesh = this.Meshes[0];
            var x = this.Meshes[0].VertexBuffers[0];
            var offset = this.Meshes[0].VertexOffset;
            mesh.InputLayouts.FindLayoutElementByUsage(ElementSemanticType.Position, 0, out ElementDescription ed, out int bufferIndex);
            //mesh.VertexBuffers[0].Data
            //var desc=Meshes[0].VertexBuffers[0].LayoutDescription.Elements[0].
            ReadOnlySpan<VertexPositionNormalTangentColorDualTexture> s;

            //unsafe
            //{
            //    s = new ReadOnlySpan<VertexPositionNormalTangentColorDualTexture>((void*)x.Data, x.VertexCount);
            //}
            s = extractVBufferData<VertexPositionNormalTangentColorDualTexture>(x);
            foreach (var item in s)
            {
                Debug.WriteLine(item);
            }
            return v;
        }

        private Span<T> extractVBufferData<T>(VertexBuffer vb)
        {
            if (Marshal.SizeOf<T>()!=vb.Size/vb.VertexCount)
            {
                throw new ArgumentException($"size of type {typeof(T).Name} does not equale to stride, stride={vb.Size / vb.VertexCount}, {typeof(T).Name} size ={Marshal.SizeOf<T>()} ");
            }
            unsafe
            {
                return new Span<T>((void*)(vb.Data + vb.Offset), vb.VertexCount);
            }
            
        }

        private void refreshComputeTask()
        {
            if (computeEffect == null)
            {
                return;
            }
            ct = new ComputeTask(ComputeEffect);
        }

        protected override void RefreshMesh()
        {
            base.RefreshMesh();
        }
    }
}
