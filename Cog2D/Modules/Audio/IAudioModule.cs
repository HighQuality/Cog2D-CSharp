using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Audio
{
    public interface IAudioModule
    {
        ISoundEffect Load(string file);
    }
}
