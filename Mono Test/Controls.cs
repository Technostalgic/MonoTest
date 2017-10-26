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
    /// holds information for the input side of a control binding
    /// </summary>
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
                return Input.isKeyPressed((Keys)self.id);
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
    /// <summary>
    /// holds information for an action and a user input method binded together
    /// </summary>
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
        public controlBinding setTriggerInterval(uint ticks) {
            trigger = triggerType.interval;
            triggerArgs = ticks;
            return this;
        }
    }
    /// <summary>
    /// holds information about control bindings and checks for input from 
    /// the user for each control binding
    /// </summary>
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
                new controlBinding(gameControl.keyboardControl(Keys.Down), ca_LogTrigger, controlBinding.triggerType.eachTick),
                new controlBinding(gameControl.keyboardControl(Keys.Up), ca_LogTrigger, controlBinding.triggerType.eachTick),
                new controlBinding(gameControl.keyboardControl(Keys.Right), ca_LogTrigger, controlBinding.triggerType.eachTick),
                new controlBinding(gameControl.keyboardControl(Keys.Left), ca_LogTrigger, controlBinding.triggerType.eachTick),
                new controlBinding(gameControl.keyboardControl(Keys.Z), ca_LogTrigger, controlBinding.triggerType.firstTick),
                new controlBinding(gameControl.keyboardControl(Keys.X), ca_LogTrigger, controlBinding.triggerType.eachTick),
                new controlBinding(gameControl.keyboardControl(Keys.C), ca_LogTrigger, controlBinding.triggerType.eachTick),
                new controlBinding(gameControl.keyboardControl(Keys.Space), ca_LogTrigger, controlBinding.triggerType.firstTick)
            };

            return r;
        }
        public static void ca_LogTrigger(object args, controlBinding self) {
            global.log_d("control triggered: " + self.control.id.ToString() + "; Timestamp: " + global.totalTimeElapsed.TotalSeconds.ToString());
        }
    }
}
