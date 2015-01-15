using Cog.Modules.Renderer;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XnaRenderer
{
    class AdditiveBlend : BlendMode
    {
        public AdditiveBlend()
        {

        }

        protected override void Set()
        {
            XnaWindow.Begin(XnaWindow.CurrentMatrix, BlendState.Additive);
        }
    }
}
