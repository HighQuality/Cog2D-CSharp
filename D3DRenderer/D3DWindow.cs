using Cog;
using Cog.Modules.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace D3DRenderer
{
    public class D3DWindow : Window, IRenderTarget
    {
        public override IRenderTarget RenderTarget { get { return this; } }

        public override bool VerticalSynchronization
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public D3DWindow(string title, int width, int height, WindowStyle style)
            : base(title, width, height, style)
        {
        }

        public override void Clear(Cog.Color color)
        {
            throw new NotImplementedException();
        }

        public override void Display()
        {
            throw new NotImplementedException();
        }

        public void SetTransformation(Vector2 center, Vector2 scale, Angle angle)
        {

        }

        public void DrawVerticies(Vertex[] verticies)
        {
        }

        public void DrawTexture(Texture texture, Vector2 windowCoords)
        {

        }

        public void DrawTexture(Texture texture, Vector2 windowCoords, Color color, Vector2 scale, Vector2 origin, float rotation, Rectangle textureRect)
        {

        }
    }
}
