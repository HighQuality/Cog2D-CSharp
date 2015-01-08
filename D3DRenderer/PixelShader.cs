using SlimDX.D3DCompiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Buffer = SlimDX.Direct3D11.Buffer;

namespace D3DRenderer
{
    public class PixelShader : DXResource
    {
        private ShaderSignature signature;
        private ShaderBytecode bytecode;
        internal Buffer[] ConstantBuffers;
        internal SlimDX.Direct3D11.PixelShader Shader;

        public PixelShader(string source, string function, ConstantBuffer[] constantBuffers)
        {
            if (constantBuffers == null)
                constantBuffers = new ConstantBuffer[0];

            bytecode = ShaderBytecode.Compile(source, function, "ps_4_0", ShaderFlags.None, EffectFlags.None);
            signature = ShaderSignature.GetInputSignature(bytecode);
            Shader = new SlimDX.Direct3D11.PixelShader(D3DWindow.Device, bytecode);

            ConstantBuffers = new Buffer[constantBuffers.Length];
            for (int i = 0; i < ConstantBuffers.Length; i++)
                ConstantBuffers[i] = constantBuffers[i].Buffer;
        }

        public override void Dispose(bool disposing)
        {
            if (disposing)
            {
                signature.Dispose();
                bytecode.Dispose();
                Shader.Dispose();
            }
        }
    }
}
