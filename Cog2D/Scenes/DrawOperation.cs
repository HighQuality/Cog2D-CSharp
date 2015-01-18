using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cog.Scenes
{
    struct DrawOperation
    {
        public float Depth;
        public long ObjectId;
        public Action Action;
    }
}
