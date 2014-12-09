using Cog.Modules.EventHost;
using Cog.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Content
{
    public class ObjectCreated : EventParameters
    {
        public readonly GameObject Object;

        public ObjectCreated(Scene sender, GameObject obj)
            : base(sender)
        {
            this.Object = obj;
        }
    }
}
