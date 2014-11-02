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
            Sprite = AddComponenet<SpriteComponent>();
            Sprite.Texture = Engine.Renderer.LoadTexture("rectangle.png");
            Movement = AddComponenet<MovementComponent>();
        }
    }
}
