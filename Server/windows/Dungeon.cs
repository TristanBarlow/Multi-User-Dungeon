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

        private static Random rand = new Random(); 

        public void Init(int size, GameObjectList objectList)
        {
            RoomList = GenerateDungeon(size, objectList);
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

        private String HelpMessage()
        {
        return U.NewLineS("Commands are ....") +
               U.NewLineS("help - for this screen") +
               U.NewLineS("look - to look around") +
               U.NewLineS("go [north | south | east | west]  - to travel between locations") +
               U.NewLineS("graf [mesage] to add grafiti to your current room") +
               U.NewLineS("pickup [name] to pick up the item of that name in the room") +
               U.NewLineS("drop [name] to drop the item of that name into the room") +
               U.NewLineS("inventory will look at the items you have collected") +
               U.NewLineS("Press any key to continue");
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
                    returnString = HelpMessage();
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
                        String itemName = action.Remove(0, input[0].Count()+1).ToLower();
                        Item tempItem = currentRoom.GetInventory().TransfereItem(itemName);
                        if (tempItem != null)
                        {
                            player.GetInventory().AddItem(tempItem);
                            returnString += " You picked up " + itemName;
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
                        Item tempItem = player.GetInventory().TransfereItem(itemName);
                        if (tempItem != null)
                        {
                            currentRoom.GetInventory().AddItem(tempItem);
                            returnString += " You droped " + currentRoom.GetInventory().GetFirstItemFromName(itemName).itemName;
                        }
                        else
                        {
                            returnString += "Could Not Find Item to drop";
                        }
                        break;
                    }

                case "inventory":
                    returnString += U.NewLineS("Inventory:");
                    returnString += player.GetInventory().GetIventoryDescription();
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
                    returnString = U.NewLineS("\nERROR") + HelpMessage();
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

        public static void MakeBranch(Room startRoom, Room endRoom, Vector2D direction, List<Room> usedRooms, List<Room> rRooms,  List<Vector2D> positions)
        {
            Vector2D opposite = direction.GetOpposite();
            Vector2D pos = Vector2D.AddVectors(startRoom.Position, direction);
            if (!positions.Contains(pos))
            {
                if (startRoom.AddConection(direction, endRoom.RoomIndex))
                {
                    positions.Add(pos);
                    endRoom.Position = pos;
                    endRoom.AddConection(opposite, startRoom.RoomIndex);
                    usedRooms.Add(endRoom);
                    rRooms.Remove(endRoom);
                }
            }
        }

        public static List<Room> GenerateDungeon(int size, GameObjectList gol)
        {
            if (size < 4)
            {
                size = 4;
            }
            size += 1;
            List<Room> rRooms = new List<Room>();
            List<Vector2D> usedPositions = new List<Vector2D>();
            List<Room> usedRooms = new List<Room>();


            int iter = 0;
            while (rRooms.Count < size)
            {
                Room r = new Room("Room" + iter, iter);
                r.GetInventory().AddItem(gol.GetRandomItem());
                r.GetInventory().AddItem(gol.GetRandomItem());
                r.GetInventory().AddItem(gol.GetRandomWeapon());
                rRooms.Add(r);
                iter++;
            }

            Vector2D[] Dir = {NORTH, EAST, SOUTH, WEST};

            Room origin = rRooms[0];
            rRooms.Remove(origin);
            origin.Position = new Vector2D(0, 0);
            usedRooms.Add(origin);

            MakeBranch(origin, rRooms[0], NORTH, usedRooms, rRooms, usedPositions);
            MakeBranch(origin, rRooms[0], EAST, usedRooms, rRooms, usedPositions);
            MakeBranch(origin, rRooms[0], SOUTH, usedRooms, rRooms, usedPositions);
            MakeBranch(origin, rRooms[0], WEST, usedRooms, rRooms, usedPositions);

                Vector2D badVec = new Vector2D(0, 0);
            while (rRooms.Count > 1)
            {
                int roomIndex = rand.Next(usedRooms.Count);

                Vector2D dir = usedRooms[roomIndex].GetAvailableDirection();
                if (!dir.Equals(badVec) && rand.NextDouble() >= 0.4)
                {
                    MakeBranch(usedRooms[roomIndex], rRooms[0], dir, usedRooms, rRooms, usedPositions);
                }
            }


            // Console.Write(U.GenerateDungeonString(rRooms));
            return usedRooms;
        }
    }
}
