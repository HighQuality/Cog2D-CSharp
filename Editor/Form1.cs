using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Square.Runner
{
    public partial class Form1 : Form
    {
        AppDomain GameDomain;

        public Form1()
        {
            InitializeComponent();
        }

        private void loadGameButton_Click(object sender, EventArgs e)
        {
            GameDomain = AppDomain.CreateDomain("Game");

            GameDomain.ExecuteAssembly("..\\Game.exe");
        }
    }
}
