using SlimDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace D3DRenderer
{
    public interface IVertexBuffer
    {
        VertexBufferBinding Binding { get; }
    }
}
