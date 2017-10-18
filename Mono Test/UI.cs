using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Mono_Test.render;

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

        public void focusAt(Vector2 position) {
            bool mfoc = false;
            for (int i = currentScreen.menuList.Count - 1; i >= 0; i--) {
                if (currentScreen.menuList[i].collision.overlapping(position, true)) {
                    currentScreen.menuFocus = i;
                    mfoc = true;
                    menuItem cmenu = currentScreen.currentMenu;
                    if(cmenu is mi_menu) {
                        bool ifoc = false;
                        for (int k = ((mi_menu)cmenu).itemList.Count - 1; i >= 0; i--) {
                            if (((mi_menu)cmenu).itemList[i].collision.overlapping(position, true)) {
                                ((mi_menu)cmenu).itemFocus = i;
                                ifoc = true;
                                break;
                            }
                        }
                        if (!ifoc) ((mi_menu)cmenu).itemFocus = -1;
                        else break;
                    }
                }
            }
            if (!mfoc) currentScreen.menuFocus = -1;
        }
        public void select() {
            if (!active) return;
            currentScreen.select();
        }

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
            menuFocus = -1;
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

        internal List<menuItem> menuList;
        internal int menuFocus;
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

        public void select() {
            if (!active) return;
            currentMenu.select();
        }

        public void addMenu(menuItem menuA) {
            menuList.Add(menuA);
        }
        public void addMenus(IEnumerable<menuItem> menus) {
            foreach (mi_menu menuI in menus)
                addMenu(menuI);
        }

        public void render(render.renderDevice rd) {
            for (int i = menuList.Count - 1; i >= 0; i--)
                menuList[i].draw(rd, position, i == menuFocus);
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
        public virtual mi_menu getMenuParent() {
            return (mi_menu)parent;
        }

        public virtual void select() {
            if (!active) return;
            global.log_d("menu item selected: " + this.name);
        }

        public virtual void draw(render.renderDevice rd, Vector2 offset, bool focus = false) {
            var bx = collision.centerAt(offset + position).getBoundingBox();
            int col = focus ? 255 : 0;
            bx.renderFill(rd, new Color(col, col, col, 50));
            bx.renderBorder(rd, Color.Black, focus ? 4 : 2);
        }
    }
    /// <summary>
    /// a menu, holds a number of menuItems and allows the user to interact with them
    /// </summary>
    public class mi_menu : menuItem {
        public mi_menu() {
            name = "ui.menuItem_defaultName";
            active = true;
            itemList = new List<menuItem>();
            itemSpacing = 50;
            autoSpacing = true;
            itemFocus = -1;
        }

        private object parent;
        public override menuItem setParent(menuItem parentA) {
            parent = parentA;
            return this;
        }
        public screen getScreenParent() {
            if (parent is screen) return (screen)parent;
            return ((mi_menu)parent).getScreenParent();
        }
        public override mi_menu getMenuParent() {
            return (parent is mi_menu) ? (mi_menu)parent : null;
        }

        internal List<menuItem> itemList;
        internal int itemFocus;
        public menuItem currentItem { get { return itemList[itemFocus]; } }
        /// <summary>
        /// how much space between menu items
        /// </summary>
        public float itemSpacing;
        /// <summary>
        /// whether or not the menu should put vertical space between menu items while drawing them
        /// </summary>
        public bool autoSpacing;

        public override void select() {
            if (!active) return;
            base.select();
            currentItem.select();
        }

        public void addItem(menuItem item) {
            itemList.Add(item.setParent(this));
        }
        public void addItems(IEnumerable<menuItem> items) {
            foreach (menuItem item in items)
                addItem(item);
        }

        public override void draw(render.renderDevice rd, Vector2 offset, bool focus = false) {
            for (int i = 0; i > itemList.Count; i++)
                itemList[i].draw(rd, offset + new Vector2(0, autoSpacing ? itemSpacing : 0));
        }
    }
    public class mi_button : menuItem {
        public mi_button(box bounds, string textA) {
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
        public override void draw(renderDevice rd, Vector2 offset, bool focus = false) {
            base.draw(rd, offset, focus);
            render.textRender rt = new textRender(text);
            if (focus) rt.filter = Color.LightGreen;
            rt.centerAt(this.position + offset);
            rd.addObject(rt);
        }
    }
}
