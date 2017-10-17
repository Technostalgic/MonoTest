using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Mono_Test.render {
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

        public SpriteFont font;
        public string text;

        public void centerAt(Vector2 position) {
            var sz = font.MeasureString(text);
            this.position = position - (sz / 2);
        }

        public override void draw(SpriteBatch sb) {
            sb.DrawString(
                font,
                text,
                position,
                filter,
                rotation,
                origin,
                scale,
                SpriteEffects.None,
                0f);
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
