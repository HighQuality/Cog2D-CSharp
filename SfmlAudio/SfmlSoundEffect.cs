using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.SfmlAudio
{
    class SfmlSoundEffect : Cog.Modules.Audio.SoundEffect
    {
        internal SFML.Audio.SoundBuffer Buffer;

        internal SfmlSoundEffect(byte[] data)
        {
            Buffer = new SFML.Audio.SoundBuffer(data);
        }

        public override Cog.Modules.Audio.ISoundInstance CreateInstance()
        {
            return new SfmlSoundInstance(this);
        }

        public override Cog.Modules.Audio.ISoundInstance Play()
        {
            var snd = CreateInstance();
            snd.Paused = false;
            return snd;
        }

        public override void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
                Buffer.Dispose();
        }

        ~SfmlSoundEffect()
        {
            Dispose(false);
        }
    }
}
