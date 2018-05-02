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
    public class Room
    {
        public int RoomIndex { set; get; }
        public String name = "";
        public String desc = " A description";
        public List<String> graffitiList = new List<String>();
        private Inventory inventory = new Inventory();
        private List<Player> players = new List<Player>();
        public int north = -1;
        public int east = -1;
        public int south = -1;
        public int west = -1;
        private Random r = new Random();
        public bool HasMoves = true;

        private bool HasChanged = false;


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

        public Inventory GetInventory()
        {
            HasChanged = true;
            return inventory;
        }

        public void Init()
        {
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

        public void AddPlayer(Player p)
        {
            players.Add(p);
            UpdatePlayers();
        }

        private void SendBufferToPlayers(MemoryStream ms)
        {
            foreach (Player p in players)
            {
                try
                {
                    p.socket.Send(ms.GetBuffer());
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine("Error sending chat message to player: " + p.PlayerName + ex);
                }

            }
        }

        public void UpdatePlayers()
        {
            DungeonResponse update = new DungeonResponse();
            update.response = GetDescription();
            MemoryStream outStream = update.WriteData();
            SendBufferToPlayers(outStream);
           
        }

        public void RemovePlayer(Player p)
        {
            if (players.Contains(p))
            {
                players.Remove(p);
                UpdatePlayers();
            }
        }
        
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

            returnString += U.NewLineS("");

            returnString += U.NewLineS(inventory.GetIventoryDescription());

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

            returnString += U.NewLineS("");

            if (players.Count > 0)
            {
                returnString += U.NewLineS("Players in Room: ");
                foreach (Player p in players)
                {
                    returnString += p.PlayerName + "  ";
                }
            }

            return returnString;
        }

        public void PlayerSpoke(Player player, String msg)
        {
            UpdateChat said = new UpdateChat();
            said.message = player.PlayerName + " said : " + msg;
            MemoryStream outStream = said.WriteData();
            SendBufferToPlayers(outStream);
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

        public Room Copy()
        {
            Room r = new Room(name, RoomIndex);
            r.name = name;
            r.desc = desc;
            r.north = north;
            r.east = east;
            r.south = south;
            r.west = west;
            r.inventory = inventory;
            return r;
        }

        public bool GetHasChanged()
        {
            if (HasChanged)
            {
                HasChanged = false;
                return true;
            }
            else { return false; }
        }

    }
}
