using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Square;
using Square.Modules;

namespace TestGame
{
    public class MovementComponent : Square.Modules.Content.LinearPhysicsComponent
    {
        public Keyboard.Key Left = Keyboard.Key.Left,
            Right = Keyboard.Key.Right,
            Down = Keyboard.Key.Down,
            Up = Keyboard.Key.Up;
        public float MovementForce = 100f,
            MaxSpeed = 200f;

        public override void Update(float deltaTime)
        {
            if (Keys[Left])
                Speed.X -= MovementForce * deltaTime;
            if (Keys[Right])
                Speed.X += MovementForce * deltaTime;
            if (Keys[Up])
                Speed.Y -= MovementForce * deltaTime;
            if (Keys[Down])
                Speed.Y += MovementForce * deltaTime;

            base.Update(deltaTime);
        }
    }
}
