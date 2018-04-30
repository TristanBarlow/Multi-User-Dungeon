using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Utilities;
using PlayerN;

namespace DungeonNamespace
{
    public struct Vector2D
    {
        public int X { set; get; }
        public int Y { set; get; }
		public Vector2D(int a, int b):this()
        {
            X = a;
            Y = b;

        }
        public Vector2D(Vector2D v):this()
        {
            X = v.X;
            Y = v.Y;
        }

        public static Vector2D AddVectors(Vector2D a, Vector2D b)
        {
            return new Vector2D(a.X + b.X, a.Y + b.Y);
        }

        public Vector2D GetOpposite()
        {
            return new Vector2D((X * -1), (Y * -1));
        }
    }

    public class Dungeon
    {        
        //private Dictionary<String, Room> roomMap;
        private List<Room> RoomList= new List<Room>();

        private String DungeonStr = " ";

        public static Vector2D NORTH = new Vector2D(0, 1);
        public static Vector2D EAST = new Vector2D(1, 0);
        public static Vector2D SOUTH = new Vector2D(0, -1);
        public static Vector2D WEST = new Vector2D(-1, 0);

        public void Init(int size, int spread)
        {
           // roomMap = new Dictionary<string, Room>();
            RoomList = GenerateDungeon(size, spread);
            foreach (Room r in RoomList)
            {
               // roomMap.Add(r.name, r);
            }
            DungeonStr = GenerateDungeonString(RoomList);
        }

        public String GetDungeonString()
        {
            if (DungeonStr == " ")
            {
                return GenerateDungeonString(RoomList);
            }
            else { return DungeonStr; }
        }

        public List<Room> GetRoomList()
        {
            return RoomList;
        }

        public Room GetRandomRoom()
        {
            Random rnd = new Random();
            int num = rnd.Next(0, RoomList.Count);
            Room randomRoom = RoomList[num];
            return randomRoom;
        }

        private void MovePlayer(Player p, Room newRoom)
        {
            p.SetRoom(newRoom);
        }

        public String PlayerAction(String action,Player player)

        {
            String returnString = "";

            var input = action.Split(' ');

            if (player == null)
            {
                returnString = U.NewLineS("Not in list you do not exsist");
                return returnString;
            }

            Room currentRoom = RoomList[player.roomIndex];

            switch (input[0].ToLower())
            {
                case "help":
                    Console.Clear();
                    returnString = U.NewLineS("Commands are ....") +
                                   U.NewLineS("help - for this screen") +
                                   U.NewLineS("look - to look around") +
                                   U.NewLineS("go [north | south | east | west]  - to travel between locations") +
                                   U.NewLineS("graf [mesage] to add grafiti to your current room") +
                                   U.NewLineS("pickup [name] to pick up the item of that name in the room")+
                                   U.NewLineS("drop [name] to drop the item of that name into the room")+
                                   U.NewLineS("inventory will look at the items you have collected")+
                                   U.NewLineS("Press any key to continue");
                    break;

                case "look":
                    returnString = U.NewLineS("you look around") +
                                   U.NewLineS(currentRoom.GetDescription());
                    break;

                case "graf":
                    int index = action.IndexOf(' ');
                    String second = action.Substring(index + 1);
                    currentRoom.AddGraf(second);
                    returnString = U.NewLineS("You added a graffiti");
                    break;

                case "pickup":
                    {
                        String itemName = input[1];
                        Item tempItem = currentRoom.inventory.TransfereItem(itemName);
                        if (tempItem != null)
                        {
                            player.inventory.AddItem(tempItem);
                            returnString += " You picked up " + player.inventory.GetFirstItemFromName(itemName).itemName;
                        }
                        else
                        {
                            returnString += "Could Not Find Item";
                        }
                        break;
                    }

                case "drop":
                    {
                        String itemName = input[1];
                        Item tempItem = player.inventory.TransfereItem(itemName);
                        if (tempItem != null)
                        {
                            currentRoom.inventory.AddItem(tempItem);
                            returnString += " You droped " + currentRoom.inventory.GetFirstItemFromName(itemName).itemName;
                        }
                        else
                        {
                            returnString += "Could Not Find Item to drop";
                        }
                        break;
                    }

                case "inventory":
                    returnString += U.NewLineS("Inventory:");
                    returnString += player.inventory.GetIventoryDescription();
                    break;

                case "go":
                    // is arg[1] sensible?
                    int[] indexs = currentRoom.GetExitIndexs();
                    if ((input[1].ToLower() == "north") && (currentRoom.north != -1))
                    {
                        MovePlayer(player, RoomList[currentRoom.north]);
                    }
                     else if ((input[1].ToLower() == "east") && (currentRoom.east != -1))
                    {
                        MovePlayer(player, RoomList[currentRoom.east]);
                    }
                    else if ((input[1].ToLower() == "south") && (currentRoom.south != -1))
                    {
                        MovePlayer(player, RoomList[currentRoom.south]);
                    }
                    else if ((input[1].ToLower() == "west") && (currentRoom.west != -1))
                    {       
                        MovePlayer(player, RoomList[currentRoom.west]);           
                    }
                    else
                    {
                        //handle error
                        returnString = U.NewLineS("\nERROR")+
                                        U.NewLineS("\nCan not go " + input[1] + " from here")+
                                        U.NewLineS("\nPress any key to continue");
                    }
                    returnString = U.NewLineS(player.GetRoom().GetDescription());
                    break;

                default:
                    //handle error
                    returnString = U.NewLineS("\nERROR")+
                                   U.NewLineS("\nCan not " + input)+
                                   U.NewLineS("\nPress any key to continue");
                    break;
            }
            if (returnString != "")
            {
                return returnString;
            }
            else
            {
                returnString = U.NewLineS("welp");
                return returnString;
            }

        }

        public static String GenerateDungeonString(List<Room> RoomList)
        {
            String rStr = "&";
            foreach (Room r in RoomList)
            {
                bool GoodRoom = false;
                int[] temp = r.GetExitIndexs();
                if (temp[0] != -1)
                {
                    rStr += "n" + temp[0] + " ";
                    GoodRoom = true;
                }
                if (temp[1] != -1)
                {
                    rStr += "e" + temp[1] + " ";
                    GoodRoom = true;
                }
                if (temp[2] != -1)
                {
                    rStr += "s" + temp[2] + " ";
                    GoodRoom = true;
                }
                if (temp[3] != -1)
                {
                    rStr += "w" + temp[3] + " ";
                    GoodRoom = true;
                }
                if (GoodRoom) rStr += "&";
            }

            return rStr;
        }

        public static void MakeConnections(ref int i, int RoomLength, Vector2D direction, ref List<Room> rRooms, ref List<Vector2D> positions)
        {
            Vector2D opposite = direction.GetOpposite();
            rRooms[0].AddConection(direction, i);
            rRooms[i].AddConection(opposite, 0);

            Vector2D pos = Vector2D.AddVectors(rRooms[0].Position, direction);
            positions.Add(pos);
            rRooms[i].Position = pos;
            

            int max = i + RoomLength;
            while (i < max)
            {
                int ti = i - 1;
                rRooms[i].AddConection(opposite, ti);
                pos = Vector2D.AddVectors(rRooms[i].Position, direction);
                positions.Add(pos);
                rRooms[i+1].Position = pos;
                rRooms[i].AddConection(direction, i + 1);
                i++;
            }

            rRooms[i].AddConection(opposite, i - 1);
            i++;
        }

        public static void MakeBranch(ref int i, int BranchStart, int NumberOfRooms, Vector2D direction, ref List<Room> rRooms, ref List<Vector2D> positions)
        {
            Vector2D opposite = direction.GetOpposite();
            Vector2D pos = Vector2D.AddVectors(rRooms[BranchStart].Position, direction);
            if (!positions.Contains(pos))
            {
                if (rRooms[BranchStart].AddConection(direction, i))
                {
                    positions.Add(pos);
                    rRooms[i].AddConection(opposite, BranchStart);
                    i++;
                }
            }
        }

        public static List<Room> GenerateDungeon(int size, int spread)
        {
            if (size < 4)
            {
                size = 4;
            }
            size += 1;
            List<Room> rRooms = new List<Room>();
            Random rand = new Random();
            List<Vector2D> usedPositions = new List<Vector2D>();


            int iter = 0;
            while (rRooms.Count < size)
            {
                rRooms.Add(new Room("Room" + iter, iter));
                iter++;
            }
            int dungeonSize = (int)(size / spread) - 1;

            int i = 1;
            rRooms[0].Position = new Vector2D(0, 0);

            MakeConnections(ref i, dungeonSize, NORTH, ref rRooms,ref usedPositions);
            MakeConnections(ref i, dungeonSize, EAST, ref rRooms , ref usedPositions);
            MakeConnections(ref i, dungeonSize, SOUTH, ref rRooms, ref usedPositions);
            MakeConnections(ref i, dungeonSize, WEST, ref rRooms, ref usedPositions);

            int RoomsLeftOver = rRooms.Count - i;
            Vector2D[] Dir = {NORTH, EAST, SOUTH, WEST};

            while (i < rRooms.Count)
            {
                MakeBranch(ref i, rand.Next(0, i), 0, Dir[rand.Next(0, 4)], ref rRooms,ref usedPositions);
            }

            // Console.Write(U.GenerateDungeonString(rRooms));
            return rRooms;
        }
    }
}
