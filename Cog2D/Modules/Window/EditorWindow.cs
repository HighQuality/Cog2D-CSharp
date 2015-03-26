using Cog.Editor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Cog.Modules.EventHost;

namespace Cog.Modules.Window
{
    public partial class EditorWindow : Form, IGameWindow
    {
        public Control GameControl { get { return pictureBox1; } }
        public Form Form { get { return this; } }
        public Action UserClosing { get; set; }
        public bool RerouteClose { get; set; }
        public Vector2 MinimumResolution { get { return new Vector2(MinimumSize.Width, MinimumSize.Height); } set { MinimumSize = new Size((int)value.X, (int)value.Y); } }

        public EditorWindow()
        {
            RerouteClose = true;
            InitializeComponent();
        }

        private void EditorWindow_Load(object sender, EventArgs e)
        {
            foreach (var _type in Engine.EnumerateGameObjectTypes().OrderBy(o => o.Name))
            {
                var type = _type;
                if (type.GetCustomAttributes(typeof(HideInEditorAttribute), true).Length == 0)
                {
                    var item = createGlobalToolStripMenuItem.DropDownItems.Add(type.Name);
                    item.Click += (ev, s) =>
                    {
                        Engine.InvokeTimed(0f, offset =>
                        {
                            Engine.SceneHost.CurrentScene.CreateObjectFromType(type, null, Mouse.Location - Engine.Resolution / 2f);
                        });
                    };
                }
            }
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
