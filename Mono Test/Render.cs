using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace Mono_Test.render {
    public class bitmapFont {
        public bitmapFont() { }
        /// <summary>
        /// initializes a bitmap font
        /// </summary>
        /// <param name="textureA">the spritesheet with all the characters</param>
        /// <param name="characterWidth">the width, in pixels of each character</param>
        /// <param name="characterHeight">the height, in pixels, of each character</param>
        public bitmapFont(Texture2D textureA, int characterWidth, int characterHeight) {
            texture = textureA;
            charWidth = characterWidth;
            charHeight = characterHeight;
            spacing = -2;
            characterWidths = new int[78];
            for (var i = characterWidths.Length - 1; i >= 0; i--) {
                characterWidths[i] = charWidth;
            }
        }

        public Texture2D texture;
        public int charWidth;
        public int charHeight;
        public int spacing;
        public int[] characterWidths;

        public Rectangle charRect(char character) {
            Point spc = charSpritePlace(character);
            Rectangle r = new Rectangle(spc.X * charWidth, spc.Y * charHeight, charWidth, charHeight);
            if (character == ' ') r.Height = 0;
            return r;
        }
        public Vector2 measureString(string s) {
            float w = 0;
            for (int i = 0; i < s.Length; i++) {
                float xoff = (specificCharWidth(s[i]) + spacing);
                w += xoff;
            }
            return new Vector2(w, charHeight);
        }

        public int specificCharWidth(char c) {
            Point spc = charSpritePlace(c);
            global.log_d(c.ToString() + characterWidths[spc.X + (spc.Y * 26)].ToString());
            return characterWidths[spc.X + (spc.Y * 26)];
        }
        public Point charSpritePlace(char c) {
            var r = new Point();
            if ((int)c >= 33 && (int)c < 40) { // symbols
                r.X = ((int)c - 33);
            }
            else if ((int)c >= 40 && (int)c < 59) { // 0 - 9 and operators
                r.X = 15 + ((int)c - 48);
            }
            else if ((int)c >= 65 && (int)c < 91) { // A - Z
                r.X = ((int)c - 65);
                r.Y = 1;
            }
            else if ((int)c >= 97 && (int)c < 123) { // a - z
                r.X = ((int)c - 97);
                r.Y = 2;
            }
            return r;
        }

        public void drawString(SpriteBatch sb, string str, Vector2 position, Color filter, float rotation, Vector2 origin, Vector2 scale) {
            float w = 0;
            for (int i = 0; i < str.Length; i++) {
                float xoff = (specificCharWidth(str[i]) + spacing) * scale.X;
                sb.Draw(texture,
                    position + new Vector2(w, 0),
                    charRect(str[i]),
                    filter,
                    rotation,
                    origin,
                    scale,
                    SpriteEffects.None, 
                    0f);
                w += xoff;
            }
        }

        public static bitmapFont font_loadDefaultFont() {
            bitmapFont r = new bitmapFont(global.game.Content.Load<Texture2D>("font"), 14, 16);
            r.characterWidths = new int[] {
                6,14,14,14,14,14,8,8,8,14,10,8,10,6,14,12,10,10,10,10,10,10,10,10,10,8,
                10,10,10,10,10,10,10,10,6,10,10,10,10,10,11,10,10,10,10,10,10,10,14,10,10,10,
                10,10,10,10,10,8,10,10,6,10,10,6,14,10,10,10,10,10,10,10,10,10,14,10,10,10,
            };
            return r;
        }
    }

    public class renderDevice {
        public renderDevice() {
            renderQeue = new List<renderObject>();
        }

        /// <summary>
        /// the queue that holds all the objects to be rendered
        /// </summary>
        List<renderObject> renderQeue;

        /// <summary>
        /// adds a render object to the end of the renderQueue
        /// </summary>
        /// <param name="obj"></param>
        public void addObject(renderObject obj) {
            renderQeue.Add(obj);
        }
        public void addObjects(IEnumerable<renderObject> objs) {
            foreach (renderObject obj in objs) renderQeue.Add(obj);
        }

        /// <summary>
        /// renders all of the objects that the renderQueue holds
        /// </summary>
        public void render() {
            rendering.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp);
            for (var i = 0; i < renderQeue.Count; i++)
                renderQeue[i].draw(rendering.spriteBatch);
            renderQeue.Clear();

            rendering.spriteBatch.End();
        }
    }

    public class renderObject {
        public renderObject() {
            scale = Vector2.One;
            filter = Color.White;
        }
        public renderObject(Vector2 pos, Vector2 size, Color col) {
            position = pos;
            scale = size;
            filter = col;
        }

        public Vector2 position;
        public Vector2 scale;
        public Vector2 origin;
        public float rotation;
        public Color filter;

        public virtual void draw(SpriteBatch sb) {
            sb.Draw(
                rendering.getDefaultTexture(),
                position,
                null,
                filter,
                rotation,
                origin,
                scale,
                SpriteEffects.None,
                0);
        }
    }
    public class textRender : renderObject {
        public textRender() {
            scale = new Vector2(1);
            font = global.defaultFont;
        }
        public textRender(string txt) {
            scale = new Vector2(1);
            font = global.defaultFont;
            text = txt;
        }

        public bitmapFont font;
        public string text;

        public void centerAt(Vector2 position) {
            var sz = font.measureString(text);
            this.position = position - (sz / 2);
        }

        public override void draw(SpriteBatch sb) {
            font.drawString(
                sb,
                text,
                position,
                filter,
                rotation,
                origin,
                scale);
        }
    }
    public class textureRender : renderObject{
        public textureRender() {
            scale = Vector2.One;
            filter = Color.White;
        }
        public textureRender(Texture2D textureA) {
            texture = textureA;
        }
        public textureRender(Texture2D textureA, Vector2 pos) {
            texture = textureA;
            position = pos;
            scale = Vector2.One;
            filter = Color.White;
        }

        public Texture2D texture;

        public override void draw(SpriteBatch sb) {
            sb.Draw(
                texture,
                position,
                null,
                filter,
                rotation,
                origin,
                scale,
                SpriteEffects.None,
                0);
        }
    }

    public static class rendering {
        public static GraphicsDeviceManager graphicsDM;
        public static SpriteBatch spriteBatch;
        /// <summary>
        /// set the specified graphics object references, so they can be more easily globally 
        /// accessed throu render.rendering.*
        /// </summary>
        /// <param name="gdm">the GraphicsDeviceManager to refer to</param>
        /// <param name="sb">the SpriteBatch to refer to</param>
        public static void setGraphicsRefs(GraphicsDeviceManager gdm, SpriteBatch sb) {
            graphicsDM = gdm;
            spriteBatch = sb;
        }
        
        public static Texture2D defaultTexture;
        /// <summary>
        /// returns a 1x1 white square texture, if the texture has not been initialized, 
        /// it initializes the texture
        /// </summary>
        public static Texture2D getDefaultTexture() {
            if (defaultTexture == null) {
                defaultTexture = new Texture2D(graphicsDM.GraphicsDevice, 1, 1);
                defaultTexture.SetData<Color>(new Color[] { new Color(255, 255, 255) });
            }
            return defaultTexture;
        }

        /// <summary>
        /// renders with the information in the given renderDevice
        /// </summary>
        /// <param name="device">the renderDevice to render</param>
        public static void render(renderDevice device) {
            device.render();
        }
        /// <summary>
        /// renders a set of renderDevice objects
        /// </summary>
        /// <param name="renderDevices">a set of renderDevice objects to render</param>
        public static void render(IEnumerable<renderDevice> renderDevices) {
            foreach (renderDevice device in renderDevices)
                render(device);
        }
    }
}
