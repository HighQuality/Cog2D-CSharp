using Cog;
using Cog.Modules.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGame
{
    class SpriteObject : GameObject
    {
        public SpriteComponent Sprite;

        public SpriteObject()
        {
            Sprite = SpriteComponent.RegisterOn(this, null);
        }
    }
}
