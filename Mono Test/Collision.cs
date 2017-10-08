using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Mono_Test {
    /// <summary>
    /// interface used for objects that are meant to test collision
    /// </summary>
    public interface collider {
        Vector2 getCenter();
        bool overlapping(Vector2 point);
        bool overlapping(collider c);
        Vector2? intersectionPoint(collider c);
        box? intersectionBounds(collider c);
        void centerAt(Vector2 pos);
    }

    public struct box {
        public box(Vector2 position, Vector2 size) {
            _position = position;
            _size = size;
        }
        public box(Vector2 size) {
            _position = new Vector2();
            _size = size;
        }
        public box(float left, float right, float top, float bottom) {
            _position = new Vector2(left, top);
            _size = new Vector2(right - left, bottom - top);
        }

        private Vector2 _position;
        private Vector2 _size;
        public Vector2 position { get { return _position; } }
        public Vector2 size { get { return _size; } }

        public void setPosition(Vector2 pos) { }
        public void centerAt(Vector2 pos) { }

        public float left { get { return _position.X; } }
        public float right { get { return _position.X + _size.X; } }
        public float top { get { return _position.Y; } }
        public float bottom { get { return _position.Y + size.Y; } }
        public Vector2 topLeft { get { return _position; } }
        public Vector2 topRight { get { return new Vector2(top, right); } }
        public Vector2 bottomLeft { get { return new Vector2(bottom, left); } }
        public Vector2 bottomRight { get { return new Vector2(bottom, right); } }
        public Vector2 getCenter() {
            return this.position + (this.size / 2);
        }
    }

    public class collisionBox : collider {
        public collisionBox() { }
        public collisionBox(box b) {
            collisionTest = b;
        }

        public box collisionTest;

        public Vector2 getCenter() {
            return collisionTest.center();
        }
    }
}
