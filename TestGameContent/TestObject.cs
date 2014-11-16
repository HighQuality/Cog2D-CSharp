using Cog;
using Cog.Modules.Content;
using Cog.Modules.EventHost;
using Cog.Modules.Renderer;
using Cog.Modules.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGame
{
    [Resource(ContainerName = "main", Filename = "valid.png", Key = "texture")]
    public class TestObject : GameObject
    {
        public SpriteComponent Sprite;
        public MovementComponent Movement;

        public TestObject()
        {
            Sprite = SpriteComponent.RegisterOn(this, Resources.GetTexture("texture"));
            Movement = MovementComponent.RegisterOn(this);
            Movement.Left = new KeyCapture(this, Keyboard.Key.A, 0, CaptureRelayMode.NoRelay);
            Movement.Right = new KeyCapture(this, Keyboard.Key.D, 0, CaptureRelayMode.NoRelay);
            Movement.Up = new KeyCapture(this, Keyboard.Key.W, 0, CaptureRelayMode.NoRelay);
            Movement.Down = new KeyCapture(this, Keyboard.Key.S, 0, CaptureRelayMode.NoRelay);

            RegisterEvent<UpdateEvent>(0, e => LocalRotation += Angle.FromRadian(e.DeltaTime));
        }
    }
}
