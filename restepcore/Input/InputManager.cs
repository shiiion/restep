using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Input;

namespace restep.Input
{
    public enum KeyState
    {
        OFF,
        PRESS,
        RELEASE,
        HOLD
    }

    public delegate void KeyEvent(int keyCode);

    internal class KeyControl
    {
        public KeyControl()
        {
            State = KeyState.OFF;
        }

        public KeyState State;
        public event KeyEvent KeyPress;
        public event KeyEvent KeyRelease;
        public event KeyEvent KeyHold;

        public void InvokePress(int index)
        {
            KeyPress?.Invoke(index);
        }

        public void InvokeRelease(int index)
        {
            KeyRelease?.Invoke(index);
        }

        public void InvokeHold(int index)
        {
            KeyHold?.Invoke(index);
        }
    }

    public class InputManager
    {
        private static KeyControl[] stateList;

        static InputManager()
        {
            stateList = new KeyControl[131];
            for(int a=0;a<stateList.Length;a++)
            {
                stateList[a] = new KeyControl();
            }
        }

        public static void UpdateStates()
        {
            KeyboardState state = Keyboard.GetState();
            for (int a=0;a<stateList.Length;a++)
            {
                if(state.IsKeyUp((Key)a))
                {
                    if(stateList[a].State == KeyState.PRESS || stateList[a].State == KeyState.HOLD)
                    {
                        stateList[a].State = KeyState.RELEASE;
                        stateList[a].InvokeRelease(a);
                    }
                    else if(stateList[a].State == KeyState.RELEASE)
                    {
                        stateList[a].State = KeyState.OFF;
                    }
                }
                else if(state.IsKeyDown((Key)a))
                {
                    if(stateList[a].State == KeyState.PRESS || stateList[a].State == KeyState.HOLD)
                    {
                        stateList[a].State = KeyState.HOLD;
                        stateList[a].InvokeHold(a);
                    }
                    if(stateList[a].State == KeyState.OFF || stateList[a].State == KeyState.RELEASE)
                    {
                        stateList[a].State = KeyState.PRESS;
                        stateList[a].InvokePress(a);
                    }
                }
            }
        }
    }
}
