using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cog;
using Cog.Modules;
using Cog.Modules.Content;
using Cog.Modules.EventHost;

namespace TestGame
{
    public class MovementComponent : Cog.Modules.Content.LinearPhysicsComponent
    {
        public KeyCapture Left,
            Right,
            Up,
            Down;
        public float MovementForce = 300f,
            MaxSpeed = 200f;

        public MovementComponent()
        {
            Left = CaptureKey(Keyboard.Key.Left, 1, Engine.IsNetworkGame ? CaptureRelayMode.ServerClientRelay : CaptureRelayMode.NoRelay);
            Right = CaptureKey(Keyboard.Key.Right, 1, Engine.IsNetworkGame ? CaptureRelayMode.ServerClientRelay : CaptureRelayMode.NoRelay);
            Up = CaptureKey(Keyboard.Key.Up, 1, Engine.IsNetworkGame ? CaptureRelayMode.ServerClientRelay : CaptureRelayMode.NoRelay);
            Down = CaptureKey(Keyboard.Key.Down, 1, Engine.IsNetworkGame ? CaptureRelayMode.ServerClientRelay : CaptureRelayMode.NoRelay);
        }

        public override void PhysicsUpdate(float deltaTime)
        {
            if (Left.IsDown)
                Speed.X -= MovementForce * deltaTime;
            if (Right.IsDown)
                Speed.X += MovementForce * deltaTime;
            if (Up.IsDown)
                Speed.Y -= MovementForce * deltaTime;
            if (Down.IsDown)
                Speed.Y += MovementForce * deltaTime;

            Speed *= Mathf.Max(0f, 1f - deltaTime * 3f);

            base.PhysicsUpdate(deltaTime);
        }
    }
}
