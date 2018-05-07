using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MessageTypes;
using Utilities;
using PlayerN;
using System.IO;

namespace DungeonNamespace
{
    /**
     *Room is mainly used during the dungeon creation process.
     * It is also used when navigating rather than constantly using the db
     * The room layout is loaded into ram. 
     */
    public class Room
    {

        public String name = "";
        public String desc = " A description";
        public int RoomIndex { set; get; }

        //index of the exits in the given location
        public int north = -1;
        public int east = -1;
        public int south = -1;
        public int west = -1;
        private Random r = new Random();

        //used a list rather than an array so i could remove items, and check the length.
        //Probably bad practice but made it easier and quicker
        public List<Vector2D> availableDirections = new List<Vector2D>();



        public Room()
        {
        }

        public Room(String Name, int index)
        {
            name = Name;
            RoomIndex = index;
            Init();
        }
 
        public void Init()
        {
            Position = new Vector2D();
            availableDirections.Add(Dungeon.NORTH);
            availableDirections.Add(Dungeon.EAST);
            availableDirections.Add(Dungeon.SOUTH);
            availableDirections.Add(Dungeon.WEST);
        }

        /**
         *Pretty sure this is Gareths code. Been here for ever 
         */
        public int[] GetExitIndexs()
        {
            int[] rInt = { north, east, south, west };
            return rInt;
        }

        //Creation stuffs


        /**
         *Adds a connection of the given room in the given direction
         * @param the room
         * @param the direction
         */
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

        public Vector2D Position { set; get; }

        public Vector2D GetAvailableDirection()
        {
            if (availableDirections.Count < 1)
            {
                return new Vector2D(0, 0);
            }
            else
            {
                Vector2D v = availableDirections[(r.Next(availableDirections.Count))];
                availableDirections.Remove(v);
                return v;
            }

        }
    }
}
