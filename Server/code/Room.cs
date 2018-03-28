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
        public String[] exits = new String[4];
        private List<Player> Players;
        public List<String> graffitiList;
        public Inventory inventory;
        public static String[] exitNames = { "NORTH", "SOUTH", "EAST", "WEST" };
       private int north = -1;
       private int east = -1;
       private int south = -1;
       private int west = -1;
       private bool Used = false;
    

        public Room()
        {
            graffitiList = new List<string>();
            Players = new List<Player>();
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
            Players = new List<Player>();
            inventory = new Inventory();
        }
        public String North
        {
            get { return exits[0]; }
            set { exits[0] = value; }
        }

        public String South
        {
            get { return exits[1]; }
            set { exits[1] = value; }
        }

        public String East
        {
            get { return exits[2]; }
            set { exits[2] = value; }
        }

        public String West
        {
            get { return exits[3]; }
            set { exits[3] = value; }
        }

        public void AddGraf(String graff){ graffitiList.Add(graff);}
                    
        public void AddPlayer(Player p) {Players.Add(p); }

        public void RemovePlayer(Player p) {Players.Remove(p);}
        
        public String GetDescription()
        {
            String returnString;
            returnString = U.NewLineS(name) +
                           U.NewLineS(desc)+
                           U.NewLineS("//")+
                           ("Exits Are:");

            if (exits.Count() != 0)
            {
                for (var i = 0; i < exits.Length; i++)
                {
                    if (exits[i] != null)
                    {
                        returnString += Room.exitNames[i] + " ";
                    }
                }
                returnString += U.NewLineS(" ")+
                                U.NewLineS("//");
                                
            }
            else
            {
                returnString = "no exits oh no";
            }

            returnString += ("Players In room : ");

            if (Players.Count() > 1)
            {
                foreach (Player iter in Players)
                {
                    returnString += iter.GetPlayerName() + "  ";
                }
                
            }
            else
            {
                returnString +=U.NewLineS("You are alone") ;
            }

            returnString += inventory.GetIventoryDescription();

            returnString += U.NewLineS("//");

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

        public List<Player> GetPlayersInRoom() { return Players; }

    }
}
