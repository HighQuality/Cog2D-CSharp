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
        public Keyboard.Key LeftKey { get { return left.Key; } set { if (left == null) left = CaptureKey(value, 1, Engine.IsNetworkGame ? CaptureRelayMode.ServerClientRelay : CaptureRelayMode.NoRelay); else left.Key = value; } }
        public Keyboard.Key RightKey { get { return right.Key; } set { if (right == null) right = CaptureKey(value, 1, Engine.IsNetworkGame ? CaptureRelayMode.ServerClientRelay : CaptureRelayMode.NoRelay); else right.Key = value; } }
        public Keyboard.Key UpKey { get { return up.Key; } set { if (up == null) up = CaptureKey(value, 1, Engine.IsNetworkGame ? CaptureRelayMode.ServerClientRelay : CaptureRelayMode.NoRelay); else up.Key = value; } }
        public Keyboard.Key DownKey { get { return down.Key; } set { if (down == null) down = CaptureKey(value, 1, Engine.IsNetworkGame ? CaptureRelayMode.ServerClientRelay : CaptureRelayMode.NoRelay); else down.Key = value; } }
        public KeyCapture left,
            right,
            up,
            down;
        public float MovementForce = 300f,
            MaxSpeed = 200f;

        public MovementComponent()
        {
        }

        public override void PhysicsUpdate(float deltaTime)
        {
            if (left != null && left.IsDown)
                Speed.X -= MovementForce * deltaTime;
            if (right != null && right.IsDown)
                Speed.X += MovementForce * deltaTime;
            if (up != null && up.IsDown)
                Speed.Y -= MovementForce * deltaTime;
            if (down != null && down.IsDown)
                Speed.Y += MovementForce * deltaTime;

            Speed *= Mathf.Max(0f, 1f - deltaTime * 3f);

            base.PhysicsUpdate(deltaTime);
        }
    }
}
