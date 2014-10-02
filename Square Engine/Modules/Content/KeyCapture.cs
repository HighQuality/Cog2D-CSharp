using Square.Modules.EventHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Square.Modules.Content
{
    public class KeyCapture
    {
        private EventListener<KeyDownEvent> listener;
        public bool IsDown { get; private set; }
        public CaptureRelayMode RelayMode;
        public Keyboard.Key Key { get; private set; }

        internal KeyCapture(GameObject obj, Keyboard.Key key, int priority, CaptureRelayMode relayMode)
        {
            this.Key = key;
            listener = obj.RegisterEvent<KeyDownEvent>(Key, priority, KeyDown);
            this.RelayMode = relayMode;
        }

        private void KeyDown(KeyDownEvent args)
        {
            IsDown = true;
            args.KeyUpEvent = KeyUp;
            args.Intercept = true;

            if (RelayMode == CaptureRelayMode.ServerRelay || RelayMode == CaptureRelayMode.ServerClientRelay)
            {
            }
        }

        private void KeyUp()
        {
            IsDown = false;

            if (RelayMode == CaptureRelayMode.ServerRelay || RelayMode == CaptureRelayMode.ServerClientRelay)
            {
            }
        }
    }
}
