using SlimDX;
using SlimDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Buffer = SlimDX.Direct3D11.Buffer;

namespace D3DRenderer
{
    public abstract class ConstantBuffer : DXResource
    {
        private List<Type> types = new List<Type>();
        protected int size;
        public Buffer Buffer;

        protected int AddType(Type type)
        {
            if (Buffer != null)
                throw new InvalidOperationException("Types must be registered to the ConstantBuffer before Initialize() has been called!");

            int typeSize = Marshal.SizeOf(type);
            int memorySize = (int)Math.Ceiling((float)typeSize / 16f) * 16;
            types.Add(type);
            size += memorySize;
            return memorySize - typeSize;
        }

        protected void Initialize()
        {
            Buffer = new Buffer(D3DWindow.Device, new BufferDescription
            {
                BindFlags = BindFlags.ConstantBuffer,
                SizeInBytes = size,
                Usage = ResourceUsage.Default
            });
        }

        protected void SetData(DataStream stream)
        {
            D3DWindow.Context.UpdateSubresource(new DataBox(0, 0, stream), Buffer, 0);
        }

        public override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Buffer.Dispose();
            }
        }
    }
}
