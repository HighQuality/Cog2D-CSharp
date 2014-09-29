using Square.Modules.EventHost;
using Square.Modules.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Square.Modules.Content
{
    public class SpriteComponent : ObjectComponent
    {
        public ITexture Texture;
        public Vector2 CoordOffset;

        public SpriteComponent()
        {
            RegisterEvent<KeyDownEvent>(Keyboard.Key.Right, 10, e =>
            {
                GameObject.Remove();
                e.Intercept = true;
            });
        }

        public override void Draw(IRenderTarget renderTarget)
        {
            renderTarget.RenderTexture(Texture, WorldCoord - CoordOffset);

            base.Draw(renderTarget);
        }
    }
}
