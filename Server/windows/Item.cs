using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Utilities;

namespace DungeonNamespace
{

    public class Inventory
    {
        private List<Item> itemList;

        public Inventory()
        {
            itemList = new List<Item>();
        }

        public void AddItem(Item nItem){ if (nItem != null) { itemList.Add(nItem); } }

        public void RemoveItem(String itemName)
        {
            try
            {
                RemoveItem(GetFirstItemFromName(itemName));
            }
            catch { Console.Write("not in itemList"); }
        }

        public void RemoveItem(Item nItem)
        {
            try
            { 
                itemList.Remove(nItem);
            }
            catch { Console.Write("not in itemList"); }
        }

        public Item GetFirstItemFromName(String itemName)
        {
            foreach (Item iter in itemList)
            {
                if (iter.itemName == itemName)
                {
                    return iter;
                }
                else return null;
            }
            return null;
        }

        public Item TransfereItem(String itemName)
        {
            Item tempItem = GetFirstItemFromName(itemName);
            if (tempItem != null)
            {
                RemoveItem(itemName);
            }
            return tempItem;
        }
        public String GetIventoryDescription()
        {
            String returnString = "";
            returnString += U.NewLineS("//");

            if (itemList.Count() > 0)
            { 
            
                foreach (Item iter in itemList)
                {
                    returnString += U.NewLineS(iter.Inspect() + "  ");
                }

            }
            else
            {
                returnString += U.NewLineS("there are no items here");
            }
            return returnString;
   
        }
        public List<Item> GetItemList() { if (itemList.Count() > 0) return itemList; else return null;}
    }

    public abstract class Item
    {
        public String itemName = "item";

        public static int ID = 0;

        private String description = "Its an Item, not particularly useful";

        public void  Init(String name, String ndescription, int id)
        {
            itemName = name;
            description = ndescription;
            ID = id;
        }
        public Item()
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

    public abstract class Weapon : Item
    {
        public static int wID = 1;

        public int damage = 1;

        public Weapon(String name, String description, int Damage)
        {
            Init(name, description, wID);
            damage = Damage;
        }
    }

    public class Cheese : Item
    {
        public int mID = 2;
        public Cheese()
        {
            Init("Cheese", "This is a block of cheese", 2);
        }
    }
    public class Rock : Item
    {
        public int mID = 3;
        public Rock()
        {
            Init("Rock", "This is a Rock", 2);
        }
    }
}
