using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.SfmlAudio
{
    public class SfmlAudioModule : Cog.Modules.Audio.IAudioModule
    {
        public Cog.Modules.Audio.SoundEffect Load(string file)
        {
            return Load(File.ReadAllBytes(file));
        }

        public Cog.Modules.Audio.SoundEffect Load(byte[] data)
        {
            return new SfmlSoundEffect(data);
        }
    }
}
