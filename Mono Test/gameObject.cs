using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mono_Test {
    public interface iUpdateable {
        void update(TimeSpan dt);
    }
    public interface iDrawable {
        void draw(render.renderDevice rd);
    }

    public class gameObject {
        static uint nextUid;
        public static uint nextUID { get { return nextUid++; } }
        
        public gameObject() {
        }
        
        public bool inWorld { get { return environment != null; } }
        private gameEnvironment _environment;
        public gameEnvironment environment { get { return _environment; } }
        public readonly uint UID = nextUID;
        public Vector2 position;

        public static bool operator ==(gameObject a, gameObject b) { return a.UID == b.UID; }
        public static bool operator !=(gameObject a, gameObject b) { return !(a == b); }
        public override bool Equals(object obj) {
            if (obj == null) return false;
            if (!(obj is gameObject)) return false;
            return this == ((gameObject)obj);
        }
        public override int GetHashCode() {
            return UID.GetHashCode();
        }

        public void worldAdd(gameEnvironment environmentA) {
            if (inWorld) return;
            environment.add(this);
            _environment = environmentA;
        }
        public void worldRemove() {
            if (!inWorld) return;
            environment.remove(this);
            _environment = null;
        }
    }

    public class character : gameObject, iUpdateable, iDrawable {
        public character() {
            collision = new collisionBox(new box(new Vector2(20, 20)));
        }

        public Vector2 velocity;
        public collider collision;

        public virtual void update(TimeSpan dt) {
            this.position += this.velocity.timeFactored(dt, environment.timeScale);
            collision.centerAt(this.position);
        }
        public virtual void draw(render.renderDevice rd) {
            box b = collision.getBoundingBox();
            b.renderFill(rd, Color.LightGreen);
            b.renderBorder(rd, Color.Black);
        }
    }
}
