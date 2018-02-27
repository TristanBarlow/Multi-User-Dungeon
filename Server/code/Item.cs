using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Utilities;

namespace Dungeon
{
    public class Item
    {
        private String itemName = "item";

        private String description = "Its an Item, not particularly useful";

        public  Item(String name, String description)
        {

        }
        public virtual String Inspect()
        {
            return U.NewLineS("ItemName: " + itemName) + U.NewLineS("Description: " + description);
        }

        public virtual String UseItem()
        {
            return U.NewLineS("This item has no use");
        }

    }
    public class Weapon : Item
    {
        public int damage = 1;
        public Weapon(String name, String description, int Damage):base(name, description)
        {
            damage = Damage;
        }
    }

}
