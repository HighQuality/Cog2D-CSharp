using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Square;
using Square.Modules;

namespace TestGame
{
    class MovementComponent : Square.Modules.Content.LinearPhysicsComponent
    {
        public Keyboard.Key Left = Keyboard.Key.Left,
            Right = Keyboard.Key.Right,
            Down = Keyboard.Key.Down,
            Up = Keyboard.Key.Up;
        public float MovementSpeed = 100f;

        public override void Update(float deltaTime)
        {
            if (Keys[Left])
                Speed.X -= MovementSpeed * deltaTime;
            if (Keys[Right])
                Speed.X += MovementSpeed * deltaTime;
            if (Keys[Up])
                Speed.Y -= MovementSpeed * deltaTime;
            if (Keys[Down])
                Speed.Y += MovementSpeed * deltaTime;

            Speed *= Math.Max(0f, 1f - deltaTime);

            base.Update(deltaTime);
        }
    }
}
