using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Content
{
    public class BaseObject : GameObject
    {
        public override Vector2 WorldCoord
        {
            get
            {
                return LocalCoord;
            }

            set
            {
                LocalCoord = value;
            }
        }

        public override Angle WorldRotation
        {
            get
            {
                return LocalRotation;
            }

            set
            {
                LocalRotation = value;
            }
        }

        public override Vector2 WorldScale
        {
            get
            {
                return LocalScale;
            }

            set
            {
                LocalScale = value;
            }
        }

        public BaseObject()
        {

        }
    }
}
