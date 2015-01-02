using Cog.Modules.Networking;
using Cog.Scenes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Content
{
    class CreateObjectMessage : NetworkMessage
    {
        public Scene Scene;
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
            if (!Client.Permissions.CreateGlobalObjects)
                throw new Exception(string.Format("{0} may not create global objects!", Client.Identifier));
            if (Scene == null)
                throw new Exception("Tried to create an object in a scene that does not exist!");

            var obj = Scene.CreateUninitializedObject(GameObject.TypeFromId(TypeId), null);
            Engine.AssignId(obj, ObjectId);

            using (var stream = new MemoryStream(ObjectData))
                using (var reader = new BinaryReader(stream))
                    obj.Deserialize(reader);

            Scene.InitializeObject(obj);
            obj.Initialize();
        }
    }
}
