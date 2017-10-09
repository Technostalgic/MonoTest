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
        void centerAt(Vector2 position);
        Vector2 getCenter();
        box getBoundingBox();

        /// <summary>
        /// whether or not a point is overlapping the collider
        /// </summary>
        /// <param name="point">the point to test</param>
        bool overlapping(Vector2 point, bool inclusive);
        /// <summary>
        /// whether or not another collider is overlapping this one
        /// </summary>
        /// <param name="c">the collider to test</param>
        bool overlapping(collider c);
        /// <summary>
        /// an intersection between this and another collider point that should lie on the perimeter
        /// of one of the two colliders
        /// </summary>
        /// <param name="c">the collider to test intersection with</param>
        /// <param name="favor">if algorithm should use this collider's perimeter or the other's when 
        /// pushing the intersect point outside of the collider so it lies on the perimeter</param>
        Vector2? intersectionPoint(collider c, bool favor = true);
        /// <summary>
        /// returns the bounding box of an intersection between two colliders
        /// </summary>
        /// <param name="c">the collisder to check intersection with</param>
        box? intersectionBounds(collider c);
        /// <summary>
        /// returns the intersection point between this collider and a ray
        /// </summary>
        /// <param name="r">the ray to test intersection with</param>
        Vector2? rayIntersect(ray r);
    }

    public struct box {
        public box(Vector2 positionA, Vector2 sizeA) {
            position = positionA;
            size = sizeA;
        }
        public box(Vector2 sizeA) {
            position = new Vector2();
            size = sizeA;
        }
        public box(float left, float right, float top, float bottom) {
            position = new Vector2(left, top);
            size = new Vector2(right - left, bottom - top);
        }

        private Vector2 position;
        private Vector2 size;
        
        public void centerAt(Vector2 pos) { }

        public float left { get { return position.X; } }
        public float right { get { return position.X + size.X; } }
        public float top { get { return position.Y; } }
        public float bottom { get { return position.Y + size.Y; } }
        public Vector2 topLeft { get { return position; } }
        public Vector2 topRight { get { return new Vector2(top, right); } }
        public Vector2 bottomLeft { get { return new Vector2(bottom, left); } }
        public Vector2 bottomRight { get { return new Vector2(bottom, right); } }
        public Vector2 getCenter() {
            return this.position + (this.size / 2f);
        }

        /// <summary>
        /// returns true if the given point lies inside of the box
        /// </summary>
        /// <param name="point">the point to test</param>
        /// <param name="inclusive">if on the very edges counts as being inside or not</param>
        public bool containsPoint(Vector2 point, bool inclusive = true) {
            if (inclusive)
                return (
                    point.X >= position.X &&
                    point.X <= right &&
                    point.Y >= position.Y &&
                    point.Y <= bottom );
            return (
                point.X > position.X &&
                point.X < right &&
                point.Y > position.Y &&
                point.Y < bottom);
        }
    }
    public struct ray {
        public ray(Vector2 start, Vector2 end) {
            position = start;
            angle = (end - start).direction();
            length = (end - start).Length();
        }

        private Vector2 position;
        private float angle;
        private float length;

        /// <summary>
        /// returns the endpoint of the array
        /// </summary>
        public Vector2 getEndPosition() {
            return new Vector2(
                position.X + (float)Math.Cos(angle) * length, 
                position.Y + (float)Math.Sin(angle) * length);
        }

        /// <summary>
        /// returns the intersection point between this ray and another
        /// </summary>
        /// <param name="r">the ray to test intersection with</param>
        /// <returns></returns>
        public Vector2? rayIntersect(ray r) { return null; }
        /// <summary>
        /// returns the intersection point between this ray and a box
        /// </summary>
        /// <param name="b">the box to test for intersection with</param>
        /// <returns></returns>
        public Vector2? boxIntersect(box b) { return null; }
        /// <summary>
        /// returns the intersection point between the ray and a circle
        /// </summary>
        /// <param name="origin">the center of the circle</param>
        /// <param name="radius">the radius of the circle</param>
        public Vector2? radialIntersect(Vector2 origin, float radius) { return null; }
        /// <summary>
        /// returns the intersection point between the ray and a collider
        /// </summary>
        /// <param name="c"></param>
        public Vector2? colliderIntersect(collider c) {
            return c.rayIntersect(this);
        }
    }

    public class collisionBox : collider {
        public collisionBox() { }
        public collisionBox(box b) {
            collisionTest = b;
        }

        public box collisionTest;

        public void centerAt(Vector2 position) {
            collisionTest.centerAt(position);
        }
        public Vector2 getCenter() {
            return collisionTest.getCenter();
        }
        public box getBoundingBox() {
            return collisionTest;
        }
        public bool overlapping(Vector2 point, bool inclusive = true) {
            return collisionTest.containsPoint(point, inclusive);
        }
        public bool overlapping(collider c) {
            global.log_e("CollsionBox.overlapping(collider) not implemented");
            return false;
        }
        public Vector2? intersectionPoint(collider c, bool favor) {
            global.log_e("CollsionBox.intersectionPoint(collider, bool) not implemented");
            return null;
        }
        public box? intersectionBounds(collider c) {
            global.log_e("CollsionBox.intersectionBounds(collider) not implemented");
            return null;
        }
        public Vector2? rayIntersect(ray r) {
            global.log_e("CollsionBox.rayIntersect(ray) not implemented");
            return null;
        }
    }
}
