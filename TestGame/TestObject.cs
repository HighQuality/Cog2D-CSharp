﻿using Cog;
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
            Sprite = AddComponent<SpriteComponent>();
            Sprite.Texture = Engine.Renderer.LoadTexture("rectangle.png");
            Sprite.Origin = Sprite.Texture.Size / 2f;
            Movement = AddComponent<MovementComponent>();
        }
    }
}
