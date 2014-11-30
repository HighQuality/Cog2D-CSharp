using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Content
{
    interface ISynchronizedList
    {
        void InsertCommand(object value, int index);
        void AddCommand(object value);
        void SetCommand(int index, object value);
    }
}
