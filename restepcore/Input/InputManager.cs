using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;
using OpenTK.Input;

namespace restep.Input
{
    public enum ControlState
    {
        OFF,
        PRESS,
        RELEASE,
        HOLD
    }

    public delegate void KeyEvent(Key keyCode);
    public delegate void MouseEvent(MouseButton buttonCode, Vector2 location);
    

    public class InputManager
    {
        static InputManager()
        {
            keyStateList = new KeyControl[131];
            buttonStates = new MButtonControl[2];

            buttonStates[0] = new MButtonControl(MouseButton.Left);
            buttonStates[1] = new MButtonControl(MouseButton.Right);

            for (int a = 0; a < keyStateList.Length; a++)
            {
                keyStateList[a] = new KeyControl((Key)a);
            }
        }

        public static event KeyEvent AnyKeyPress;
        public static event KeyEvent AnyKeyRelease;
        public static event KeyEvent AnyKeyHold;

        #region Key Control
        private class KeyControl
        {
            public KeyControl(Key key)
            {
                this.key = key;
                State = ControlState.OFF;
            }

            private Key key;
            public ControlState State;
            public event KeyEvent KeyPress;
            public event KeyEvent KeyRelease;
            public event KeyEvent KeyHold;

            public void InvokePress()
            {
                KeyPress?.Invoke(key);
            }

            public void InvokeRelease()
            {
                KeyRelease?.Invoke(key);
            }

            public void InvokeHold()
            {
                KeyHold?.Invoke(key);
            }
        }

        private static KeyControl[] keyStateList;

        public static void AddPressCallback(Key key, KeyEvent callback)
        {
            keyStateList[(int)key].KeyPress += callback;
        }

        public static void AddReleaseCallback(Key key, KeyEvent callback)
        {
            keyStateList[(int)key].KeyRelease += callback;
        }

        public static void AddHoldCallback(Key key, KeyEvent callback)
        {
            keyStateList[(int)key].KeyHold += callback;
        }

        public static ControlState GetKeyState(Key key)
        {
            return keyStateList[(int)key].State;
        }
        #endregion

        #region Mouse Control
        internal class MButtonControl
        {
            public MButtonControl(MouseButton button)
            {
                this.button = button;
            }

            private MouseButton button;
            public ControlState ButtonState;
            public event MouseEvent ButtonPress;
            public event MouseEvent ButtonRelease;

            public void InvokePress()
            {
                ButtonPress?.Invoke(button, InputManager.CursorLoc);
            }

            public void InvokeRelease()
            {
                ButtonRelease?.Invoke(button, InputManager.CursorLoc);
            }
        }

        private static MButtonControl[] buttonStates;

        public static Vector2 CursorLoc { get; set; }

        private static int mb(MouseButton button) =>
            (button == MouseButton.Left ? 0 : (button == MouseButton.Right ? 1 : -1));

        public static void AddPressCallback(MouseButton button, MouseEvent callback)
        {
            if (mb(button) != -1)
            {
                buttonStates[mb(button)].ButtonPress += callback;
            }
        }

        public static void AddReleaseCallback(MouseButton button, MouseEvent callback)
        {
            if (mb(button) != -1)
            {
                buttonStates[mb(button)].ButtonRelease += callback;
            }
        }

        public static ControlState GetMouseButtonState(MouseButton button)
        {
            if (mb(button) != -1)
            {
                return buttonStates[mb(button)].ButtonState;
            }
            return ControlState.OFF;
        }

        #endregion

        private static void UpdateMouse()
        {
            MouseState state = Mouse.GetState();
            if(state.LeftButton == ButtonState.Pressed)
            {
                if(buttonStates[0].ButtonState == ControlState.OFF || buttonStates[0].ButtonState == ControlState.RELEASE)
                {
                    buttonStates[0].ButtonState = ControlState.PRESS;
                    buttonStates[0].InvokePress();
                }
                if(buttonStates[0].ButtonState == ControlState.PRESS)
                {
                    buttonStates[0].ButtonState = ControlState.HOLD;
                }
            }
            else if(state.LeftButton == ButtonState.Released)
            {
                if (buttonStates[0].ButtonState == ControlState.PRESS || buttonStates[0].ButtonState == ControlState.HOLD)
                {
                    buttonStates[0].ButtonState = ControlState.RELEASE;
                    buttonStates[0].InvokeRelease();
                }
                if (buttonStates[0].ButtonState == ControlState.RELEASE)
                {
                    buttonStates[0].ButtonState = ControlState.OFF;
                }
            }

            if (state.RightButton == ButtonState.Pressed)
            {
                if (buttonStates[1].ButtonState == ControlState.OFF || buttonStates[1].ButtonState == ControlState.RELEASE)
                {
                    buttonStates[1].ButtonState = ControlState.PRESS;
                    buttonStates[1].InvokePress();
                }
                if (buttonStates[1].ButtonState == ControlState.PRESS)
                {
                    buttonStates[1].ButtonState = ControlState.HOLD;
                }
            }
            else if (state.RightButton == ButtonState.Released)
            {
                if (buttonStates[1].ButtonState == ControlState.PRESS || buttonStates[1].ButtonState == ControlState.HOLD)
                {
                    buttonStates[1].ButtonState = ControlState.RELEASE;
                    buttonStates[1].InvokeRelease();
                }
                if (buttonStates[1].ButtonState == ControlState.RELEASE)
                {
                    buttonStates[1].ButtonState = ControlState.OFF;
                }
            }
        }

        private static void UpdateKeyboard()
        {
            KeyboardState state = Keyboard.GetState();

            for (int a = 0; a < keyStateList.Length; a++)
            {
                if (state.IsKeyUp((Key)a))
                {
                    if (keyStateList[a].State == ControlState.PRESS || keyStateList[a].State == ControlState.HOLD)
                    {
                        keyStateList[a].State = ControlState.RELEASE;
                        keyStateList[a].InvokeRelease();
                        AnyKeyRelease?.Invoke((Key)a);
                    }
                    else if (keyStateList[a].State == ControlState.RELEASE)
                    {
                        keyStateList[a].State = ControlState.OFF;
                    }
                }
                else if (state.IsKeyDown((Key)a))
                {
                    if (keyStateList[a].State == ControlState.PRESS || keyStateList[a].State == ControlState.HOLD)
                    {
                        keyStateList[a].State = ControlState.HOLD;
                        keyStateList[a].InvokeHold();
                        AnyKeyHold?.Invoke((Key)a);
                    }
                    if (keyStateList[a].State == ControlState.OFF || keyStateList[a].State == ControlState.RELEASE)
                    {
                        keyStateList[a].State = ControlState.PRESS;
                        keyStateList[a].InvokePress();
                        AnyKeyPress?.Invoke((Key)a);
                    }
                }
            }
        }

        public static void UpdateStates()
        {
            UpdateKeyboard();
            UpdateMouse();
        }

        public static void UpdateCursor(GameWindow window)
        {
            MouseState state = Mouse.GetCursorState();

            var point = window.PointToClient(new System.Drawing.Point(state.X, state.Y));
            CursorLoc = new Vector2(point.X, (Framework.RestepGlobals.ContentAreaSize.Y - point.Y));
        }
    }
}
