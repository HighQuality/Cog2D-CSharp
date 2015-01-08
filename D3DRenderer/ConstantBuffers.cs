using SlimDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace D3DRenderer
{
    public class ConstantBuffer<T> : ConstantBuffer
        where T : struct
    {
        public ConstantBuffer()
        {
            AddType(typeof(T));
            Initialize();
        }

        public void SetData(T first)
        {
            DataStream stream = new DataStream(size, true, true);
            stream.Write(first);
            stream.Position = 0;
            SetData(stream);
        }
    }

    public class ConstantBuffer<T, T2> : ConstantBuffer
        where T : struct
        where T2 : struct
    {
        private int[] extraData = new int[2];

        public ConstantBuffer()
        {
            extraData[0] = AddType(typeof(T));
            extraData[1] = AddType(typeof(T2));
            Initialize();
        }

        public void SetData(T first, T2 second)
        {
            using (DataStream stream = new DataStream(new byte[size], true, true))
            {
                stream.Write(first);
                stream.Position += extraData[0];
                stream.Write(second);
                stream.Position = 0;
                SetData(stream);
            }
        }
    }

    public class ConstantBuffer<T, T2, T3> : ConstantBuffer
        where T : struct
        where T2 : struct
        where T3 : struct
    {
        private int[] extraData = new int[3];

        public ConstantBuffer()
        {
            extraData[0] = AddType(typeof(T));
            extraData[1] = AddType(typeof(T2));
            extraData[2] = AddType(typeof(T3));
            Initialize();
        }

        public void SetData(T first, T2 second, T3 third)
        {
            using (DataStream stream = new DataStream(new byte[size], true, true))
            {
                stream.Write(first);
                stream.Position += extraData[0];
                stream.Write(second);
                stream.Position += extraData[1];
                stream.Write(third);
                stream.Position = 0;
                SetData(stream);
            }
        }
    }

    public class ConstantBuffer<T, T2, T3, T4> : ConstantBuffer
        where T : struct
        where T2 : struct
        where T3 : struct
        where T4 : struct
    {
        private int[] extraData = new int[4];

        public ConstantBuffer()
        {
            extraData[0] = AddType(typeof(T));
            extraData[1] = AddType(typeof(T2));
            extraData[2] = AddType(typeof(T3));
            extraData[3] = AddType(typeof(T4));
            Initialize();
        }

        public void SetData(T first, T2 second, T3 third, T4 fourth)
        {
            using (DataStream stream = new DataStream(new byte[size], true, true))
            {
                stream.Write(first);
                stream.Position += extraData[0];
                stream.Write(second);
                stream.Position += extraData[1];
                stream.Write(third);
                stream.Position += extraData[2];
                stream.Write(fourth);
                stream.Position = 0;
                SetData(stream);
            }
        }
    }
}
