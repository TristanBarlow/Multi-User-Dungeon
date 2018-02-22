using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading; 

namespace Dungeon
{
    public class DungeonS
    {        
        Dictionary<String, Room> roomMap;

        //Player, Location
        Dictionary<String, Player> playerDictionary;

        public void Init()
        {
            roomMap = new Dictionary<string, Room>();
            playerDictionary = new Dictionary<String, Player>();

            {
                var room = new Room("Room 0", "You are standing in the entrance hall\nAll adventures start here");
                room.North = "Room 1";
                roomMap.Add(room.name, room);
            }

            {
                var room = new Room("Room 1", "You are in room 1");
                room.South = "Room 0";
                room.West = "Room 3";
                room.East = "Room 2";
                roomMap.Add(room.name, room);
            }

            {
                var room = new Room("Room 2", "You are in room 2");
                room.North = "Room 4";
                roomMap.Add(room.name, room);
            }

            {
                var room = new Room("Room 3", "You are in room 3");
                room.East = "Room 1";
                roomMap.Add(room.name, room);
            }

            {
                var room = new Room("Room 4", "You are in room 4");
                room.South = "Room 2";
                room.West = "Room 5";
                roomMap.Add(room.name, room);
            }

            {
                var room = new Room("Room 5", "You are in room 5");
                room.South = "Room 1";
                room.East = "Room 4";
                roomMap.Add(room.name, room);
            }
        }

        public void newClient(String clientName)
        {
            Player newPLayer = new Player(clientName, roomMap["Room 0"]);
            playerDictionary.Add(clientName, newPLayer);
        }

        private String newLineS(String s)
        {
            String newline = "\r\n";
            String finalString = s + newline;
            return finalString;
        }

        public String playerAction(String action, String PlayerName)
        {
            String returnString = "";

            Player player;

            var input = action.Split(' ');

            if (playerDictionary.ContainsKey(PlayerName))
            {
                player = playerDictionary[PlayerName];
            }
            else
            {
                returnString = newLineS("Not in list you fucked up");
                return returnString;
            }

            switch (input[0].ToLower())
            {
                case "help":
                    Console.Clear();
                    returnString = newLineS("Commands are ....")+
                                   newLineS("help - for this screen")+
                                   newLineS("look - to look around")+
                                   newLineS("go [north | south | east | west]  - to travel between locations")+
                                   newLineS("Press any key to continue");
                    break;

                case "look":
                    //loop straight back
                    returnString = newLineS("you look around");
                    Thread.Sleep(1000);
                    break;

                case "say":
                    Console.Write("You say ");
                    for (var i = 1; i < input.Length; i++)
                    {
                        Console.Write(input[i] + " ");
                    }

                    Thread.Sleep(1000);
                    Console.Clear();
                    break;

                case "go":
                    // is arg[1] sensible?
                    if ((input[1].ToLower() == "north") && (player.currentRoom.North != null))
                    {
                        player.currentRoom = roomMap[player.currentRoom.North];
                    }
                    else
                    {
                        if ((input[1].ToLower() == "south") && (player.currentRoom.South != null))
                        {
                            player.currentRoom = roomMap[player.currentRoom.South];
                        }
                        else
                        {
                            if ((input[1].ToLower() == "east") && (player.currentRoom.East != null))
                            {
                                player.currentRoom = roomMap[player.currentRoom.East];
                            }
                            else
                            {
                                if ((input[1].ToLower() == "west") && (player.currentRoom.West != null))
                                {
                                    player.currentRoom = roomMap[player.currentRoom.West];
                                }
                                else
                                {
                                    //handle error
                                   returnString = newLineS("\nERROR")+
                                                  newLineS("\nCan not go " + input[1] + " from here")+
                                                  newLineS("\nPress any key to continue");
                                }
                            }
                        }
                    }
                    returnString = newLineS(player.currentRoom.desc);
                    break;

                default:
                    //handle error
                    returnString = newLineS("\nERROR")+
                                   newLineS("\nCan not " + input)+
                                   newLineS("\nPress any key to continue");
                    break;
            }
            if (returnString != "")
            {
                return returnString;
            }
            else
            {
                returnString = newLineS("welp");
                return returnString;
            }

        }
    }
}
