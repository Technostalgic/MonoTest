using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Mono_Test {
    public static class Input {
        public static MouseState mouseState;
        public static KeyboardState keyboardState;
        public static Vector2 mousePos_previous { get { return lastMousePos; } }
        static Vector2 lastMousePos;
        static List<Keys> keyClickedState;
        static bool[] mouseClickedState;
        
        public static Vector2 mousePos() {
            return new Vector2(mouseState.X, mouseState.Y);
        }
        /// <summary>
        /// returns true if the mouse position is different than it was last tick
        /// </summary>
        /// <returns></returns>
        public static bool isMouseMoving() {
            return mousePos() != lastMousePos;
        }
        /// <summary>
        /// returns true if the specefied mouse button is pressed
        /// </summary>
        /// <param name="button">the specefied mouse button</param>
        public static bool isMousePressed(mouseButton button = mouseButton.Left) {
            switch (button) {
                case mouseButton.Left: return mouseState.LeftButton == ButtonState.Pressed;
                case mouseButton.Right: return mouseState.RightButton == ButtonState.Pressed;
                case mouseButton.Middle: return mouseState.MiddleButton == ButtonState.Pressed;
            }
            return false;
        }
        /// <summary>
        /// returns true the first tick the specified mouse button is pressed
        /// </summary>
        /// <param name="button">the specified mouse button</param>
        public static bool isMouseClicked(mouseButton button = mouseButton.Left) {
            return isMousePressed(button) && !mouseClickedState[(int)button];
        }
        /// <summary>
        /// returns true if the specified keyboard key is pressed
        /// </summary>
        /// <param name="key">the specified keyboard key</param>
        public static bool isKeyPressed(Keys key) {
            return keyboardState.IsKeyDown(key);
        }
        /// <summary>
        /// returns true the first tick the specified keyboard key is pressed
        /// </summary>
        /// <param name="key">the specified keyboard key</param>
        public static bool isKeyClicked(Keys key) {
            return isKeyPressed(key) && !keyClickedState.Contains(key);
        }

        /// <summary>
        /// refresshes the keyClickedState and mouseClickedState variables based on input
        /// </summary>
        static void refreshClickedState() {
            // refresh key clicks
            keyClickedState = new List<Keys>();

            // refresh mouse clicks
            for (int i = mouseClickedState.Length - 1; i >= 0; i--)
                mouseClickedState[i] = false;
        }
        /// <summary>
        /// sets the key and mouse click state for the upcoming tick
        /// </summary>
        static void setClickedState() {
            // adds pressed keys to clicked keys
            keyClickedState.AddRange(keyboardState.GetPressedKeys());

            // adds pressed mousebuttons to clicked mouseButtons
            for (int i = mouseClickedState.Length - 1; i >= 0; i--)
                mouseClickedState[i] = isMousePressed((mouseButton)i);
        }
        /// <summary>
        /// initializes the input manager
        /// </summary>
        public static void init() {
            keyClickedState = new List<Keys>();
            mouseClickedState = new bool[3];
        }
        /// <summary>
        /// refreshes the retrieved input from the input devices
        /// </summary>
        public static void refresh() {
            refreshClickedState();
            setClickedState();
            lastMousePos = mousePos();
            mouseState = Mouse.GetState();
            keyboardState = Keyboard.GetState();
        }

        public enum mouseButton {
            Left,
            Right,
            Middle
        }
    }
}
