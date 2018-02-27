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
        private static Dictionary<String, Room> roomMap;

        public void Init()
        {
            roomMap = new Dictionary<string, Room>();

            {
                var room = new Room("Room 0", "You are standing in the entrance hall, You want to find someone to fight.");
                room.North = "Room 1";
                roomMap.Add(room.name, room);
            }

            {
                var room = new Room("Room 1", "This room is much like the others, perfect for fighting");
                room.South = "Room 0";
                room.West = "Room 3";
                room.East = "Room 2";
                roomMap.Add(room.name, room);
            }

            {
                var room = new Room("Room 2", "Only two exits, a perfect place for an ambush");
                room.North = "Room 4";
                room.West = "Room 1";
                roomMap.Add(room.name, room);
            }

            {
                var room = new Room("Room 3", "A dead end, One must be carful");
                room.East = "Room 1";
                roomMap.Add(room.name, room);
            }

            {
                var room = new Room("Room 4", "Far from the enterance hall probably a good spot for a quiet murder");
                room.South = "Room 2";
                room.West = "Room 5";
                roomMap.Add(room.name, room);
            }

            {
                var room = new Room("Room 5", "Stop exploring and find some one to kill");
                room.South = "Room 1";
                room.East = "Room 4";
                roomMap.Add(room.name, room);
            }
        }

        public Room GetRandomRoom()
        {
            Random rnd = new Random();
            int num = rnd.Next(0, roomMap.Count());
            Room randomRoom = roomMap.Values.ElementAt(num);
            return randomRoom;
        }

        private void MovePlayer(Player p, Room newRoom)
        {
            p.currentRoom.RemovePlayer(p);
            newRoom.AddPlayer(p);
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
                                   U.NewLineS(player.currentRoom.getDescription());
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

                case "invetory":
                    returnString += U.NewLineS("Inventory:");
                    returnString += player.inventory.GetIventoryDescription();
                    break;

                case "go":
                    // is arg[1] sensible?
                    if ((input[1].ToLower() == "north") && (player.currentRoom.North != null))
                    {
                       MovePlayer(player, roomMap[player.currentRoom.North]);
                    }
                    else
                    {
                        if ((input[1].ToLower() == "south") && (player.currentRoom.South != null))
                        {
                            MovePlayer(player, roomMap[player.currentRoom.South]);
                        }
                        else
                        {
                            if ((input[1].ToLower() == "east") && (player.currentRoom.East != null))
                            {
                                MovePlayer(player, roomMap[player.currentRoom.East]);
                            }
                            else
                            {
                                if ((input[1].ToLower() == "west") && (player.currentRoom.West != null))
                                {
                                   
                                    MovePlayer(player, roomMap[player.currentRoom.West]);
                                    
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
                    returnString = U.NewLineS(player.currentRoom.getDescription());
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
    }
}
