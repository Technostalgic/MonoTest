using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Mono_Test.ui {
    public delegate object menuAction(object args);

    /// <summary>
    /// contains data for user interfaces
    /// </summary>
    public class userInterface {
        public userInterface() {
            active = true;
            screenList = new List<screen>();
        }

        public bool active;
        public List<screen> screenList;
        private int screenFocus;

        public screen currentScreen { get { return screenList[screenFocus]; } }

        public void update() {
        }
        public void render(render.renderDevice rd) {
            currentScreen.render(rd);
        }
    }
    /// <summary>
    /// contains data for a subsection of a user interface
    /// </summary>
    public class screen {
        public screen() {
            UID = getNextUID();
            name = "ui.screen_defaultName";
            active = true;
            menuList = new List<menuItem>();
        }

        private static uint nextUID;
        private static uint getNextUID() {
            nextUID++;
            return nextUID;
        }

        private uint UID;
        public uint uid { get { return UID; } }

        public string name;
        public Vector2 position;
        public bool active;

        private List<menuItem> menuList;
        private int menuFocus;
        public menuItem currentMenu { get { return menuList[menuFocus]; } }

        public static bool operator== (screen a, screen b) { return a.uid == b.uid; }
        public static bool operator!= (screen a, screen b) { return !(a == b); }
        public override bool Equals(object obj) {
            if (!(obj is screen)) return false;
            return this == (screen)obj;
        }
        public override int GetHashCode() {
            return base.GetHashCode();
        }

        public void addMenu(menuItem menuA) {
            menuList.Add(menuA);
        }
        public void addMenus(IEnumerable<menuItem> menus) {
            foreach (menu menuI in menus)
                addMenu(menuI);
        }

        public void render(render.renderDevice rd) {
            foreach (menuItem m in menuList) m.draw(rd, position);
        }
    }
    /// <summary>
    /// an interactive part of a menu, i.e. a button, checkbox, slider, etc 
    /// </summary>
    public class menuItem {
        public menuItem() {
            name = "ui.menuItem_defaultName";
            active = true;
        }

        public string name;
        public bool active;
        public Vector2 position;
        public collider collision;

        private object parent;
        public virtual menuItem setParent(menuItem parentA) {
            parent = parentA;
            return this;
        }
        public virtual menu getMenuParent() {
            return (menu)parent;
        }

        public virtual void select() {
            global.log_d("menu item selected: " + this.name);
        }

        public virtual void draw(render.renderDevice rd, Vector2 offset) {
            var bx = collision.centerAt(offset + position).getBoundingBox();
            bx.renderFill(rd, new Color(0, 0, 0, 50));
            bx.renderBorder(rd, Color.Black, 2);
        }
    }
    /// <summary>
    /// a menu, holds a number of menuItems and allows the user to interact with them
    /// </summary>
    public class menu : menuItem {
        public menu() {
            name = "ui.menu_defaultName";
            active = true;
            itemList = new List<menuItem>();
            itemSpacing = 50;
            autoSpacing = true;
        }

        private object parent;
        public override menuItem setParent(menuItem parentA) {
            parent = parentA;
            return this;
        }
        public screen getScreenParent() {
            if (parent is screen) return (screen)parent;
            return ((menu)parent).getScreenParent();
        }
        public override menu getMenuParent() {
            return (parent is menu) ? (menu)parent : null;
        }

        private List<menuItem> itemList;
        /// <summary>
        /// how much space between menu items
        /// </summary>
        public float itemSpacing;
        /// <summary>
        /// whether or not the menu should put vertical space between menu items while drawing them
        /// </summary>
        public bool autoSpacing;

        public void addItem(menuItem item) {
            itemList.Add(item.setParent(this));
        }
        public void addItems(IEnumerable<menuItem> items) {
            foreach (menuItem item in items)
                addItem(item);
        }

        public override void draw(render.renderDevice rd, Vector2 offset) {
            for (int i = 0; i > itemList.Count; i++)
                itemList[i].draw(rd, offset + new Vector2(0, autoSpacing ? itemSpacing : 0));
        }
    }
    public class menu_button : menuItem {
        public menu_button(box bounds, string textA) {
            position = bounds.getCenter();
            collision = new collisionBox(bounds);
            text = textA;
        }

        public string text;
        public menuAction action;
        public object args;

        public override void select() {
            action(args);
            base.select();
        }
    }
}
