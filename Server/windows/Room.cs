using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MessageTypes;
using Utilities;
using PlayerN;

namespace DungeonNamespace
{
    public class Room
    {
        public int RoomIndex { set; get; }
        public String name = "";
        public String desc = " A description";
        public List<String> graffitiList = new List<String>();
        public Inventory inventory = new Inventory();
        public int north = -1;
        public int east = -1;
        public int south = -1;
        public int west = -1;
        private bool Used = false;
    

        public Room()
        {
            graffitiList = new List<string>();
        }

        public Room(String Name, int index)
        {
            name = Name;
            RoomIndex = index;
            Item newItem = new Cheese();
            inventory.AddItem(newItem);
            inventory.AddItem(newItem);
        }

        public Room(String name, String desc)
        {
            this.desc = desc;
            this.name = name;
            Item newItem = new Cheese();
			RoomIndex = -1;
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

        public void AddGraf(String graff){ graffitiList.Add(graff);}
        
        public String GetDescription()
        {
            String returnString;
            returnString = U.NewLineS(name) +
                           U.NewLineS(desc)+
                           ("Exits Are:");

            if (north != -1) returnString += "North ";
            if (east != -1) returnString += "East ";
            if (south != -1) returnString += "South ";
            if (west != -1) returnString += "West ";

            returnString += U.NewLineS(" ");

            returnString += inventory.GetIventoryDescription();

            returnString += U.NewLineS(" ");

            returnString += ("Graffiti: ");
            if (graffitiList.Count() != 0)
            {
                foreach (String iter in graffitiList)
                {
                    returnString += U.NewLineS(iter);
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
