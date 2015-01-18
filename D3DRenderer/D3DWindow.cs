using Cog;
using Cog.Modules.Renderer;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Device = SlimDX.Direct3D11.Device;
using Resource = SlimDX.Direct3D11.Resource;
using Buffer = SlimDX.Direct3D11.Buffer;
using System.Diagnostics;
using System.Threading;

namespace D3DRenderer
{
    public class D3DWindow : Window, IRenderTarget
    {
        internal static SlimDX.Direct3D11.Device Device;
        internal static SlimDX.Direct3D11.DeviceContext Context;

        internal ConstantBuffer<SlimDX.Matrix> MatrixBuffer;

        SlimDX.Direct3D11.RenderTargetView renderTarget;
        SlimDX.DXGI.SwapChain swapChain;
        
        private PixelShader _pixelShader;
        public PixelShader PixelShader
        {
            get { return _pixelShader; }
            set
            {
                if (value != null)
                {
                    Context.PixelShader.Set(value.Shader);
                    Context.PixelShader.SetConstantBuffers(value.ConstantBuffers, 0, value.ConstantBuffers.Length);
                }
                else
                    Context.PixelShader.Set(null);
                _pixelShader = value;
            }
        }
        private VertexShader _vertexShader;
        public VertexShader VertexShader
        {
            get { return _vertexShader; }
            set
            {
                if (value != null)
                {
                    Context.VertexShader.Set(value.Shader);
                    Context.VertexShader.SetConstantBuffers(value.ConstantBuffers, 0, value.ConstantBuffers.Length);
                }
                else
                    Context.VertexShader.Set(null);
                Context.InputAssembler.InputLayout = value.InputLayout;
                _vertexShader = value;
            }
        }

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

        private static List<Vertex> vertices;

        public D3DWindow(string title, int width, int height, WindowStyle style)
            : base(title, width, height, style)
        {
            vertices = new List<Vertex>();

            var description = new SwapChainDescription()
            {
                BufferCount = 2,
                Usage = SlimDX.DXGI.Usage.RenderTargetOutput,
                OutputHandle = Handle,
                IsWindowed = true,
                ModeDescription = new SlimDX.DXGI.ModeDescription(0, 0, new SlimDX.Rational(60, 1), Format.R8G8B8A8_UNorm),
                SampleDescription = new SampleDescription(1, 0),
                Flags = SwapChainFlags.AllowModeSwitch,
                SwapEffect = SwapEffect.Discard
            };

            SlimDX.Direct3D11.Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, description, out Device, out swapChain);

            using (var resource = Resource.FromSwapChain<Texture2D>(swapChain, 0))
                renderTarget = new RenderTargetView(Device, resource);

            Context = Device.ImmediateContext;
            Context.OutputMerger.SetTargets(renderTarget);
            Context.Rasterizer.SetViewports(new Viewport(0f, 0f, Resolution.X, Resolution.Y));

            PixelShader = new PixelShader(DefaultShader.Source, "PShader", null);

            MatrixBuffer = new ConstantBuffer<SlimDX.Matrix>();
            VertexShader = new VertexShader( DefaultShader.Source, "VShader", new[] { MatrixBuffer }, DefaultShader.InputElements);
            SetTransformation(Vector2.Zero, Vector2.One, Angle.FromDegree(0f));

            // prevent DXGI handling of alt+enter, which doesn't work properly with Winforms
            using (var factory = swapChain.GetParent<Factory>())
                factory.SetWindowAssociation(Handle, WindowAssociationFlags.IgnoreAltEnter);

            Context.Rasterizer.State = RasterizerState.FromDescription(Device, new RasterizerStateDescription
            {
                CullMode = CullMode.None,
                IsScissorEnabled = false,
                IsAntialiasedLineEnabled = false,
                IsDepthClipEnabled = false,
                IsFrontCounterclockwise = false,
                IsMultisampleEnabled = false,
                DepthBias = 0,
                FillMode = FillMode.Solid,
                DepthBiasClamp = 0.0f,
                SlopeScaledDepthBias = 0.0f
            });

            Context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
        }

        public override void Clear(Cog.Color color)
        {
            Context.ClearRenderTargetView(renderTarget, new SlimDX.Color4(color.A / 255f, color.R / 255f, (float)color.G / 255f, (float)color.B / 255f));
        }

        public override void Display()
        {
            // Draw buffered vertices that haven't been drawn yet
            DrawBuffer();

            swapChain.Present(1, PresentFlags.None);
        }

        public void SetTransformation(Vector2 center, Vector2 scale, Angle angle)
        {
            // Draw vertices queued to be drawn with the old transformation
            DrawBuffer();
            MatrixBuffer.SetData(SlimDX.Matrix.Identity *
                SlimDX.Matrix.Translation(-center.X, -center.Y, 0.5f) *
                SlimDX.Matrix.RotationZ(-angle.Radian) *
                SlimDX.Matrix.Scaling(2f / Resolution.X * scale.X, -2f / Resolution.Y * scale.Y, 1f));
        }
        
        private Texture _texture;
        private void SetTexture(D3DTexture texture)
        {
            if (_texture != texture)
            {
                // Draw vertices queued to be drawn with the old texture
                DrawBuffer();

                _texture = texture;
                D3DWindow.Context.PixelShader.SetShaderResource(texture.ResourceView, 0);
            }
        }

        private static VertexBuffer<Vertex> vertexBuffer;
        internal static void DrawBuffer()
        {
            if (vertices.Count == 0)
                return;
            if (vertexBuffer == null)
            {
                vertexBuffer = new VertexBuffer<Vertex>(1024);
                Context.InputAssembler.SetVertexBuffers(0, vertexBuffer.Binding);
            }

            // TODO: Redo to single operation
            while (vertexBuffer.VertexCount < vertices.Count)
            {
                vertexBuffer.Dispose();
                vertexBuffer = new VertexBuffer<Vertex>(vertexBuffer.VertexCount * 2);
                Context.InputAssembler.SetVertexBuffers(0, vertexBuffer.Binding);
            }

            vertexBuffer.SetData(vertices.ToArray());
            Context.Draw(vertices.Count, 0);
            //Context.DrawInstanced(4, vertices.Count / 4, 0, 0);
            vertices.Clear();
        }

        public void DrawTexture(Texture texture, Vector2 windowCoords)
        {
            SetTexture((D3DTexture)texture);

            SlimDX.Vector4 color = new SlimDX.Vector4(1f, 1f, 1f, 1f);

            Vertex topLeft = new Vertex { Position = new SlimDX.Vector3(windowCoords.X, windowCoords.Y, 0f), TexCoord = new SlimDX.Vector2(0f, 0f), Color = color },
                bottomLeft = new Vertex { Position = new SlimDX.Vector3(windowCoords.X, windowCoords.Y + texture.Size.Y, 0f), TexCoord = new SlimDX.Vector2(0f, 1f), Color = color },
                topRight = new Vertex { Position = new SlimDX.Vector3(windowCoords.X + texture.Size.X, windowCoords.Y, 0f), TexCoord = new SlimDX.Vector2(1f, 0f), Color = color },
                bottomRight = new Vertex { Position = new SlimDX.Vector3(windowCoords.X + texture.Size.X, windowCoords.Y + texture.Size.Y, 0f), TexCoord = new SlimDX.Vector2(1f, 1f), Color = color };

            vertices.Add(topLeft);
            vertices.Add(bottomLeft);
            vertices.Add(topRight);

            vertices.Add(bottomLeft);
            vertices.Add(bottomRight);
            vertices.Add(topRight);
        }

        public void DrawTexture(Texture texture, Vector2 windowCoords, Color color, Vector2 scale, Vector2 origin, float rotation, Rectangle textureRect)
        {
            SetTexture((D3DTexture)texture);

            var m = SlimDX.Matrix.Translation(-origin.X, -origin.Y, 0f) *
                SlimDX.Matrix.Scaling(scale.X, scale.Y, 1f) *
                SlimDX.Matrix.RotationZ(rotation / 180f * Mathf.Pi) *
                SlimDX.Matrix.Translation(windowCoords.X, windowCoords.Y, 0f);

            float rX = textureRect.Left / texture.Size.X;
            float rY = textureRect.Top / texture.Size.Y;
            float rX2 = textureRect.Right / texture.Size.X;
            float rY2 = textureRect.Bottom / texture.Size.Y;

            SlimDX.Vector4 outColor = new SlimDX.Vector4(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);

            Vertex topLeft = new Vertex { Position = SlimDX.Vector3.TransformCoordinate(new SlimDX.Vector3(0f, 0f, 0f), m), TexCoord = new SlimDX.Vector2(rX, rY), Color = outColor },
                bottomLeft = new Vertex { Position = SlimDX.Vector3.TransformCoordinate(new SlimDX.Vector3(0f, textureRect.Height, 0f), m), TexCoord = new SlimDX.Vector2(rX, rY2), Color = outColor },
                topRight = new Vertex { Position = SlimDX.Vector3.TransformCoordinate(new SlimDX.Vector3(textureRect.Width, 0f, 0f), m), TexCoord = new SlimDX.Vector2(rX2, rY), Color = outColor },
                bottomRight = new Vertex { Position = SlimDX.Vector3.TransformCoordinate(new SlimDX.Vector3(textureRect.Width, textureRect.Height, 0f), m), TexCoord = new SlimDX.Vector2(rX2, rY2), Color = outColor };

            vertices.Add(topLeft);
            vertices.Add(bottomLeft);
            vertices.Add(topRight);

            vertices.Add(bottomLeft);
            vertices.Add(bottomRight);
            vertices.Add(topRight);
        }

        public override void ResizeBackBuffer(Vector2 newResolution)
        {
            renderTarget.Dispose();

            swapChain.ResizeBuffers(2, 0, 0, Format.R8G8B8A8_UNorm, SwapChainFlags.AllowModeSwitch);
            using (var resource = Resource.FromSwapChain<Texture2D>(swapChain, 0))
                renderTarget = new RenderTargetView(Device, resource);

            Context.OutputMerger.SetTargets(renderTarget);
            Context.Rasterizer.SetViewports(new Viewport(0.0f, 0.0f, newResolution.X, newResolution.Y));
        }
    }
}
