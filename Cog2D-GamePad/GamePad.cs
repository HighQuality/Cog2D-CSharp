using Cog;
using Cog.Modules.EventHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cog2D.GamePad
{
    public static class GamePad
    {
        private static GamePadState[] previousState;

        public static void Initialize()
        {
            previousState = new GamePadState[(int)GamePadIndex.Count];
            Engine.EventHost.RegisterEvent<InputUpdateEvent>(0, Update);
        }

        private static void Update(InputUpdateEvent ev)
        {
            for (GamePadIndex playerIndex = 0; playerIndex < GamePadIndex.Count; playerIndex++)
            {
                GamePadState oldState = previousState[(int)playerIndex];
                GamePadState state = GamePadState.New(playerIndex);

                if (state.IsConnected || oldState.IsConnected)
                {
                    if (!oldState.IsConnected && state.IsConnected)
                    {
                        if (!Engine.EventHost.GetEvent<GamePadConnectedEvent>().Trigger(new GamePadConnectedEvent(playerIndex)))
                        {
                            Engine.EventHost.GetEvent<GamePadConnectedEvent>(playerIndex).Trigger(new GamePadConnectedEvent(playerIndex));
                        }
                    }
                    else if (oldState.IsConnected && !state.IsConnected)
                    {
                        if (!Engine.EventHost.GetEvent<GamePadDisconnectedEvent>().Trigger(new GamePadDisconnectedEvent(playerIndex)))
                        {
                            Engine.EventHost.GetEvent<GamePadDisconnectedEvent>(playerIndex).Trigger(new GamePadDisconnectedEvent(playerIndex));
                        }
                    }

                    for (GamePadButton button = 0; button < GamePadButton.Count; button++)
                    {
                        if (oldState.ButtonIsUp(button) && state.ButtonIsDown(button))
                        {
                            if (!Engine.EventHost.GetEvent<GamePadButtonPressed>().Trigger(new GamePadButtonPressed(playerIndex, button)))
                            {
                                Engine.EventHost.GetEvent<GamePadButtonPressed>(playerIndex).Trigger(new GamePadButtonPressed(playerIndex, button));
                            }
                        }
                        else if (oldState.ButtonIsDown(button) && state.ButtonIsUp(button))
                        {
                            if (!Engine.EventHost.GetEvent<GamePadButtonReleased>().Trigger(new GamePadButtonReleased(playerIndex, button)))
                            {
                                Engine.EventHost.GetEvent<GamePadButtonReleased>(playerIndex).Trigger(new GamePadButtonReleased(playerIndex, button));
                            }
                        }
                    }

                    if (!Engine.EventHost.GetEvent<GamePadUpdateEvent>().Trigger(new GamePadUpdateEvent(state)))
                    {
                        Engine.EventHost.GetEvent<GamePadUpdateEvent>(playerIndex).Trigger(new GamePadUpdateEvent(state));
                    }
                }

                previousState[(int)playerIndex] = state;
            }
        }
    }
}
