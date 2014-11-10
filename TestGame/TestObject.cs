using Cog;
using Cog.Modules.Content;
using Cog.Modules.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGame
{
    [Resource(ContainerName = "main", Filename = "valid.png", Key = "texture")]
    class TestObject : GameObject
    {
        public SpriteComponent Sprite;

        public TestObject()
        {
            Sprite = SpriteComponent.RegisterOn(this, Resources.GetTexture("texture"));
        }
    }
}
