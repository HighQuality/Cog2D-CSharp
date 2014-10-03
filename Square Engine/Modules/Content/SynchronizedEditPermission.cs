using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Square.Modules.Content
{
    struct SynchronizedEditPermission
    {
        public bool CanEdit;
        public bool RequireOwner;

        public SynchronizedEditPermission(bool canEdit, bool requireOwner)
        {
            this.CanEdit = canEdit;
            this.RequireOwner = requireOwner;
        }
    }
}
