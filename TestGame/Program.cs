﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cog;
using Cog.SfmlRenderer;
using Cog.Modules.Renderer;
using Cog.Modules.EventHost;
using Cog.Modules.Content;
using System.Threading;
using Cog.Modules.Networking;
using Cog.Interface;
using System.IO;
using Cog.SfmlAudio;

namespace TestGame
{
    class Program
    {
        static void Main(string[] args)
        {
            Engine.Initialize<D3DRenderer.D3DRenderer, SfmlAudioModule>();
            float time = 0f;

            var container = Engine.ResourceHost.LoadDictionary("main", "resources");

            Texture texture = null;

            LoadingScene scene = null;

            Engine.EventHost.RegisterEvent<InitializeEvent>(0, e =>
            {
                var message = Engine.ConnectServer("127.0.0.1", 1234);
                if (message != null)
                    Debug.Error(message);
                else
                    Debug.Success("Successfully connected to server @{0}:{1}!", Engine.ClientModule.Hostname, Engine.ClientModule.Port);

                texture = Engine.Renderer.LoadTexture("splash.png");

                scene = Engine.SceneHost.CreateLocal<LoadingScene>();
            });
            
            Engine.EventHost.RegisterEvent<UpdateEvent>(1, e =>
            {
                time += e.DeltaTime;
            });
            
            Engine.EventHost.RegisterEvent<DrawEvent>(1, e =>
            {
                e.RenderTarget.DrawTexture(texture, new Vector2((float)Engine.TimeStamp * 32f, 0f), Color.White, new Vector2(2f, 0.25f), texture.Size / 2f, (float)Engine.TimeStamp * 45f, new Rectangle(Vector2.Zero, texture.Size));
            });

            Engine.EventHost.RegisterEvent<ExitEvent>(0, e =>
            {
            });
            
            Engine.StartGame("Test Game", WindowStyle.Default);
        }
    }
}
