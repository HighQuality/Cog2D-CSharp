using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Cog.Modules.Window
{
    interface IGameWindow
    {
        Control GameControl { get; }
        Form Form { get; }
        bool RerouteClose { get; set; }
        Action UserClosing { get; set; }
    }
}
