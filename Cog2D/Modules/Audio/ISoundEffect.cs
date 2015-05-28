using Cog.Modules.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Audio
{
    public abstract class SoundEffect : Resource
    {
        /// <summary>
        /// Creates an instance of the sound without playing it.
        /// Set ISoundInstance.Paused to false to start playing the sound.
        /// </summary>
        /// <returns></returns>
        public abstract ISoundInstance CreateInstance();
        /// <summary>
        /// Creates and plays an instance of the sound.
        /// </summary>
        public abstract ISoundInstance Play();
        /// <summary>
        /// Creates and loops an instance of the sound.
        /// </summary>
        public abstract ISoundInstance Loop();
    }
}
