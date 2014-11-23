using Cog.Modules.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Content
{
    class RemoveObjectMessage : NetworkMessage
    {
        public GameObject Object;

        public RemoveObjectMessage(GameObject obj)
        {
            this.Object = obj;
        }

        public override void Received()
        {
            Object.ForceRemove();
        }
    }
}
