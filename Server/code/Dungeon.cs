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

        //List of current Players
        private static List <Player> playerList;

        public List <Player> GetPlayerList()
        {
            return playerList;
        }

        public void Init()
        {
            roomMap = new Dictionary<string, Room>();
            playerList = new List<Player>();

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

        public void NewClient(String clientName)
        {
            Random rnd = new Random();
            int num = rnd.Next(0, roomMap.Count());
            Room randomRoom = roomMap.Values.ElementAt(num);

            Player newPlayer = new Player(clientName, randomRoom);
            randomRoom.addPlayer(newPlayer);
            playerList.Add(newPlayer);
        }

        private void MovePlayer(Player p, Room newRoom)
        {
            p.currentRoom.removePlayer(p);
            newRoom.addPlayer(p);
            p.currentRoom = newRoom;
        }

        private Player GetPlayerReference(String PlayerName)
        {
            foreach (Player player in playerList)
            {
                if (player.GetPlayerName() == PlayerName)
                {
                    return player;
                }
            }
            return null; 
        }

        public String PlayerAction(String action, String PlayerName)
        {
            String returnString = "";

            Player player = GetPlayerReference(PlayerName);

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
                    returnString = U.NewLineS("Commands are ....")+
                                   U.NewLineS("help - for this screen")+
                                   U.NewLineS("look - to look around")+
                                   U.NewLineS("go [north | south | east | west]  - to travel between locations")+
                                   U.NewLineS("graf [mesage] to add grafiti to your current room")+
                                   U.NewLineS("Press any key to continue");
                    break;

                case "look":
                    //loop straight back
                    returnString = U.NewLineS("you look around") +
                                   U.NewLineS(player.currentRoom.getDescription());
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
                    returnString = U.NewLineS("You added a graffiti");
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
