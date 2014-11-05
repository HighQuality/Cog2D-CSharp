using Cog;
using Cog.Modules.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGame
{
    class TestObject : GameObject
    {
        public SpriteComponent Sprite;
        public MovementComponent Movement;

        public TestObject()
        {
            Sprite = SpriteComponent.RegisterOn(this, Engine.Renderer.LoadTexture("rectangle.png"));
            Movement = MovementComponent.RegisterOn(this);
        }
    }
}
