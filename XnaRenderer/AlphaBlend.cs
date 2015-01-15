using Cog.Modules.Renderer;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XnaRenderer
{
    class AlphaBlend : BlendMode
    {
        public AlphaBlend()
        {

        }

        protected override void Set()
        {
            XnaWindow.Begin(XnaWindow.CurrentMatrix, BlendState.AlphaBlend);
        }
    }
}
