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
    public class World : GameObject
    {
        public World()
        {
            /*for (int i = 0; i < 1; i++)
                Scene.CreateObject<PlayerObject>(new Vector2(i * 16f, i * 16f));*/

            RegisterEvent<DrawEvent>(0, Draw);
            RegisterEvent<KeyDownEvent>(99, e => { if (e.Key == Keyboard.Key.A) { Remove(); e.Intercept = true; } });
        }


        private void Draw(DrawEvent args)
        {
        
        }
    }
}
