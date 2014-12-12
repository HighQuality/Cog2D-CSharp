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

        public Action OnPressed,
            OnReleased;

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
            if (!IsDown)
            {
                args.KeyUpEvent = KeyUp;
                IsDown = true;

                if (OnPressed != null)
                    OnPressed();

                if (RelayMode == CaptureRelayMode.ServerRelay || RelayMode == CaptureRelayMode.ServerClientRelay)
                {
                }
            }
            args.Intercept = true;
        }

        private void KeyUp()
        {
            if (IsDown)
            {
                IsDown = false;

                if (OnReleased != null)
                    OnReleased();
            }

            if (RelayMode == CaptureRelayMode.ServerRelay || RelayMode == CaptureRelayMode.ServerClientRelay)
            {
            }
        }
    }
}
