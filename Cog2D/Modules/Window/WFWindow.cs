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
    public partial class WFWindow : Form, IGameWindow
    {
        public Action UserClosing { get; set; }
        public bool RerouteClose { get; set; }
        public Control GameControl { get { return pictureBox1; } }
        public Form Form { get { return this; } }
        public Vector2 MinimumResolution { get { return new Vector2(MinimumSize.Width, MinimumSize.Height); } set { MinimumSize = new Size((int)value.X, (int)value.Y); } }

        public WFWindow()
        {
            RerouteClose = true;
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
