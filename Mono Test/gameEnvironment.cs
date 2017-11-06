using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mono_Test {
    public class gameEnvironment {
        public gameEnvironment() {
            renderer = new render.renderDevice();
            objects = new List<gameObject>();
        }

        public render.renderDevice renderer;
        public List<gameObject> objects;
        public float timeScale = 1;

        public void add(gameObject obj) {
            if (obj.inWorld) {
                global.log_e("gameObject<" + obj.ToString() + "> is already inside an environment");
                return;
            }
            this.objects.Add(obj);
            return;
        }
        public void remove(gameObject obj) {
            if (!obj.inWorld || obj.environment != this) {
                global.log_e("gameObject<" + obj.ToString() + "> is not inside this environment");
                return;
            }
            this.objects.Remove(obj);
        }

        public void update(TimeSpan dt) {
            for (int i = objects.Count - 1; i >= 0; i--)
                objects[i].update(dt);
            
        }
        public void draw() {
            for (int i = objects.Count - 1; i >= 0; i--)
                objects[i].draw(renderer);
            renderer.render();
        }

        public static gameEnvironment getDefault() {
            return new gameEnvironment();
        }
    }
}
