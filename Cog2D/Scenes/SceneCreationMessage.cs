﻿using Cog.Modules.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Cog.Scenes
{
    class SceneCreationMessage : NetworkMessage
    {
        public ushort TypeId;
        public long Id;
        public string SceneName;
        public byte[] Data;
        public byte[] UserData;

        public SceneCreationMessage()
        {

        }

        public override void Received()
        {
            // TODO: Move to restriction attribute
            if (Engine.IsServer)
                throw new Exception("Client sent server a SceneCreationMessage");

            var scene = SceneCache.CreateFromId(TypeId, Id, true);
            Engine.ClientModule.RemotelyCreatedScenes.Add(scene);
            scene.ReadSceneCreationData(Data);
            using (var stream = new MemoryStream(UserData))
            {
                using (var reader = new BinaryReader(stream))
                {
                    scene.ReadUserData(reader);
                }
            }
            
            // TODO: Add to Engine resolve dictionary and fire SceneReceivedEvent instead of pushing
            Engine.SceneHost.Push(scene);

            Debug.Event("Received scene \"{0}\" from server", scene.Name);
        }
    }
}
