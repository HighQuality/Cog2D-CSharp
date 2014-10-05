using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Square
{
    public partial class SplashScreen : Form
    {
        private System.Drawing.Bitmap img;

        public SplashScreen(Square.Image image)
        {
            InitializeComponent();
            img = new System.Drawing.Bitmap(image.Width, image.Height);
            for (int x=image.Width - 1; x >= 0; x--)
            {
                for (int y=image.Height - 1; y >= 0; y--)
                {
                    var color = image.GetColor(x, y);
                    img.SetPixel(x, y, System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B));
                }
            }
        }

        private void SplashScreen_Load(object sender, EventArgs e)
        {
            Width = img.Width;
            Height = img.Height;
            CenterToScreen();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            using (var g = e.Graphics)
            {
                g.DrawImage(img, e.ClipRectangle);
            }
        }
    }
}
