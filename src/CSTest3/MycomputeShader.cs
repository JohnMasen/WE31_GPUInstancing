using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Graphics.Effects;
using WaveEngine.Mathematics;
using Buffer = WaveEngine.Common.Graphics.Buffer;

namespace CSTest3
{
    internal class MycomputeShader : ComputeTaskDecorator
    {
        public MycomputeShader(Effect effect) : base(effect) { }
        private ConstantBuffer CBuffer0 => ComputeTask.CBuffers[0];
        public float Time
        {
            get => CBuffer0.GetBufferData<float>();
            set => CBuffer0.SetBufferData<float>(value);
        }

        public int Stride
        {
            get => CBuffer0.GetBufferData<int>(12);
            set => CBuffer0.SetBufferData<int>(value, 12);
        }

        public Buffer Data 
        {
            get => ComputeTask.UABuffers[0].UABuffer;
            set => ComputeTask.UABuffers[0].UABuffer = value;
        }

    }
}
