using Cog.Modules.EventHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Content
{
    public class KeyCapture
    {
        private EventListener<KeyDownEvent> listener;
        public bool IsDown { get; private set; }
        public CaptureRelayMode RelayMode;

        private readonly int priority;
        private GameObject baseObject;

        private Keyboard.Key _key;
        public Keyboard.Key Key
        {
            get { return _key; }
            set
            {
                _key = value;

                if (listener != null)
                    listener.Cancel();
                listener = baseObject.RegisterEvent<KeyDownEvent>(Key, priority, KeyDown);
            }
        }

        public KeyCapture(GameObject obj, Keyboard.Key key, int priority, CaptureRelayMode relayMode)
        {
            this.baseObject = obj;
            this.priority = priority;
            this.Key = key;
            this.RelayMode = relayMode;
        }

        public void StopListen()
        {
            listener.Cancel();
            IsDown = false;
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
