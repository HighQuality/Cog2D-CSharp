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
    public partial class EditorWindow : Form
    {
        public Control GameControl;
        public event Action UserClosing;
        public bool RerouteClose = true;

        public EditorWindow()
        {
            InitializeComponent();
            GameControl = pictureBox1;
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
