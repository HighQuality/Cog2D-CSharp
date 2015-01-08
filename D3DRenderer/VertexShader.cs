using SlimDX.D3DCompiler;
using SlimDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Buffer = SlimDX.Direct3D11.Buffer;

namespace D3DRenderer
{
    public class VertexShader : DXResource
    {
        private ShaderSignature signature;
        private ShaderBytecode bytecode;
        internal InputElement[] InputElements;
        internal InputLayout InputLayout;
        internal Buffer[] ConstantBuffers;
        internal SlimDX.Direct3D11.VertexShader Shader;

        public VertexShader(string source, string function, ConstantBuffer[] constantBuffers, InputElement[] inputElements)
        {
            if (constantBuffers == null)
                constantBuffers = new ConstantBuffer[0];
            if (inputElements == null)
                inputElements = new InputElement[0];

            this.InputElements = inputElements;

            bytecode = ShaderBytecode.Compile(source, function, "vs_4_0", ShaderFlags.None, EffectFlags.None);
            signature = ShaderSignature.GetInputSignature(bytecode);
            InputLayout = new InputLayout(D3DWindow.Device, bytecode, inputElements);
            Shader = new SlimDX.Direct3D11.VertexShader(D3DWindow.Device, bytecode);

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
                InputLayout.Dispose();
                Shader.Dispose();
            }
        }
    }
}
