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
    public partial class EditorWindow : Form, IGameWindow
    {
        public Control GameControl { get { return pictureBox1; } }
        public Form Form { get { return this; } }
        public Action UserClosing { get; set; }
        public bool RerouteClose { get; set; }

        public EditorWindow()
        {
            RerouteClose = true;
            InitializeComponent();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Console.WriteLine(comboBox1.SelectedIndex);
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

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                gameRightClickMenu.Show(pictureBox1, e.Location);
            }
        }
    }
}
