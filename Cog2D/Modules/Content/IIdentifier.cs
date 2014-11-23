using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Content
{
    public interface IIdentifier
    {
        long Id { get; set; }
        bool IsLocal { get; }
        bool IsGlobal { get; }
    }
}
