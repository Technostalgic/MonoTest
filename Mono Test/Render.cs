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
        public renderDevice() { }

        public SpriteBatch spriteBatch;

        public void render(Viewport v) {

        }
    }

    public class renderObject {
        public renderObject() { }

        public void draw(SpriteBatch sb) {
        }
    }

    public static class rendering {
        public static Texture2D defaultTexture;
        public static Texture2D getDefaultTexture() {
            if (defaultTexture == null) {
                defaultTexture = new Texture2D(null, 1, 1);
                defaultTexture.SetData<byte>(new byte[] { 255 });
            }
            return defaultTexture;
        }
    }
}
