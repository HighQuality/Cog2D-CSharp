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
        public UInt16 ObjectType;
        public UInt16[] Components;
        public byte[][] ComponentDatas;

        public CreateObjectMessage(UInt16 objectType, UInt16[] components, byte[][] componentDatas)
        {
            if (!Engine.Permissions.CreateGlobalObjects)
                throw new Exception("You may not create global objects!");
            this.ObjectType = objectType;
            this.Components = components;
            this.ComponentDatas = componentDatas;
        }

        public override void Received()
        {
            if (!Sender.Permissions.CreateGlobalObjects)
                throw new Exception(string.Format("{0} may not create global objects!", Sender.Identifier));
            GameObject.CreateFromMessage(this);
        }
    }
}
