using Square;
using Square.Modules.EventHost;
using Square.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGame
{
    class GameScene : Scene
    {
        public World World;

        public GameScene()
            : base("Game")
        {
            World = new World(this);
        }
    }
}
