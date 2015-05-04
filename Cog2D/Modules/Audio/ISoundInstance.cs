using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Audio
{
    public interface ISoundInstance
    {
        /// <summary>
        /// Gets the Sound Effect which this sound instance is playing
        /// </summary>
        SoundEffect SoundEffect { get; }

        /// <summary>
        /// Gets or sets whether or not the sound is paused
        /// </summary>
        bool Paused { get; set; }
        
        /// <summary>
        /// Gets or sets the volume of the sound
        /// 0 to 1
        /// Default value is 1
        /// </summary>
        float Volume { get; set; }
        
        /*
        /// <summary>
        /// Gets or sets the panning of the sound
        /// -1 (Left) to 1 (Right)
        /// Default value is 0
        /// </summary>
        float Panning { get; set; }
        */

        /*
        /// <summary>
        /// Gets or sets the pitch of the sound.
        /// Default value is 1
        /// </summary>
        float Pitch { get; set; }
        */

        /// <summary>
        /// Stops the sound from playing.
        /// A sound instance that has been stopped may not be resumed.
        /// </summary>
        void Stop();
    }
}
