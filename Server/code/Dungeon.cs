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
                var room = new Room("Room 1", "this the best room you have ever been in");
                room.South = "Room 0";
                room.West = "Room 3";
                room.East = "Room 2";
                roomMap.Add(room.name, room);
            }

            {
                var room = new Room("Room 2", "this room is even better!");
                room.North = "Room 4";
                roomMap.Add(room.name, room);
            }

            {
                var room = new Room("Room 3", "omg how");
                room.East = "Room 1";
                roomMap.Add(room.name, room);
            }

            {
                var room = new Room("Room 4", "buy battle screens");
                room.South = "Room 2";
                room.West = "Room 5";
                roomMap.Add(room.name, room);
            }

            {
                var room = new Room("Room 5", "this story is so good");
                room.South = "Room 1";
                room.East = "Room 4";
                roomMap.Add(room.name, room);
            }
        }

        public void newClient(String clientName)
        {
            Room defaultRoom = roomMap["Room 0"];
            Player newPLayer = new Player(clientName, defaultRoom);
            defaultRoom.addPlayer(newPLayer);
            playerDictionary.Add(clientName, newPLayer);
        }

        private void movePlayer(Player p, Room newRoom)
        {
            p.currentRoom.removePlayer(p);
            newRoom.addPlayer(p);
            p.currentRoom = newRoom;
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
                returnString = U.newLineS("Not in list you fucked up");
                return returnString;
            }

            switch (input[0].ToLower())
            {
                case "help":
                    Console.Clear();
                    returnString = U.newLineS("Commands are ....")+
                                   U.newLineS("help - for this screen")+
                                   U.newLineS("look - to look around")+
                                   U.newLineS("go [north | south | east | west]  - to travel between locations")+
                                   U.newLineS("graf [mesage] to add grafiti to your current room")+
                                   U.newLineS("Press any key to continue");
                    break;

                case "look":
                    //loop straight back
                    returnString = U.newLineS("you look around") +
                                   U.newLineS(player.currentRoom.getDescription());
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

                case "graf":
                    int index = action.IndexOf(' ');
                    String second = action.Substring(index + 1);
                    player.currentRoom.addGraf(second);
                    returnString = U.newLineS("You added a graffiti");
                    break;

                case "go":
                    // is arg[1] sensible?
                    if ((input[1].ToLower() == "north") && (player.currentRoom.North != null))
                    {
                       movePlayer(player, roomMap[player.currentRoom.North]);
                    }
                    else
                    {
                        if ((input[1].ToLower() == "south") && (player.currentRoom.South != null))
                        {
                            movePlayer(player, roomMap[player.currentRoom.South]);
                        }
                        else
                        {
                            if ((input[1].ToLower() == "east") && (player.currentRoom.East != null))
                            {
                                movePlayer(player, roomMap[player.currentRoom.East]);
                            }
                            else
                            {
                                if ((input[1].ToLower() == "west") && (player.currentRoom.West != null))
                                {
                                   
                                    movePlayer(player, roomMap[player.currentRoom.West]);
                                    
                                }
                                else
                                {
                                    //handle error
                                   returnString = U.newLineS("\nERROR")+
                                                  U.newLineS("\nCan not go " + input[1] + " from here")+
                                                  U.newLineS("\nPress any key to continue");
                                }
                            }
                        }
                    }
                    returnString = U.newLineS(player.currentRoom.getDescription());
                    break;

                default:
                    //handle error
                    returnString = U.newLineS("\nERROR")+
                                   U.newLineS("\nCan not " + input)+
                                   U.newLineS("\nPress any key to continue");
                    break;
            }
            if (returnString != "")
            {
                return returnString;
            }
            else
            {
                returnString = U.newLineS("welp");
                return returnString;
            }

        }
    }
}
