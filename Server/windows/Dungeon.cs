using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Utilities;
using PlayerN;
using Server;

namespace DungeonNamespace
{
    /**
     *Used for the generation of the map only 
     */
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

    /**
     *This is where all player dungeon actions happen. Handles the logic for player actions all in one space. In addition
     * Responsible for the generation of the map.
     */
    public class Dungeon
    {        
        //private Dictionary<String, Room> roomMap;
        private List<Room> RoomList= new List<Room>();

        //The string which is sent to the players so they can draw the dungeon their side.
        private String DungeonStr = " ";

        //map generation stuff
        public static Vector2D NORTH = new Vector2D(0, 1);
        public static Vector2D EAST = new Vector2D(1, 0);
        public static Vector2D SOUTH = new Vector2D(0, -1);
        public static Vector2D WEST = new Vector2D(-1, 0);

        private static Random rand = new Random(); 

        /**
         *Sometimes the dungeon is loaded from a DB so this isnt always needed. this used when a new dungeon is created
         * @param size the amount of rooms to have in the dungeon
         * @param objectlist the list of all objects includeing rooms and their descriptions.
         */
        public void Init(int size, GameObjectList objectList)
        {
            RoomList = GenerateDungeon(size, objectList);
            DungeonStr = GenerateDungeonString(RoomList);
        }

        /**
         *Returns the dugeon string, if none is present it will generate one. 
         */
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

        /**
         *Used when a new player logs in, or when a player is in a room they shouldnt be(smaller dungeon generated) 
         */
        public Room GetRandomRoom()
        {
            Random rnd = new Random();
            int num = rnd.Next(0, RoomList.Count);
            Room randomRoom = RoomList[num];
            return randomRoom;
        }

        /**
         *Gets the help message, was a bit messy having it in the main function 
         */
        private String HelpMessage()
        {
         return U.NL("Commands are ....") +
               U.NL("help - for this screen") +
               U.NL("look - to look around") +
               U.NL("go [north | south | east | west]  - to travel between locations") +
               U.NL("graf [mesage] to add grafiti to your current room") +
               U.NL("say [mesage] will speak to your current room") +
               U.NL("pickup [name] to pick up the item of that name in the room") +
               U.NL("drop [name] to drop the item of that name into the room") +
               U.NL("inventory will look at the items you have collected") +
               U.NL("Press any key to continue");
        }

        /**
         *The main function which handles all game logic.
         * @param action the message the client has sent to the server
         * @param player the player who sent the message
         * @param database the database reference for database lookups and writes
         */
        public ActionResponse PlayerAction(String action,Player player, SqlWrapper database)

        {
            String[] input = action.Split(' ');
            String subject = "";
            ActionResponse rAction = new ActionResponse();

            //gets the rest of the action which isnt the first word(the command)
            if (input.Length > 1)
            {
                subject = action.Remove(0, input[0].Count() + 1).ToLower();
            }

            //sanity check, this has never happened but it cant hurt to have
            if (player == null)
            {
                rAction.message = U.NL("Not in list you do not exsist");
                return rAction;
            }

            //Get the currnt room once. 
           Room currentRoom = RoomList[player.roomIndex];

            switch (input[0].ToLower())
            {
                case "help":
                    Console.Clear();
                    rAction.Set(HelpMessage(), ActionID.NORMAL);
                    break;

                case "look":
                    String temp = database.GetRoomDescrption(currentRoom.RoomIndex);
                    rAction.Set(temp, ActionID.NORMAL);
                    break;

                case "graf":
                    database.AddGrafiti(subject, currentRoom.RoomIndex);
                    rAction.Set("You added Grafiti: " + subject, ActionID.UPDATE);
                    break;

                case "pickup":
                    String pik = database.MoveItem("room" + player.roomIndex, "player" + player.PlayerName, subject);
                    pik += "Pick up Item: " + subject;
                    rAction.Set(pik, ActionID.UPDATE);
                    break;

                case "drop":
                    String drp = database.MoveItem("player" + player.PlayerName, "room" + player.roomIndex, subject);
                    drp += "Drop Item: " + subject;
                    rAction.Set( drp,ActionID.UPDATE);
                    break;

                case "inventory":
                    rAction.Set(database.GetOwnedItems("player" + player.PlayerName), ActionID.NORMAL);
                    break;

                case "say":
                    rAction.Set(subject, ActionID.SAY);
                    break;
                    

                case "go":
                    // is arg[1] sensible?
                    int[] indexs = currentRoom.GetExitIndexs();

                    if ((input[1].ToLower() == "north") && (currentRoom.north != -1))
                    {
                         player.SetRoom(RoomList[currentRoom.north]);
                        
                    }
                     else if ((input[1].ToLower() == "east") && (currentRoom.east != -1))
                    {
                       player.SetRoom(RoomList[currentRoom.east]);
                    }
                    else if ((input[1].ToLower() == "south") && (currentRoom.south != -1))
                    {
                        player.SetRoom(RoomList[currentRoom.south]);
                    }
                    else if ((input[1].ToLower() == "west") && (currentRoom.west != -1))
                    {
                        player.SetRoom(RoomList[currentRoom.west]);           
                    }
                    else
                    {
                        //handle error
                        rAction.Set(U.NL("\nERROR") +
                                    U.NL("\nCan not go " + input[1] + 
                                    " from here"), ActionID.UPDATE);
                        return rAction;
                    }
                    String str = U.NL(database.UpdatePlayerPos(player));
                    rAction.Set(str, ActionID.MOVE);
                    break;

                default:
                    //handle error
                    rAction.Set(U.NL("\nERROR") + HelpMessage(), ActionID.NORMAL);
                    break;
            }
            return rAction;

        }

        /**
         *Creates a dungeon string when given a list of rooms that make the dungeon
         * @param RoomList the rooms that are to be used to generate the string
         */
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

        /**
         *Map generation stuff 
         */
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

        /**
         *Map generation stuff 
         */
        public static List<Room> GenerateDungeon(int size, GameObjectList gol)
        {
            //check if its logical map
            if (size < 4)
            {
                size = 4;
            }

            //add one for origin
            size += 1;


            List<Room> rRooms = new List<Room>();
            List<Vector2D> usedPositions = new List<Vector2D>();
            List<Room> usedRooms = new List<Room>();


            //create empty rooms
            int iter = 0;
            while (rRooms.Count < size)
            {
                Room r = new Room("Room" + iter, iter);

                KeyValuePair<String, String> RoomDesc = gol.GetRandomRoom();

                r.name = RoomDesc.Key;
                r.desc = RoomDesc.Value;
                rRooms.Add(r);
                iter++;
            }

            //set up possible directions
            Vector2D[] Dir = {NORTH, EAST, SOUTH, WEST};

            //make origin and attach first four rooms manually
            Room origin = rRooms[0];
            rRooms.Remove(origin);
            origin.Position = new Vector2D(0, 0);
            usedRooms.Add(origin);

            MakeBranch(origin, rRooms[0], NORTH, usedRooms, rRooms, usedPositions);
            MakeBranch(origin, rRooms[0], EAST, usedRooms, rRooms, usedPositions);
            MakeBranch(origin, rRooms[0], SOUTH, usedRooms, rRooms, usedPositions);
            MakeBranch(origin, rRooms[0], WEST, usedRooms, rRooms, usedPositions);

            //If this vec is returned we know the room is out of directions
             Vector2D badVec = new Vector2D(0, 0);

            //populate until there are no more rooms left 
            while (rRooms.Count > 1)
            {
                int roomIndex = rand.Next(usedRooms.Count);

                Vector2D dir = usedRooms[roomIndex].GetAvailableDirection();
                if (!dir.Equals(badVec) && rand.NextDouble() >= 0.4)
                {
                    MakeBranch(usedRooms[roomIndex], rRooms[0], dir, usedRooms, rRooms, usedPositions);
                }
            }

            return usedRooms;
        }
    }
}
