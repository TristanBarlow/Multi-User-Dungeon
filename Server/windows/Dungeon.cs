using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Utilities;
using PlayerN;

namespace Dungeon
{
    public class DungeonS
    {        
        //private Dictionary<String, Room> roomMap;
        private List<Room> RoomList;

		public String DungeonStr { set; get; }

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

        public Room GetRandomRoom()
        {
            Random rnd = new Random();
            int num = rnd.Next(0, RoomList.Count);
            Room randomRoom = RoomList[num];
            return randomRoom;
        }

        private void MovePlayer(Player p, Room newRoom)
        {
            p.currentRoom = newRoom;
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
                    //loop straight back
                    returnString = U.NewLineS("you look around") +
                                   U.NewLineS(player.currentRoom.GetDescription());
                    break;

                case "graf":
                    int index = action.IndexOf(' ');
                    String second = action.Substring(index + 1);
                    player.currentRoom.AddGraf(second);
                    returnString = U.NewLineS("You added a graffiti");
                    break;

                case "pickup":
                    {
                        String itemName = input[1];
                        Item tempItem = player.currentRoom.inventory.TransfereItem(itemName);
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
                            player.currentRoom.inventory.AddItem(tempItem);
                            returnString += " You droped " + player.currentRoom.inventory.GetFirstItemFromName(itemName).itemName;
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
                    int[] indexs = player.currentRoom.GetExitIndexs();
                    if ((input[1].ToLower() == "north") && (indexs[0] != -1))
                    {
                        MovePlayer(player, RoomList[indexs[0]]);
                    }
                    else
                    {
                        if ((input[1].ToLower() == "south") && (indexs[2] != -1))
                        {
                            MovePlayer(player, RoomList[indexs[2]]);
                        }
                        else
                        {
                            if ((input[1].ToLower() == "east") && (indexs[1] != -1))
                            {
                                MovePlayer(player, RoomList[indexs[1]]);
                            }
                            else
                            {
                                if ((input[1].ToLower() == "west") && (indexs[3] != -1))
                                {
                                   
                                    MovePlayer(player, RoomList[indexs[3]]);
                                    
                                }
                                else
                                {
                                    //handle error
                                   returnString = U.NewLineS("\nERROR")+
                                                  U.NewLineS("\nCan not go " + input[1] + " from here")+
                                                  U.NewLineS("\nPress any key to continue");
                                }
                            }
                        }
                    }
                    returnString = U.NewLineS(player.currentRoom.GetDescription());
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

        public static void MakeConnections(ref int i, int RoomLength, int direction, ref List<Room> rRooms)
        {
            int opposite = -1 * direction;
            rRooms[0].AddConection(direction, i);
            rRooms[i].AddConection(opposite, 0);
            int max = i + RoomLength;
            while (i < max)
            {
                int ti = i - 1;
                rRooms[i].AddConection(opposite, ti);
                rRooms[i].AddConection(direction, i + 1);
                i++;
            }
            rRooms[i].AddConection(opposite, i - 1);
            i++;
        }

        public static void MakeBranch(ref int i, int BranchStart, int NumberOfRooms, int direction, ref List<Room> rRooms)
        {
            int opposite = -1 * direction;
            if (rRooms[BranchStart].AddConection(direction, i))
            {
                rRooms[i].AddConection(opposite, BranchStart);
                i++;
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
            int iter = 0;
            while (rRooms.Count < size)
            {
                rRooms.Add(new Room("Room" + iter, iter));
                iter++;
            }
            int RoomLength = (int)(size / spread) - 1;

            int i = 1;

            MakeConnections(ref i, RoomLength, 1, ref rRooms);
            MakeConnections(ref i, RoomLength, -1, ref rRooms);
            MakeConnections(ref i, RoomLength, 2, ref rRooms);
            MakeConnections(ref i, RoomLength, -2, ref rRooms);

            int RoomsLeftOver = rRooms.Count - i;
            int[] Dir = { 1, -1, -2, 2 };

            while (i < rRooms.Count)
            {
                MakeBranch(ref i, rand.Next(0, i), 0, Dir[rand.Next(0, 4)], ref rRooms);
            }

            // Console.Write(U.GenerateDungeonString(rRooms));
            return rRooms;
        }
    }
}
