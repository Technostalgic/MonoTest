using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mono_Test.ui {
    public delegate object menuAction(object args);

    /// <summary>
    /// contains data for user interfaces
    /// </summary>
    public class UserInterface {
        public UserInterface() {
            active = true;
            screenList = new List<screen>();
        }

        public bool active;
        public List<screen> screenList;
        private int screenFocus;

        public screen currentScreen { get { return screenList[screenFocus]; } }

        public void update() {
        }
    }
    /// <summary>
    /// contains data for a subsection of a user interface
    /// </summary>
    public class screen {
        public screen() {
            UID = getNextUID();
            active = true;
            menuList = new List<menu>();
        }

        private static uint nextUID;
        private static uint getNextUID() {
            nextUID++;
            return nextUID;
        }

        private uint UID;
        public uint uid { get { return UID; } }

        public bool active;
        public List<menu> menuList;
        private int menuFocus;
        public menu currentMenu { get { return menuList[menuFocus]; } }

        public static bool operator== (screen a, screen b) { return a.uid == b.uid; }
        public static bool operator!= (screen a, screen b) { return !(a == b); }
        public override bool Equals(object obj) {
            if (!(obj is screen)) return false;
            return this == (screen)obj;
        }
        public override int GetHashCode() {
            return base.GetHashCode();
        }

        public void addMenu(menu menuA) {
            menuList.Add((menu)menuA.setParent(this));
        }
        public void addMenus(IEnumerable<menu> menus) {
            foreach (menu menuI in menus)
                addMenu(menuI);
        }
    }
    /// <summary>
    /// a menu, holds a number of menuItems and allows the user to interact with them
    /// </summary>
    public class menu : menuItem {
        public menu() {
            active = true;
            itemList = new List<menuItem>();
        }

        private object parent;
        public override menuItem setParent(object parentA) {
            if (!((parentA is menu) || (parentA is screen)))
                global.log_e("menu object must have a parent of either Type `menu` or `screen`");
            parent = parentA;
            return this;
        }
        public screen getScreenParent() {
            if (parent is screen) return (screen) parent;
            return ((menu)parent).getScreenParent();
        }
        public override menu getMenuParent() {
            return (parent is menu) ? (menu)parent : null;
        }
        private List<menuItem> itemList;

        public void addItem(menuItem item) {
            itemList.Add(item.setParent(this));
        }
        public void addItems(IEnumerable<menuItem> items) {
            foreach (menuItem item in items)
                addItem(item);
        }
    }
    /// <summary>
    /// an interactive part of a menu, i.e. a button, checkbox, slider, etc 
    /// </summary>
    public class menuItem {
        public menuItem() {
            active = true;
        }

        public bool active;

        private object parent;
        public virtual menuItem setParent(object parentA) {
            if (!(parent is menu))
                global.log_e("menuItem parent must be of type `menu` if being used as a menuItem");
            parent = parentA;
            return this;
        }
        public virtual menu getMenuParent() {
            return (menu)parent;
        }
    }
}
