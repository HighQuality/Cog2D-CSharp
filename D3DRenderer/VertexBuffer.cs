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
    public class VertexBuffer<T> : DXResource, IVertexBuffer
        where T : struct
    {
        public int VertexSize { get; private set; }
        public int VertexCount { get; private set; }
        private Buffer buffer;
        public VertexBufferBinding Binding { get; private set; }

        public VertexBuffer(T[] vertices)
        {
            VertexSize = Marshal.SizeOf(typeof(T));
            VertexCount = vertices.Length;

            using (DataStream stream = new DataStream(vertices.Length * VertexSize, true, true))
            {
                stream.WriteRange(vertices);
                stream.Position = 0;

                buffer = new Buffer(D3DWindow.Device, stream, new BufferDescription
                {
                    BindFlags = BindFlags.VertexBuffer,
                    SizeInBytes = vertices.Length * VertexSize,
                    Usage = ResourceUsage.Default,
                    OptionFlags = ResourceOptionFlags.None,
                    CpuAccessFlags = CpuAccessFlags.None,
                    StructureByteStride = 0
                });
            }

            Binding = new VertexBufferBinding(buffer, VertexSize, 0);
        }
        public VertexBuffer(int vertexCapacity)
        {
            VertexSize = Marshal.SizeOf(typeof(T));
            VertexCount = vertexCapacity;

            buffer = new Buffer(D3DWindow.Device, null, new BufferDescription
            {
                BindFlags = BindFlags.VertexBuffer,
                SizeInBytes = vertexCapacity * VertexSize,
                Usage = ResourceUsage.Default,
                OptionFlags = ResourceOptionFlags.None,
                CpuAccessFlags = CpuAccessFlags.None,
                StructureByteStride = 0
            });

            Binding = new VertexBufferBinding(buffer, VertexSize, 0);
        }

        public void SetData(T[] vertices)
        {
            if (vertices.Length > VertexCount)
                throw new ArgumentOutOfRangeException("vertices.Length must be less than or equal to VertexCount");

            using (DataStream stream = new DataStream(VertexCount * VertexSize, true, true))
            {
                stream.WriteRange(vertices);
                stream.Position = 0;

                D3DWindow.Context.UpdateSubresource(new DataBox(0, 0, stream), buffer, 0);
            }
        }

        public override void Dispose(bool disposing)
        {
            if (disposing)
            {
                buffer.Dispose();
            }
        }
    }
}
