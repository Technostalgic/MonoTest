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
        private static controlScheme controlMap;
        /// <summary>
        /// applies the default control scheme
        /// </summary>
        public static void setDefaultControls() {
            controlMap = controlScheme.getDefaultControlScheme();
        }                                                                                                        

        /// <summary>
        /// the current control scheme
        /// </summary>
        public static controlScheme controls { get { return controlMap; } }
        
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
            controlMap.checkUserInput();
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
            trigger = triggerType.eachTick;
        }
        /// <summary>
        /// initializes a control binding with the specified data
        /// </summary>
        /// <param name="controlA">the game control to check for input</param>
        /// <param name="actionA">the action to perform when the binding is triggered</param>
        /// <param name="triggerA">how the binding will be activated</param>
        public controlBinding(gameControl controlA, controlAction actionA, triggerType triggerA = triggerType.eachTick) {
            active = true;
            control = controlA;
            action = actionA;
            trigger = triggerA;
        }

        public enum triggerType {
            eachTick,
            firstTick,
            release,
            interval
        }
        public delegate void controlAction(object args = null, controlBinding self = null);
        public gameControl control;
        public controlAction action;
        public triggerType trigger;
        public object args;
        public bool active;
        private uint ticksTriggered;
        private object triggerArgs;

        /// <summary>
        /// checks to see if the control binding is being triggered
        /// </summary>
        public void check() {
            if (!active) return;
            if (control.isActive()) {
                switch (trigger) {
                    case triggerType.firstTick:
                        if (ticksTriggered == 0)
                            action(args, this);
                        break;
                    case triggerType.eachTick:
                        action(args, this);
                        break;
                    case triggerType.interval:
                        if (ticksTriggered % (uint)triggerArgs == 0)
                            action(args, this);
                        break;
                }
                ticksTriggered++;
            }
            else {
                if (trigger == triggerType.release && ticksTriggered > 0)
                    action(args, this);
                ticksTriggered = 0;
            }
        }

        /// <summary>
        /// sets the trigger to `interval` and sets the trigger args to the specified value
        /// </summary>
        /// <param name="ticks">how many ticks the delay is between each time the control action is performed</param>
        public void setTriggerInterval(uint ticks) {
            trigger = triggerType.interval;
            triggerArgs = ticks;
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

        public static controlScheme getDefaultControlScheme() {
            controlScheme r = new controlScheme();

            r.bindings = new List<controlBinding>() {
                new controlBinding(gameControl.keyboardControl(Keys.Enter), ca_LogTrigger, controlBinding.triggerType.firstTick),
                new controlBinding(gameControl.keyboardControl(Keys.Up), ca_LogTrigger, controlBinding.triggerType.firstTick),
                new controlBinding(gameControl.keyboardControl(Keys.Down), ca_LogTrigger, controlBinding.triggerType.firstTick),
                new controlBinding(gameControl.keyboardControl(Keys.Right), ca_LogTrigger, controlBinding.triggerType.firstTick),
                new controlBinding(gameControl.keyboardControl(Keys.Left), ca_LogTrigger, controlBinding.triggerType.firstTick),
                new controlBinding(gameControl.keyboardControl(Keys.Z), ca_LogTrigger, controlBinding.triggerType.firstTick),
                new controlBinding(gameControl.keyboardControl(Keys.X), ca_LogTrigger, controlBinding.triggerType.firstTick),
                new controlBinding(gameControl.keyboardControl(Keys.C), ca_LogTrigger, controlBinding.triggerType.firstTick),
                new controlBinding(gameControl.keyboardControl(Keys.Space), ca_LogTrigger, controlBinding.triggerType.firstTick)
            };

            return r;
        }
        public static void ca_LogTrigger(object args, controlBinding self) {
            global.log_d("control triggered: " + self.control.id.ToString());
        }
    }
}
