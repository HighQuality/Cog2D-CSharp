using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Audio
{
    public class NoSoundInstance : ISoundInstance
    {
        public SoundEffect sfx;

        public bool Paused
        {
            get; set;
        }

        public SoundEffect SoundEffect
        {
            get
            {
                return sfx;
            }
        }

        public float Volume
        {
            get; set;
        }

        public void Stop()
        {
        }
    }

    public class NoSoundSoundEffect : SoundEffect
    {
        public override ISoundInstance CreateInstance()
        {
            return new NoSoundInstance { sfx = this };
        }

        public override void Dispose()
        {
        }

        public override ISoundInstance Loop()
        {
            return new NoSoundInstance { sfx = this };
        }

        public override ISoundInstance Play()
        {
            return new NoSoundInstance { sfx = this };
        }
    }

    public class DefaultAudioModule : IAudioModule
    {
        public SoundEffect Load(byte[] data)
        {
            return new NoSoundSoundEffect();
        }

        public SoundEffect Load(string file)
        {
            return new NoSoundSoundEffect();
        }
    }
}
