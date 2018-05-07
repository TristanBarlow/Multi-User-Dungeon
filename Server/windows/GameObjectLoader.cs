using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DungeonNamespace
{
    /**
     *This class is responsible for handling the parsing of the text files that are used for the rooms, items and weapons 
     * In addition it stores an instance of each class which is used to look up the item/weapons attributes
     */
    public class GameObjectList
    {
        //item list contains weapons aswell String is the items ID
        private Dictionary<String, Item> AllAvailableItems = new Dictionary<String, Item>();

        //only contains weapons
        private Dictionary<String, Weapon> AllAvailableWeapons = new Dictionary<String, Weapon>();

        //Room names and descriptions
        private Dictionary<String, String> RoomsAndDescriptions = new Dictionary<String, String>();

        private Random rand = new Random();

        public GameObjectList()
        {
            //get and parse itemlist if it exsists
            String itemPath = "ItemList.txt";

            if (!File.Exists(itemPath))
            {
                Console.WriteLine("Could not find items list");
            }
            else
            {
                ParseItemFile(itemPath);
            }

            //get and parse weaponlist if it exsists
            String weaponPath = "WeaponList.txt";
            if (!File.Exists(weaponPath))
            {
                Console.WriteLine("Could not find weapon list");
            }
            else
            {
                ParseWeaponFile(weaponPath);
            }

            //get and parse roomlist if it exsists
            String roomPath = "RoomList.txt";
            if (!File.Exists(roomPath))
            {
                Console.WriteLine("Could not find room list");
            }
            else
            {
                ParseRoomFile(roomPath);
            }


        }

        //File Parsing basic stuff
        private void ParseWeaponFile(String path)
        {
            // Open the file to read from.
            string readText = File.ReadAllText(path);
            readText = readText.Replace("\r\n", "");

            String[] items = readText.Split('&');
            {
                for (int i = 1; i < items.Length; i++)
                {
                    String[] s = items[i].Split(',');
                    if (s.Length > 3)
                    {
                        AllAvailableWeapons.Add(s[3], new Weapon(s[0], s[1], Int32.Parse(s[2]), s[3]));
                        AllAvailableItems.Add(s[3], new Weapon(s[0], s[1], Int32.Parse(s[2]), s[3]));
                    }
                }
            }
        }
        private void ParseItemFile(String path)
        {
            // Open the file to read from.
            string readText = File.ReadAllText(path);
            readText = readText.Replace("\r\n", "");

            String[] items = readText.Split('&');
            {
                for (int i = 1; i < items.Length; i++)
                {
                    String[] w = items[i].Split(',');
                    if (w.Length > 2)
                    {
                        AllAvailableItems.Add(w[2], new Item(w[0], w[1], w[2]));
                    }
                }
            }
        }
        private void ParseRoomFile(String path)
        {
            // Open the file to read from.
            string readText = File.ReadAllText(path);
            readText = readText.Replace("\r\n", "");

            String[] items = readText.Split('&');
            {
                for (int i = 1; i < items.Length; i++)
                {
                    String[] w = items[i].Split(',');
                    if (w.Length > 1)
                    {
                        RoomsAndDescriptions.Add(w[0],w[1]);
                    }
                }
            }
        }

        public KeyValuePair<String, String> GetRandomRoom()
        {
            int index = rand.Next(0, RoomsAndDescriptions.Count);
            return RoomsAndDescriptions.ElementAt(index);
        }

        public Weapon GetWeapon(String wID)
        {
            Weapon w = null;
            if (AllAvailableWeapons.ContainsKey(wID))
            {
                return AllAvailableWeapons[wID];
            }
            return w;
        }
        public Item GetItem(String iID)
        {
            Item i = null;
            if (AllAvailableItems.ContainsKey(iID))
            {
                return AllAvailableItems[iID];
            }
            return i;
        }

        public Item GetRandomItem()
        {
            if (AllAvailableItems.Count > 0)
            {
                Item i = null;
                i = AllAvailableItems.ElementAt(rand.Next(AllAvailableItems.Count)).Value;
                return i;
            }
            return null;
        }
        public Weapon GetRandomWeapon()
        {
            if (AllAvailableWeapons.Count > 0)
            {
                Weapon w = null;
                w = AllAvailableWeapons.ElementAt(rand.Next(AllAvailableWeapons.Count)).Value;
                return w;
            }
            return null;
        }
    }
}
