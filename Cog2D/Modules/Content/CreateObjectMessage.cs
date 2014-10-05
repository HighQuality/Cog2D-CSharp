using Cog.Modules.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Content
{
    class CreateObjectMessage : NetworkMessage
    {
        public ulong ObjectType;
        public ulong[] Components;

        public CreateObjectMessage(ulong objectType, ulong[] components)
        {
            if (!Engine.Permissions.CreateGlobalObjects)
                throw new Exception("You may not create global objects!");
        }

        public override void Received()
        {
            //if (!Sender.Permissions.CreateGlobalObjects)
            //    throw new Exception(string.Format("{0} may not create global objects!", Sender.Identifier));
           //GameObject.CreateFromId(ObjectType);
        }
    }
}
