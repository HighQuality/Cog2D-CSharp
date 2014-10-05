using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Content
{
    public interface IContentModule
    {
        void LoadContent(string assemblyFileName);
        void UnloadContent();
    }
}
