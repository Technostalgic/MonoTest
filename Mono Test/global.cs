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
    /// Used for global access to data and methods
    /// </summary>
    public static class global {
        public static render.renderDevice gameRenderer;
        public static render.renderDevice menuRenderer;
        public static Random random;
        public static Color bgColor;

        private static Texture2D _cursorTexture;
        /// <summary>
        /// draws the cursor sprite at the cursor's screen position
        /// </summary>
        /// <param name="inMenu">whether it should use the menuRenderer or not</param>
        public static void drawCursor(bool inMenu = true) {
            render.renderDevice rd = inMenu ? menuRenderer : gameRenderer;
            MouseState mouse = Mouse.GetState();
            render.textureRender cursor = new render.textureRender(
                global._cursorTexture,
                new Vector2(mouse.X, mouse.Y));
            rd.addObject(cursor);
            int[] a = new int[32];
            bool[] test = new bool[4];

            //log_d(cursor.position);
        }

        public static render.bitmapFont defaultFont;

        public static void initialize() {
            bgColor = Color.Gainsboro;
            random = new Random();
            loadContent();
            gameRenderer = new render.renderDevice();
            menuRenderer = new render.renderDevice();
            Input.init();
            setDefaultControls();
            debugLogInit();
        }
        public static void loadContent() {
            loadFont();
            _cursorTexture = game.Content.Load<Texture2D>("Cursor");
        }
        private static void loadFont() {
            defaultFont = render.bitmapFont.font_loadDefaultFont();
        }

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

        public static ui.userInterface usi;
        /// <summary>
        /// loads and generates the user interface data for the game
        /// </summary>
        public static void loadUI() {
            usi = new ui.userInterface();

            ui.screen scr1 = new ui.screen();
            ui.mi_button start = new ui.mi_button(new box(140, 340, 300, 340), "Start Game");
            ui.mi_button quit = new ui.mi_button(new box(440, 640, 300, 340), "Exit");
            quit.action = delegate (object args) {
                game.Exit();
                
                return null;
            };

            scr1.addMenu(start);
            scr1.addMenu(quit);
            usi.screenList.Add(scr1);
        }

        /// <summary>
        /// main general logic tick
        /// </summary>
        public static void tick() {
            Input.refresh();
            controlMap.checkUserInput();
            update();
        }
        /// <summary>
        /// main logic tick for game, update world, physics, collisions, etc
        /// </summary>
        /// <param name="t">time elapsed since last update</param>
        public static void update() {
            if (Input.isMouseMoving())
                usi.focusAt(Input.mousePos());
            if (Input.isMouseClicked())
                usi.select();
        }
        /// <summary>
        /// renders the game
        /// </summary>
        public static void draw() {
            usi.render(menuRenderer);

            //gameRenderer.addObject(new render.textureRender(defaultFont.texture));

            drawCursor();

            render.rendering.render(new render.renderDevice[] { gameRenderer, menuRenderer });
        }

        /// <summary>
        /// console output -> log the message, treated as a debug message
        /// </summary>
        /// <param name="message">the string to output to the log, if it is not a string, it will be casted to one</param>
        public static void log_d(object message) {
            if (message == null)
                Console.WriteLine(" >>NULL");
            else Console.WriteLine(message.ToString());
        }
        /// <summary>
        /// console output -> log the message, treated as error message
        /// </summary>
        /// <param name="message">message to log</param>
        public static void log_e(object message, bool breakCode = true) {
            Console.WriteLine(" >>ERROR: " + message.ToString());
            if (breakCode) throw new Exception(message.ToString());
        }
    }
    public static class addOns {
        public static float direction(this Vector2 a) {
            return (float)Math.Atan2(a.Y, a.X);
        }
        public static Vector2 rounded(this Vector2 a) {
            a.X = (int)Math.Round(a.X);
            a.Y = (int)Math.Round(a.Y);
            return a;
        }
    }
}
