using Cog;
using Cog.Modules.Content;
using Cog.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGame
{
    public class PlayerObject : GameObject
    {
        public MovementComponent MovementComponent { get; private set; }
        public SpriteComponent SpriteComponent { get; private set; }
        
        public PlayerObject()
        {
            MovementComponent = AddComponent<MovementComponent>();
            SpriteComponent = AddComponent<SpriteComponent>();
            // SpriteComponent.Texture = Engine.Renderer.LoadTexture("texture.png");
        }
    }
}
