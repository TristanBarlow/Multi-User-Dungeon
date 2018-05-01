using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Utilities;

namespace DungeonNamespace
{



    public class Inventory
    {
        private List<Item> itemList;

        private String inventoryID = " ";

        private static int MaxInventory = 70;

        public Inventory()
        {
            itemList = new List<Item>();
        }

        public void AddItem(Item nItem)
        {
            if (nItem != null && itemList.Count <= MaxInventory)
            {
                itemList.Add(nItem);
            }
        }

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
                if (iter.itemName.ToLower() == itemName)
                {
                    return iter;
                }
                
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
            returnString += U.NewLineS("");

            if (itemList.Count() > 0)
            { 
            
                foreach (Item iter in itemList)
                {
                    returnString += (iter.Inspect() + "  ");
                    returnString += U.NewLineS("");
                }

            }
            else
            {
                returnString += ("there are no items here");
            }
            return returnString;
   
        }

        public String GetInventoryID()
        {
            inventoryID = "&";
            foreach (Item i in itemList)
            {
                inventoryID += i.ID +"&";
            }
            return inventoryID;
        }

        public void MakeInventory(String idString, GameObjectList gol)
        {
            String[]ids = idString.Split('&');
            foreach (String id in ids)
            {
                Item i = null;
                i = gol.GetItem(id);
                if (i == null) i = gol.GetWeapon(id);
                if (i != null) itemList.Add(i);
            }
        }

        public List<Item> GetItemList() { if (itemList.Count() > 0) return itemList; else return null;}
    }

    public class Item
    {
        public String itemName = "item";

        public String ID = "i0";

        private String description = "Its an Item, not particularly useful";
        public Item(String name, String descr, String id)
        {
            itemName = name;
            ID = id;
            description = descr;
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

        public Weapon(String name, String description, int Damage, String id):base(name,description, id)
        {
            damage = Damage;
        }
    }
}
