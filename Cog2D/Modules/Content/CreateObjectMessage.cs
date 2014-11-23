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
        public ushort TypeId;
        public long ObjectId;
        public byte[] ObjectData;

        public CreateObjectMessage(byte[] objectData)
        {
            if (!Engine.Permissions.CreateGlobalObjects)
                throw new Exception("You may not create global objects!");
            this.ObjectData = objectData;
        }

        public override void Received()
        {
            if (!Sender.Permissions.CreateGlobalObjects)
                throw new Exception(string.Format("{0} may not create global objects!", Sender.Identifier));
            throw new NotImplementedException();
        }
    }
}
