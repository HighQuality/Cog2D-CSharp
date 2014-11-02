using Cog.Modules.EventHost;
using Cog.Modules.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Content
{
    public class SpriteComponent : ObjectComponent
    {
        public ITexture Texture;
        public Vector2 Origin,
            Scale;
        public Color Color = Color.White;

        public SpriteComponent()
        {
        }

        public override void Draw(IRenderTarget renderTarget)
        {
            renderTarget.RenderTexture(Texture, WorldCoord, Color, Vector2.One, Origin, GameObject.WorldRotation.Degree, new Rectangle(Vector2.Zero, Texture.Size));

            base.Draw(renderTarget);
        }
    }
}
