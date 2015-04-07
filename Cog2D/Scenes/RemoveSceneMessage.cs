using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cog.Modules.Networking;

namespace Cog.Scenes
{
    class RemoveSceneMessage : NetworkMessage
    {
        public Scene SceneToRemove;

        public RemoveSceneMessage(Scene scene)
        {
            this.SceneToRemove = scene;
        }

        public override void Received()
        {
            if (Engine.IsServer)
            {
                Debug.Warning("Client {0} tried to send us a RemoveSceneMessage!", Client.Identifier);
                return;
            }
            if (SceneToRemove == null)
            {
                Debug.Warning("Server told us to remove a null scene!");
                return;
            }
            SceneToRemove.Remove();
        }
    }
}
