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
                var firstObject = CreateObject<TestObject>(new Vector2(0f, 0f));
                firstObject.ObjectName = "First Object";
                firstObject.LocalRotation = Angle.FromDegree(90f);

                var secondObject = CreateObject<TestObject>(firstObject, new Vector2(32f, 0f));
                secondObject.ObjectName = "Second Object";

                secondObject.SynchronizedTarget.Value = firstObject;
                firstObject.SynchronizedTarget.Value = secondObject;
            }
        }
    }
}
