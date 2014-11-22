using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Audio
{
    public class DefaultAudioModule : IAudioModule
    {
        public SoundEffect Load(byte[] data)
        {
            return null;
        }

        public SoundEffect Load(string file)
        {
            return null;
        }
    }
}
