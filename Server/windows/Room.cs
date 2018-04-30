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
        private Random r = new Random();
        public bool HasMoves = true;

        public List<Vector2D> availableDirections = new List<Vector2D>();

        public Vector2D Position { set; get; }

        public Vector2D GetAvailableDirection()
        {
            if (availableDirections.Count < 1)
            {
                HasMoves = false;
                return new Vector2D(0, 0);
            }
            else
            {
                Vector2D v = availableDirections[(r.Next(availableDirections.Count))];
                availableDirections.Remove(v);
                return v;
            }

        }

        public Room()
        {
            graffitiList = new List<string>();
        }

        public Room(String Name, int index)
        {
            name = Name;
            RoomIndex = index;
            Init();
        }

        public Room(String name, String desc)
        {
            this.desc = desc;
            this.name = name;
            Init();
        }

        public void Init()
        {
            Item newItem = new Cheese();
            inventory.AddItem(newItem);
            inventory.AddItem(newItem);
            Position = new Vector2D();
            availableDirections.Add(Dungeon.NORTH);
            availableDirections.Add(Dungeon.EAST);
            availableDirections.Add(Dungeon.SOUTH);
            availableDirections.Add(Dungeon.WEST);
        }

        public int[] GetExitIndexs()
        {
            int[] rInt = { north, east, south, west };
            return rInt;
        }

        public bool AddConection(Vector2D Direction, int indexOfRoom)
        {
            if (indexOfRoom >= 0)
            {
                if (Direction.Equals(Dungeon.NORTH) && north == -1) { north = indexOfRoom; return true; }
                else if (Direction.Equals(Dungeon.EAST) && east == -1) { east = indexOfRoom; return true; }
                else if (Direction.Equals(Dungeon.SOUTH) && south == -1) { south = indexOfRoom; return true; }
                else if (Direction.Equals(Dungeon.WEST) && west == -1) { west = indexOfRoom; return true; }
            }
            return false;
        }

        public void AddGraf(String graff){ graffitiList.Add(graff);}
        
        public String GetDescription()
        {
            String returnString = U.NewLineS(" ");
            returnString =  U.NewLineS(name) +
                            U.NewLineS(" ")+
                            U.NewLineS(desc)+
                            U.NewLineS(" ")+ 
                            ("Exits Are:");

            if (north != -1) returnString += "North ";
            if (east != -1) returnString += "East ";
            if (south != -1) returnString += "South ";
            if (west != -1) returnString += "West ";

            returnString += U.NewLineS(" ");

            returnString += U.NewLineS(inventory.GetIventoryDescription());

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
