using Cog;
using Cog.Modules.Renderer;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XnaRenderer
{
    public class XnaWindow : Window, IRenderTarget
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

        private static SpriteBatch spriteBatch;
        public static GraphicsDevice GraphicsDevice;
        private static bool hasBegun;
        public static BlendState CurrentBlendState;
        public static Microsoft.Xna.Framework.Matrix CurrentMatrix;

        public XnaWindow(string title, int width, int height, WindowStyle style)
            : base(title, width, height, style)
        {
            ResizeBackBuffer(Resolution);
        }

        public override void Clear(Cog.Color color)
        {
            GraphicsDevice.Clear(new Microsoft.Xna.Framework.Color(color.R, color.G, color.B, color.A));
        }

        public override void Display()
        {
            TryEnd();
            GraphicsDevice.Present();
            Begin(Microsoft.Xna.Framework.Matrix.Identity, CurrentBlendState);
        }

        public void SetTransformation(Vector2 center, Vector2 scale, Angle angle)
        {
            TryEnd();

            var matrix = Microsoft.Xna.Framework.Matrix.Identity * Microsoft.Xna.Framework.Matrix.CreateTranslation(-center.X, -center.Y, 0.5f) *
                Microsoft.Xna.Framework.Matrix.CreateRotationZ(-angle.Radian) *
                Microsoft.Xna.Framework.Matrix.CreateScale(scale.X, scale.Y, 1f) *
                Microsoft.Xna.Framework.Matrix.CreateTranslation(Resolution.X / 2f, Resolution.Y / 2f, 0f);

            Begin(matrix, CurrentBlendState);
        }

        public void DrawTexture(Cog.Modules.Renderer.Texture texture, Vector2 windowCoords)
        {
            spriteBatch.Draw(((XnaTexture)texture).InnerTexture, new Microsoft.Xna.Framework.Vector2(windowCoords.X, windowCoords.Y), Microsoft.Xna.Framework.Color.White);
        }
        public void DrawTexture(Cog.Modules.Renderer.Texture texture, Vector2 windowCoords, Color color, Vector2 scale, Vector2 origin, float rotation, Rectangle textureRect)
        {
            var spriteEffects = SpriteEffects.None;
            if (scale.X < 0f)
            {
                spriteEffects |= SpriteEffects.FlipHorizontally;
                scale.X *= -1f;
            }
            if (scale.Y < 0f)
            {
                spriteEffects |= SpriteEffects.FlipVertically;
                scale.Y *= -1f;
            }

            spriteBatch.Draw(((XnaTexture)texture).InnerTexture, new Microsoft.Xna.Framework.Vector2(windowCoords.X, windowCoords.Y), new Microsoft.Xna.Framework.Rectangle((int)textureRect.Left, (int)textureRect.Top, (int)textureRect.Width, (int)textureRect.Height), new Microsoft.Xna.Framework.Color(color.R, color.G, color.B, color.A), rotation / 180f * Mathf.Pi, new Microsoft.Xna.Framework.Vector2(origin.X, origin.Y), new Microsoft.Xna.Framework.Vector2(scale.X, scale.Y), spriteEffects, 0f);
        }

        internal static void TryEnd()
        {
            if (hasBegun)
                spriteBatch.End();
            hasBegun = false;
        }

        internal static void Begin(Microsoft.Xna.Framework.Matrix matrix, BlendState blendState)
        {
            if (hasBegun)
                spriteBatch.End();
            else
                hasBegun = true;

            CurrentBlendState = blendState;
            CurrentMatrix = matrix;

            spriteBatch.Begin(SpriteSortMode.Immediate, blendState, null, null, null, null, matrix);
        }

        public override void ResizeBackBuffer(Cog.Vector2 newResolution)
        {
            PresentationParameters pp = new PresentationParameters();
            pp.DeviceWindowHandle = Handle;

            pp.BackBufferFormat = SurfaceFormat.Color;
            pp.BackBufferWidth = (int)newResolution.X;
            pp.BackBufferHeight = (int)newResolution.Y;
            pp.RenderTargetUsage = RenderTargetUsage.DiscardContents;
            pp.IsFullScreen = false;

            pp.MultiSampleCount = 16;

            pp.DepthStencilFormat = DepthFormat.Depth24Stencil8;

            if (GraphicsDevice == null)
            {
                GraphicsDevice = new GraphicsDevice(GraphicsAdapter.DefaultAdapter,
                                                          GraphicsProfile.HiDef,
                                                          pp);

                spriteBatch = new SpriteBatch(GraphicsDevice);

                Begin(Microsoft.Xna.Framework.Matrix.Identity, BlendState.AlphaBlend);
            }
            else
            {
                GraphicsDevice.Reset(pp);
            }
        }
    }
}
