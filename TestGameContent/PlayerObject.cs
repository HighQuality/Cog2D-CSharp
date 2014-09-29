using Square;
using Square.Modules.Content;
using Square.Scenes;
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

        public PlayerObject(Scene scene, Vector2 position)
            : base(scene, position)
        {
            MovementComponent = AddComponenet<MovementComponent>();
            SpriteComponent = AddComponenet<SpriteComponent>();
            SpriteComponent.Texture = Engine.Renderer.LoadTexture("texture.png");
        }
    }
}
