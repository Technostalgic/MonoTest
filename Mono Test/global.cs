using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Mono_Test {
    /// <summary>
    /// represents controls that the player can use to interact with the program
    /// </summary>
    public enum controlCode {
        menuSelect, menuNext, menuPrevious,
        left, right, up, down, jump,
        use, attackPrimary, attackSecondary,
        pause
    }

    /// <summary>
    /// Used for global access to data and methods
    /// </summary>
    public static class global {
        /// <summary>
        /// runs on the first tick of the program, used to experiment and test output
        /// </summary>
        public static void debugLogInit() {
        }

        /// <summary>
        /// represents the total time elapsed since the game was first run
        /// </summary>
        private static TimeSpan ttime;
        /// <summary>
        /// Updates the coutner for totalTimeElapsed, ttime
        /// </summary>
        /// <param name="t">time to update it to</param>
        public static void updateTotalTimeElapsed(TimeSpan t) {
            ttime = t;
        }

        
        /// <summary>
        /// represents the current control scheme
        /// </summary>
        private static gameControl[] controlMap = new gameControl[Enum.GetNames(typeof(controlCode)).Length];
        /// <summary>
        /// applies the default control scheme
        /// </summary>
        public static void setDefaultControls() {
            controlMap[(int)controlCode.menuSelect] = gameControl.keyboardControl(Keys.Enter); 
            controlMap[(int)controlCode.menuNext] = gameControl.keyboardControl(Keys.Down);
            controlMap[(int)controlCode.menuPrevious] = gameControl.keyboardControl(Keys.Up); 
            controlMap[(int)controlCode.left] = gameControl.keyboardControl(Keys.Left);
            controlMap[(int)controlCode.right] = gameControl.keyboardControl(Keys.Right); 
            controlMap[(int)controlCode.up] = gameControl.keyboardControl(Keys.Up); 
            controlMap[(int)controlCode.down] = gameControl.keyboardControl(Keys.Down); 
            controlMap[(int)controlCode.jump] = gameControl.keyboardControl(Keys.Z); 
            controlMap[(int)controlCode.use] = gameControl.keyboardControl(Keys.Space); 
            controlMap[(int)controlCode.attackPrimary] = gameControl.keyboardControl(Keys.C); 
            controlMap[(int)controlCode.attackSecondary] = gameControl.keyboardControl(Keys.X);
            controlMap[(int)controlCode.pause] = gameControl.keyboardControl(Keys.P);
        }                                                                                                        

        /// <summary>
        /// the current control scheme
        /// </summary>
        public static gameControl[] controls { get { return controlMap; } }

        private static bool[] controlState = new bool[controls.Length];
        /// <summary>
        /// retreives user input to determine the state of whether controls are active or not
        /// </summary>
        public static void refreshControlState() {
            try {
                for (int i = controlState.Length - 1; i >= 0; i--) controlState[i] = controlMap[i].isActive();
            } catch(Exception e) {
                log_e(e);
                throw new Exception("You must first initialize the controls before accessing them, try using `setDefaultControls()`");
            }
        }
        /// <summary>
        /// checks to see if a specific control is active
        /// </summary>
        /// <param name="c">control to check</param>
        public static bool isControlActivated(controlCode c) {
            return controlState[(int)c];
        }
        
        /// <summary>
        /// represents the total time elapsed since the game was first run
        /// </summary>
        public static TimeSpan totalTimeElapsed { get { return ttime; } }
        /// <summary>
        /// gets the game object that is currently running
        /// </summary>
        public static Game1 game { get { return Program.game; } }

        /// <summary>
        /// main general logic tick
        /// </summary>
        public static void tick() {
            refreshControlState();

        }
        /// <summary>
        /// main logic tick for game, update world, physics, collisions, etc
        /// </summary>
        /// <param name="t">time elapsed since last update</param>
        public static void update(TimeSpan etime) {

        }

        /// <summary>
        /// console output -> log the message, treated as a debug message
        /// </summary>
        /// <param name="message">the string to output to the log, if it is not a string, it will be casted to one</param>
        public static void log_d(object message) {
            Console.WriteLine(message.ToString());
        }
        /// <summary>
        /// console output -> log the message, treated as error message
        /// </summary>
        /// <param name="message">message to log</param>
        public static void log_e(object message) {
            Console.WriteLine(" >>ERROR: " + message.ToString());
        }
    }

    public class gameControl {
        public gameControl() { }

        public delegate bool activatedCheck();
        public activatedCheck isActive;
        public object id;
        public object args;

        /// <summary>
        /// the default activation check for keyboard controls
        /// </summary>
        /// <param name="self">the gameControl object to apply the activation check to</param>
        public static activatedCheck ac_keyboard(gameControl self) {
            return delegate () {
                return Keyboard.GetState().IsKeyDown((Keys)self.id);
            };
        }

        /// <summary>
        /// initializes and returns a control bounded to a keyboard key
        /// </summary>
        /// <param name="k">the keyboard key that the control is bound to</param>
        public static gameControl keyboardControl(Keys k) {
            gameControl g = new gameControl();
            g.isActive = ac_keyboard(g);
            g.id = k;
            return g;
        }
    }
    public class controlBinding {
        public controlBinding() {
            active = true;
        }

        public enum triggerType {
            eachTick,
            firstTick,
        }
        public delegate void controlAction(object args);
        public gameControl control;
        public controlAction action;
        public triggerType trigger;
        public object args;
        public bool active;
        private uint ticksTriggered;

        /// <summary>
        /// checks to see if the control binding is being activated
        /// </summary>
        public void check() {
            if (!active) return;
            if (control.isActive()) {
                switch (trigger) {
                    case triggerType.firstTick:
                        if (ticksTriggered == 0)
                            action(args);
                        break;
                    case triggerType.eachTick:
                        action(args);
                        break;
                }
                ticksTriggered++;
            }
            else ticksTriggered = 0;
        }
    }
    public class controlScheme {
        public controlScheme() {
            bindings = new List<controlBinding>();
        }

        public List<controlBinding> bindings;

        /// <summary>
        /// checks to see if the user has triggered any of the controls
        /// </summary>
        public void checkUserInput() {
            foreach (controlBinding binding in bindings)
                binding.check();
        }
    }
}
