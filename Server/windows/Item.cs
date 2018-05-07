using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Utilities;

namespace DungeonNamespace
{

    public class Item
    {
        public String itemName = "item";

        public String ID = "i0";

        public String description = "Its an Item, not particularly useful";
        public Item(String name, String descr, String id)
        {
            itemName = name;
            ID = id;
            description = descr;
        }

        public virtual String Inspect()
        {
            return U.NL("ItemName: " + itemName) + U.NL("Description: " + description);
        }

        public virtual String UseItem()
        {
            return U.NL("This item has no use");
        }


    }

    public class Weapon : Item
    {
        public int damage = 1;

        public Weapon(String name, String description, int Damage, String id):base(name,description, id)
        {
            damage = Damage;
        }
    }
}
