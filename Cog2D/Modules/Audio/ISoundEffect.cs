using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Audio
{
    public interface ISoundEffect : IDisposable
    {
        /// <summary>
        /// Creates an instance of the sound without playing it.
        /// Set ISoundInstance.Paused to false to start playing the sound.
        /// </summary>
        /// <returns></returns>
        ISoundInstance CreateInstance();
        /// <summary>
        /// Creates and plays an instance of the sound.
        /// </summary>
        ISoundInstance Play();
    }
}
