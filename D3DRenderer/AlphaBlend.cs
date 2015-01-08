using Cog.Modules.Renderer;
using SlimDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace D3DRenderer
{
    class AlphaBlend : BlendMode
    {
        private BlendState blendState;

        public AlphaBlend()
        {
            var desc = new BlendStateDescription()
            {
                IndependentBlendEnable = false,
                AlphaToCoverageEnable = false
            };

            desc.RenderTargets[0] = new RenderTargetBlendDescription()
            {
                BlendEnable = true,
                SourceBlend = BlendOption.SourceAlpha,
                DestinationBlend = BlendOption.InverseSourceAlpha,
                BlendOperation = BlendOperation.Add,
                SourceBlendAlpha = BlendOption.One,
                DestinationBlendAlpha = BlendOption.InverseSourceAlpha,
                BlendOperationAlpha = BlendOperation.Add,
                RenderTargetWriteMask = ColorWriteMaskFlags.All
            };

            blendState = BlendState.FromDescription(D3DWindow.Device, desc);
        }

        protected override void Set()
        {
            // Draw vertices queued to be drawn with the old blend state
            D3DWindow.DrawBuffer();

            D3DWindow.Context.OutputMerger.BlendState = blendState;
        }
    }
}
