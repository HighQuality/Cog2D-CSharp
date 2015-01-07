using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Cog.Modules.Window
{
    public partial class WFWindow : Form
    {
        public event Action UserClosing;
        public bool RerouteClose = true;

        public WFWindow()
        {
            InitializeComponent();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (e.CloseReason == CloseReason.UserClosing && RerouteClose)
            {
                if (UserClosing != null)
                    UserClosing();
                e.Cancel = true;
            }
        }
    }
}
