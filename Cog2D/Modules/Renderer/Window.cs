using Cog.Modules.EventHost;
using Cog.Modules.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace Cog.Modules.Renderer
{
    public abstract class Window
    {
        private IGameWindow window;
        public IntPtr Handle;

        private Dictionary<Keys, Action> keyUpEvents = new Dictionary<Keys, Action>();
        private LinkedList<Keys> pressedKeys = new LinkedList<Keys>();

        private static Dictionary<Keys, Keyboard.Key> keymap = new Dictionary<Keys, Keyboard.Key>
        {
            #region Keyboard Definitions
            { Keys.Space, Keyboard.Key.Space },
            { Keys.Back, Keyboard.Key.Backspace },
            { Keys.Left, Keyboard.Key.Left },
            { Keys.Right, Keyboard.Key.Right },
            { Keys.Up, Keyboard.Key.Up },
            { Keys.Down, Keyboard.Key.Down },
            { Keys.LShiftKey, Keyboard.Key.LShift },
            { Keys.RShiftKey, Keyboard.Key.RShift },
            { Keys.LControlKey, Keyboard.Key.LCtrl },
            { Keys.RControlKey, Keyboard.Key.RCtrl },
            { Keys.Escape, Keyboard.Key.Escape },
            { Keys.Tab, Keyboard.Key.Tab },
            { Keys.Return, Keyboard.Key.Return },

            { Keys.A, Keyboard.Key.A },
            { Keys.B, Keyboard.Key.B },
            { Keys.C, Keyboard.Key.C },
            { Keys.D, Keyboard.Key.D },
            { Keys.E, Keyboard.Key.E },
            { Keys.F, Keyboard.Key.F },
            { Keys.G, Keyboard.Key.G },
            { Keys.H, Keyboard.Key.H },
            { Keys.I, Keyboard.Key.I },
            { Keys.J, Keyboard.Key.J },
            { Keys.K, Keyboard.Key.K },
            { Keys.L, Keyboard.Key.L },
            { Keys.M, Keyboard.Key.M },
            { Keys.N, Keyboard.Key.N },
            { Keys.O, Keyboard.Key.O },
            { Keys.P, Keyboard.Key.P },
            { Keys.Q, Keyboard.Key.Q },
            { Keys.R, Keyboard.Key.R },
            { Keys.S, Keyboard.Key.S },
            { Keys.T, Keyboard.Key.T },
            { Keys.U, Keyboard.Key.U },
            { Keys.V, Keyboard.Key.V },
            { Keys.W, Keyboard.Key.W },
            { Keys.X, Keyboard.Key.X },
            { Keys.Y, Keyboard.Key.Y },
            { Keys.Z, Keyboard.Key.Z },

            { Keys.F1, Keyboard.Key.F1 },
            { Keys.F2, Keyboard.Key.F2 },
            { Keys.F3, Keyboard.Key.F3 },
            { Keys.F4, Keyboard.Key.F4 },
            { Keys.F5, Keyboard.Key.F5 },
            { Keys.F6, Keyboard.Key.F6 },
            { Keys.F7, Keyboard.Key.F7 },
            { Keys.F8, Keyboard.Key.F8 },
            { Keys.F9, Keyboard.Key.F9 },
            { Keys.F10, Keyboard.Key.F10 },
            { Keys.F11, Keyboard.Key.F11 },
            { Keys.F12, Keyboard.Key.F12 },

            { Keys.D0, Keyboard.Key.Num0 },
            { Keys.D1, Keyboard.Key.Num1 },
            { Keys.D2, Keyboard.Key.Num2 },
            { Keys.D3, Keyboard.Key.Num3 },
            { Keys.D4, Keyboard.Key.Num4 },
            { Keys.D5, Keyboard.Key.Num5 },
            { Keys.D6, Keyboard.Key.Num6 },
            { Keys.D7, Keyboard.Key.Num7 },
            { Keys.D8, Keyboard.Key.Num8 },
            { Keys.D9, Keyboard.Key.Num9 }, 
            #endregion
        };

        private static Dictionary<Keyboard.Key, Keys> reverseKeyMap;

        private static Dictionary<MouseButtons, EventHost.Mouse.Button> wfbuttonToCogButton = new Dictionary<MouseButtons, EventHost.Mouse.Button>
        {
            { MouseButtons.Left, EventHost.Mouse.Button.Left },
            { MouseButtons.Right, EventHost.Mouse.Button.Right },
            { MouseButtons.Middle, EventHost.Mouse.Button.Middle }
        };

        internal static Func<IGameWindow> CreateWindow;
        internal Vector2 MinimumResolution { get { return new Vector2(window.Form.MinimumSize.Width, window.Form.MinimumSize.Height); } set { window.Form.MinimumSize = new System.Drawing.Size((int)value.X, (int)value.Y); } }

        public Window(string title, int width, int height, WindowStyle style)
        {
            if (CreateWindow != null)
                window = CreateWindow();
            else
                window = new WFWindow();
            Handle = window.GameControl.Handle;
            window.Form.Hide();
            Visible = false;
            window.Form.Text = title;
            window.Form.MinimumSize = new System.Drawing.Size((int)Engine.MinimumResolution.X, (int)Engine.MinimumResolution.Y);
            window.Form.ClientSize = new System.Drawing.Size(width, height);
            // window.FormBorderStyle = FormBorderStyle.Fixed3D;
            // window.MaximizeBox = false;
            window.Form.FormClosed += (_, __) => IsOpen = false;
            window.GameControl.MouseMove += (s, par) =>
            {
                _mousePosition = new Vector2(par.Location.X, par.Location.Y);
            };
            window.UserClosing += () =>
            {
                Engine.EventHost.GetEvent<CloseButtonEvent>().Trigger(new CloseButtonEvent(this));
            };
            window.GameControl.MouseDown += Window_MouseDown;
            window.GameControl.SizeChanged += GameControl_SizeChanged;
            window.GameControl.MouseUp += Window_MouseUp;
            window.GameControl.KeyPress += Window_KeyPress;
            window.GameControl.KeyDown += Window_KeyDown;
            IsOpen = true;

            window.GameControl.Focus();

            if (reverseKeyMap == null)
            {
                reverseKeyMap = new Dictionary<Keyboard.Key, Keys>();
                foreach (var pair in keymap)
                    reverseKeyMap[pair.Value] = pair.Key;
            }
        }

        void GameControl_SizeChanged(object sender, EventArgs e)
        {
            ResizeBackBuffer(new Vector2(window.GameControl.Size.Width, window.GameControl.Size.Height));
            Engine.EventHost.GetEvent<ResolutionChangedEvent>().Trigger(new ResolutionChangedEvent(window.GameControl.Size.Width, window.GameControl.Size.Height));
        }

        private void Window_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            window.GameControl.Focus();
            
            EventHost.Mouse.Button cogButton;
            if (wfbuttonToCogButton.TryGetValue(e.Button, out cogButton))
                Cog.Modules.EventHost.Mouse.SetDown(cogButton);
        }

        private void Window_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            EventHost.Mouse.Button cogButton;
            if (wfbuttonToCogButton.TryGetValue(e.Button, out cogButton))
                Cog.Modules.EventHost.Mouse.SetReleased(cogButton);
        }

        private Keyboard.Key WinKeyToCog(Keys key)
        {
            Keyboard.Key output;
            if (keymap.TryGetValue(key, out output))
                return output;
            return Keyboard.Key.Unknown;
        }

        private void Window_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (!keyUpEvents.ContainsKey(e.KeyCode))
            {
                var newKey = WinKeyToCog(e.KeyCode);
                if (newKey == Keyboard.Key.Unknown)
                {
                    Debug.Error(e.KeyCode.ToString() + " is not mapped to a Cog.Keyboard.Key");
                    return;
                }

                var keyParameters = new KeyDownEvent(this, newKey);
                // If a specific key handler didn't intercept the event, send it on to the generic key handler event
                if (!Engine.EventHost.GetEvent<KeyDownEvent>(newKey).Trigger(keyParameters))
                    Engine.EventHost.GetEvent<KeyDownEvent>().Trigger(keyParameters);
                keyUpEvents.Add(e.KeyCode, keyParameters.KeyUpEvent);
                pressedKeys.AddLast(e.KeyCode);
            }
        }

        private void Window_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar))
                Engine.EventHost.GetEvent<TextEnteredEvent>().Trigger(new TextEnteredEvent(e.KeyChar));
        }

        /// <summary>
        /// Gets or Sets the title of the window
        /// </summary>
        public string Title { get { return window.Form.Text; } set { window.Form.Text = value; } }
        /// <summary>
        /// Gets or Sets the size of the window
        /// </summary>
        public Vector2 Resolution { get { return new Vector2(window.GameControl.Size.Width, window.GameControl.Size.Height); } }
        /// <summary>
        /// Gets or Sets the position of the window
        /// </summary>
        public Vector2 Position { get { return new Vector2(window.Form.Location.X, window.Form.Location.Y); } set { window.Form.Location = new System.Drawing.Point((int)value.X, (int)value.Y); } }
        private Vector2 _mousePosition;
        public Vector2 MousePosition { get { return _mousePosition; } set { System.Windows.Forms.Cursor.Position = window.GameControl.PointToScreen(new System.Drawing.Point((int)value.X, (int)value.Y)); } }
        /// <summary>
        /// Gets or Sets the visibility of the window
        /// </summary>
        public bool Visible { get { return window.Form.Visible; } set { window.Form.Visible = true; } }
        /// <summary>
        /// Gets if the window is still open
        /// </summary>
        public bool IsOpen { get; private set; }
        public abstract bool VerticalSynchronization { get; set; }

        /// <summary>
        /// Gets the current render target
        /// </summary>
        public abstract IRenderTarget RenderTarget { get; }

        /// <summary>
        /// Commands the window to dispatch it's queued events
        /// </summary>
        public void DispatchEvents()
        {
            System.Windows.Forms.Application.DoEvents();

            var node = pressedKeys.First;
            while (node != null)
            {
                bool isDown = false;

                try
                {
                    isDown = System.Windows.Input.Keyboard.IsKeyDown(KeyInterop.KeyFromVirtualKey((int)node.Value));
                }
                catch (Exception) { }

                if (!isDown)
                {
                    var action = keyUpEvents[node.Value];
                    if (action != null)
                        action();
                    keyUpEvents.Remove(node.Value);
                    pressedKeys.Remove(node);
                }
                node = node.Next;
            }
        }
        /// <summary>
        /// Commands the window to clear the back buffer to the specified color
        /// </summary>
        /// <param name="color">The color to clear to</param>
        public abstract void Clear(Color color);
        /// <summary>
        /// Commands the window to switch the backbuffer displaying rendered content
        /// </summary>
        public abstract void Display();
        /// <summary>
        /// Closes the window
        /// </summary>
        public void Close()
        {
            // No longer reroute close to the CloseButtonEvent
            window.RerouteClose = false;
            window.Form.Close();
        }

        public abstract void ResizeBackBuffer(Vector2 newResolution);
        
        /// <summary>
        /// Checks whether a key is down or not.
        /// </summary>
        public bool IsKeyDown(Keyboard.Key key)
        {
            throw new NotImplementedException();
        }
    }
}
