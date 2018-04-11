using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MessageTypes;
using Utilities;
using PlayerN;

namespace Dungeon
{
    public class Room
    {
        public int RoomIndex { set; get; } = -1;
        public String name = "";
        private String desc = "";
        public List<String> graffitiList;
        public Inventory inventory;
        private int north = -1;
        private int east = -1;
        private int south = -1;
        private int west = -1;
        private bool Used = false;
    

        public Room()
        {
            graffitiList = new List<string>();
        }

        public Room(String Name, int index)
        {
            name = Name;
            RoomIndex = index;
            Init();
            Item newItem = new Item("cheese", "this is a block of chese");
            inventory.AddItem(newItem);
            inventory.AddItem(newItem);
        }

        public Room(String name, String desc)
        {
            this.desc = desc;
            this.name = name;
            Init();
            Item newItem = new Item("cheese", "this is a block of chese");
            inventory.AddItem(newItem);
            inventory.AddItem(newItem);

        }

        public int[] GetExitIndexs()
        {
            int[] rInt = { north, east, south, west };
            return rInt;
        }

        public bool AddConection(int Direction, int indexOfRoom)
        {
            if (indexOfRoom >= 0)
            {
                switch (Direction)
                {
                    case 1:
                        if (north == -1) { north = indexOfRoom; Used = true; return true; }
                        break;
                    case 2:
                        if (east == -1) { east = indexOfRoom; Used = true; return true; }
                        break;
                    case -1:
                        if (south == -1) { south = indexOfRoom; Used = true; return true; }
                        break;
                    case -2:
                        if (west == -1) { west = indexOfRoom; Used = true; return true; }
                        break;

                }
            }
            return false;
        }

        private void Init()
        {
            graffitiList = new List<string>();
            inventory = new Inventory();
        }

        public void AddGraf(String graff){ graffitiList.Add(graff);}
        
        public String GetDescription()
        {
            String returnString;
            returnString = U.NewLineS(name) +
                           U.NewLineS(desc)+
                           ("Exits Are:");

            if (Used)
            {
                if (north != -1) returnString += "North ";
                if (east != -1) returnString += "East ";
                if (south != -1) returnString += "South ";
                if (west != -1) returnString += "West ";
            }
            else
            {
                returnString = "no exits oh no";
            }
            returnString += U.NewLineS(" ");

            returnString += inventory.GetIventoryDescription();

            returnString += U.NewLineS("Graffiti: ");
            if (graffitiList.Count() != 0)
            {
                foreach (String iter in graffitiList)
                {
                    returnString += iter;
                }
            }
            else
            {
                returnString += U.NewLineS("no Graffiti Be the first!!!");
            }

            return returnString;
        }

        public String GetGraff()
        {
            String returnString = "";
            if (graffitiList.Count() != 0)
            {
                foreach (String graff in graffitiList)
                {
                    returnString += U.NewLineS(graff);
                }

                return returnString;
            }
            else
            {
                return U.NewLineS("no Graffiti");
            }
        }

    }
}
