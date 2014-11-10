using Cog;
using Cog.Modules.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cog.Modules.EventHost;

namespace Cog.Interface
{
    public class Window : InterfaceElement
    {
        enum ResizeMode
        {
            TopLeft,
            Top,
            TopRight,
            Right,
            BottomRight,
            Bottom,
            BottomLeft,
            Left,

            None
        }

        private Texture windowTexture;

        private Vector2 oldMousePos;
        private bool isDragging;
        private ResizeMode resizeMode = ResizeMode.None;
        private BitmapFont font;

        public Window(InterfaceElement parent, Vector2 location)
            : base(parent, location)
        {
            windowTexture = Engine.Renderer.LoadTexture("blue_window.png");
            Size = new Vector2(128f, 128f);
            MinimumSize = new Vector2(17f, 29f);
            Padding = new Padding(8f, 8f, 18f, 8f);

            font = new BitmapFont("merriweather_16.fnt");
        }

        public override void OnUpdate(float deltaTime)
        {
            if (isDragging)
            {
                Vector2 delta = Mouse.Location - oldMousePos;
                oldMousePos = Mouse.Location;

                switch (resizeMode)
                {
                    case ResizeMode.TopRight:
                        Location = new Vector2(Location.X, Location.Y + delta.Y);
                        Size = new Vector2(Size.X + delta.X, Size.Y - delta.Y);
                        break;
                    case ResizeMode.Right:
                        Size = new Vector2(Size.X + delta.X, Size.Y);
                        break;
                    case ResizeMode.BottomRight:
                        Size += delta;
                        break;
                    case ResizeMode.Bottom:
                        Size = new Vector2(Size.X, Size.Y + delta.Y);
                        break;
                    case ResizeMode.BottomLeft:
                        Location = new Vector2(Location.X + delta.X, Location.Y);
                        Size = new Vector2(Size.X - delta.X, Size.Y + delta.Y);
                        break;
                    case ResizeMode.Left:
                        Location = new Vector2(Location.X + delta.X, Location.Y);
                        Size = new Vector2(Size.X - delta.X, Size.Y);
                        break;
                    case ResizeMode.TopLeft:
                        Location = Location + delta;
                        Size = Size - delta;
                        break;
                    case ResizeMode.Top:
                        /*Location = new Vector2(Location.X, Location.Y + delta.Y);
                        Size = new Vector2(Size.X, Size.Y - delta.Y);*/
                        Location += delta;
                        break;
                    case ResizeMode.None:
                        Location += delta;
                        break;
                    default:
                        throw new NotImplementedException("Window.ResizeMode." + resizeMode.ToString() + " is not implemented!");
                }
            }

            base.OnUpdate(deltaTime);
        }

        public override void OnDraw(IRenderTarget target, Vector2 drawPosition)
        {
            // Top Left
            target.RenderTexture(windowTexture, drawPosition, Color.White, Vector2.One, Vector2.Zero, 0f, new Rectangle(0f, 0f, 8f, 20f));
            // Top Right
            target.RenderTexture(windowTexture, drawPosition + new Vector2(Size.X - 8f, 0f), Color.White, Vector2.One, Vector2.Zero, 0f, new Rectangle(windowTexture.Size.X - 8f, 0f, 8f, 20f));
            // Bottom Left
            target.RenderTexture(windowTexture, drawPosition + new Vector2(0f, Size.Y - 8f), Color.White, Vector2.One, Vector2.Zero, 0f, new Rectangle(0f, 23f, 8f, 8f));
            // Bottom Right
            target.RenderTexture(windowTexture, drawPosition + Size - new Vector2(8f, 8f), Color.White, Vector2.One, Vector2.Zero, 0f, new Rectangle(11f, 23f, 8f, 8f));

            // Top
            target.RenderTexture(windowTexture, drawPosition + new Vector2(8f, 0f), Color.White, new Vector2(Size.X - 16f, 1f), Vector2.Zero, 0f, new Rectangle(9f, 0f, 1f, 20f));
            // Bottom
            target.RenderTexture(windowTexture, drawPosition + new Vector2(8f, Size.Y + -8f), Color.White, new Vector2(Size.X - 16f, 1f), Vector2.Zero, 0f, new Rectangle(9f, 23, 1f, 8f));
            // Left
            target.RenderTexture(windowTexture, drawPosition + new Vector2(0f, 20f), Color.White, new Vector2(1f, Size.Y - 28f), Vector2.Zero, 0f, new Rectangle(0f, 21f, 8f, 1f));
            // Right
            target.RenderTexture(windowTexture, drawPosition + new Vector2(Size.X + -8f, 20f), Color.White, new Vector2(1f, Size.Y - 28f), Vector2.Zero, 0f, new Rectangle(11f, 21f, 8f, 1f));

            // Middle Texture
            target.RenderTexture(windowTexture, drawPosition + new Vector2(8f, 20f), Color.White, Size - new Vector2(16f, 28f), Vector2.Zero, 0f, new Rectangle(9f, 21f, 1f, 1f));

            var textRect = ContentBounds;
            textRect.TopLeft += Location;

            font.DrawString(target, @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Pellentesque accumsan, risus sed luctus vulputate, magna ex egestas lorem, id posuere tellus velit quis ipsum. Morbi efficitur felis sed congue convallis. Pellentesque id dapibus mauris. Phasellus commodo sodales eleifend. Donec non fermentum risus. Morbi pretium ex ligula, tristique finibus ante dapibus et. In vitae massa id turpis rhoncus lobortis. Aliquam auctor ligula mauris, sed semper est porta vehicula.",
                Color.Black, textRect);

            base.OnDraw(target, drawPosition);
        }

        public override void OnPressed(Mouse.Button button, Vector2 position)
        {
            if (!ContentBounds.Contains(position))
            {
                if (button == Mouse.Button.Left)
                {
                    if (position.X >= Size.X - Padding.Right)
                    {
                        if (position.Y >= Size.Y - Padding.Bottom)
                            resizeMode = ResizeMode.BottomRight;
                        else if (position.Y < Padding.Top)
                            resizeMode = ResizeMode.TopRight;
                        else
                            resizeMode = ResizeMode.Right;
                    }
                    else if (position.X < Padding.Left)
                    {
                        if (position.Y >= Size.Y - Padding.Bottom)
                            resizeMode = ResizeMode.BottomLeft;
                        else if (position.Y < Padding.Top)
                            resizeMode = ResizeMode.TopLeft;
                        else
                            resizeMode = ResizeMode.Left;
                    }
                    else
                    {
                        if (position.Y < Padding.Top)
                            resizeMode = ResizeMode.Top;
                        else
                            resizeMode = ResizeMode.Bottom;
                    }
                    isDragging = true;
                    oldMousePos = ScreenCoord + position;
                }
            }

            base.OnPressed(button, position);
        }

        public override void OnReleased(Mouse.Button button, Vector2 position)
        {
            if (button == Mouse.Button.Left)
                isDragging = false;

            base.OnReleased(button, position);
        }
    }
}
