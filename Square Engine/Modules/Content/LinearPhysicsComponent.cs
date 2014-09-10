using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Square.Modules.Content
{
    public class LinearPhysicsComponent : GameComponent
    {
        public Vector2 Speed;

        public override void Update(float deltaTime)
        {
            WorldCoord += new Vector2(Speed.X * deltaTime, Speed.Y * deltaTime);

            base.Update(deltaTime);
        }
    }
}
