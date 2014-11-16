using Cog;
using Cog.Modules.EventHost;
using Cog.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGame
{
    public class GameScene : Scene
    {
        public World World;

        public GameScene()
            : base("Game")
        {
            if (Engine.IsServer)
            {
                var obj = CreateObject<TestObject>(new Vector2(0f, 0f));
                CreateObject<TestObject>(obj, new Vector2(32f, 0f));
                var second = CreateObject<TestObject>(obj, new Vector2(0f, 32f));
                CreateObject<TestObject>(second, new Vector2(8f, 0f));
            }
        }
    }
}
