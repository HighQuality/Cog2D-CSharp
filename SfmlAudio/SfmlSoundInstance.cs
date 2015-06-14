using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.SfmlAudio
{
    class SfmlSoundInstance : Cog.Modules.Audio.ISoundInstance
    {
        public Cog.Modules.Audio.SoundEffect SoundEffect { get; private set; }
        internal SFML.Audio.Sound Sound;

        private bool _isPaused = true;
        public bool Paused
        {
            get { return _isPaused; }
            set { if (value && !_isPaused) Sound.Pause(); if (!value && _isPaused) Sound.Play(); _isPaused = value; }
        }
        public float Volume
        {
            get { return Sound.Volume / 100f; }
            set { Sound.Volume = value * 100f; }
        }
        public float Pitch
        {
            get { return Sound.Pitch; }
            set { Sound.Pitch = value; }
        }

        internal SfmlSoundInstance(SfmlSoundEffect sfx)
            : this(sfx, false)
        {
        }

        internal SfmlSoundInstance(SfmlSoundEffect sfx, bool looped)
        {
            Sound = new SFML.Audio.Sound(sfx.Buffer);
            Sound.Loop = looped;

            SoundEffect = sfx;
            Volume = 1f;
        }

        public void Stop()
        {
            Sound.Stop();
        }
    }
}
