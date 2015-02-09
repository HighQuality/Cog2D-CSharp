using Cog;
using Cog.Modules.Animation;
using Cog.Modules.Content;
using Cog.Modules.EventHost;
using Cog.Modules.Renderer;
using Cog.Modules.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGame
{
    [Resource(ContainerName = "main", Filename = "valid.png", Key = "texture")]
    public class StationaryObject : GameObject<LoadingScene>
    {
        public StationaryObject()
        {
            SpriteComponent.RegisterOn(this, Resources.GetTexture("texture"));
        }
    }
}
