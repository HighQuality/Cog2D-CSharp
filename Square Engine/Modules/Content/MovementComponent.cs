using Square.Modules.EventHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Square.Modules.Content
{
    public class MovementComponent : LinearPhysicsComponent
    {
        public MovementComponent()
        {
            RegisterEvent<CloseButtonEvent>(999, e =>
            {
                GameObject.Remove();
                e.Intercept = true;
            });
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
        }

        public override bool KeyDown(Keyboard.Key key)
        {
            if (key == Keyboard.Key.Right)
                Speed.X += 100f;
            else if (key == Keyboard.Key.Left)
                Speed.X -= 100f;
            else if (key == Keyboard.Key.Up)
                Speed.Y -= 100f;
            else if (key == Keyboard.Key.Down)
                Speed.Y += 100f;

            return true;
        }
        public override void KeyUp(Keyboard.Key key)
        {
            if (key == Keyboard.Key.Right)
                Speed.X -= 100f;
            else if (key == Keyboard.Key.Left)
                Speed.X += 100f;
            else if (key == Keyboard.Key.Down)
                Speed.Y -= 100f;
            else if (key == Keyboard.Key.Up)
                Speed.Y += 100f;
            base.KeyDown(key);
        }
    }
}
