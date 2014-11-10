using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Cog.Extensions
{
    public static class ByteArrayExtensions
    {
        public static byte[] Md5(this byte[] self)
        {
            using (var md5 = MD5.Create())
            {
                return md5.ComputeHash(self);
            }
        }
    }
}
