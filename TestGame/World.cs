using Square;
using Square.Modules.Content;
using Square.Modules.EventHost;
using Square.Modules.Renderer;
using Square.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGame
{
    class World : GameObject
    {
        public World(Scene scene)
            : base(scene, Vector2.Zero)
        {
            for (int i = 0; i < 1; i++)
                new PlayerObject(Scene, new Vector2());

            RegisterEvent<DrawEvent>(0, Draw);
            RegisterEvent<KeyDownEvent>(99, e => { if (e.Key == Keyboard.Key.A) { Remove(); e.Intercept = true; } });
        }

        private void Draw(DrawEvent args)
        {
        
        }
    }
}
