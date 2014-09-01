using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Square;
using Square.SfmlRenderer;
using Square.Modules.Renderer;

namespace TestGame
{
    class Program
    {
        static void Main(string[] args)
        {
            Engine.StartGame<SfmlRenderer>("Test Game", 640, 480, WindowStyle.Default);
        }
    }
}
