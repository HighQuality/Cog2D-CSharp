using Square.Modules.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Square.Modules.Content
{
    public class SpriteComponent : GameComponent
    {
        public ITexture Texture;
        public Vector2 CoordOffset;

        public override void Draw(IRenderTarget renderTarget)
        {
            renderTarget.RenderTexture(Texture, WorldCoord - CoordOffset);

            base.Draw(renderTarget);
        }
    }
}
