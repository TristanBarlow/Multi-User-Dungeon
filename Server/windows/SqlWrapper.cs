using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DungeonNamespace;
using PlayerN;
using System.Data;
using Utilities;

#if TARGET_LINUX
using Mono.Data.Sqlite;
using sqliteConnection = Mono.Data.Sqlite.SqliteConnection;
using sqliteCommand = Mono.Data.Sqlite.SqliteCommand;
using sqliteDataReader = Mono.Data.Sqlite.SqliteDataReader;
#endif

#if TARGET_WINDOWS
using System.Data.SQLite;
using sqliteConnection = System.Data.SQLite.SQLiteConnection;
using sqliteCommand = System.Data.SQLite.SQLiteCommand;
using sqliteDataReader = System.Data.SQLite.SQLiteDataReader;
#endif

namespace Server
{
    /**
     *This class handles all the database queries and openeing and everything sql 
     * Could probably write a better wrapper for the actual database queries. But 
     * with I would probably need to add a new struct. Frankly not worth the hastle.
     * for a project this size
     */
    public class SqlWrapper
    { 
        private sqliteConnection Database = null;

        //table names
        private const String dungeonTableName = "DungeonTable";
        private const String playerTableName = "PlayerTable";
        private const String itemTableName = "ItemTable";

        //the create table command
        private const String createTable = "create table if not exists ";

        private const String itemColumns = "(name varchar(30), itemID varchar(10), uniqueID varchar(32), owner varchar(36))";
        private const String playerColumns = " (name varchar(30), password varchar(150), salt varchar(64), rIndex int)";

        //Used to look up item IDs to give player the appropriate weapon descriptions etc
        private GameObjectList gameObjectList;

        //A list of all player strings, so that when seeing who is in the room, offline players arent recoreded.
        private List<String> ActivePlayers = new List<string>();

        private int DungeonSize = 0;

        /**
         * Constructor will try and open the database if none exsists it will create one
         * In addition it will create new tables if non exsist. 
         */
        public SqlWrapper(GameObjectList objectList)
        {

            gameObjectList = objectList;



            //try and open database, if failed make one!
            Database = new sqliteConnection("Data Source=Dungeon" + ";Version=3;FailIfMissing=True");
            try
            {
                Database.Open();
            }
            catch
            {
                Console.WriteLine("Open existing DB failed: So creating one");
                sqliteConnection.CreateFile("Dungeon");
                Database = new sqliteConnection("Data Source=Dungeon" + ";Version=3;FailIfMissing=True");
                Database.Open();
            }

            //do the disable roll back thingy
            //Not sure what this does, fixed a problem, found it on stack overflow
            String disableRollback = "PRAGMA journal_mode = OFF";
            sqliteCommand cmd = new sqliteCommand();
            cmd.Connection = Database;
            cmd.CommandText = disableRollback;
            cmd.ExecuteNonQuery();

            //create tables
            new sqliteCommand(createTable + itemTableName + itemColumns, Database).ExecuteNonQuery();

            new sqliteCommand(createTable + playerTableName + playerColumns ,  Database).ExecuteNonQuery();
        }

        /**
         *This function distributes items randomly amongst the dungeon
         * @param numberOfItems the number of items to scatter
         */
        public void AddItems(int numberOfItems)
        {
            sqliteCommand command;
            Random r = new Random();

            //populate item table with items
            for (int i = 0; i < numberOfItems; i++)
            {
                String room = "room";
                room += r.Next(0, DungeonSize);
                Item item = gameObjectList.GetRandomItem();

                command = new sqliteCommand("INSERT INTO " + itemTableName +
                            "  (name, itemID, uniqueID, owner) " +
                            "VALUES (?,?,?,?) ", Database);
                command.Parameters.Add("@name", DbType.String).Value = item.itemName.ToLower();
                command.Parameters.Add("@itemID", DbType.String).Value = item.ID;
                command.Parameters.Add("@uniqueID", DbType.String).Value = Guid.NewGuid().ToString();
                command.Parameters.Add("@owner", DbType.String).Value = room;
                command.ExecuteNonQuery();
            }
        }

        /**
         * Try and move an item from an oldowner to a new owner
         * @param oldOwner the owner trying to move the item from
         * @param newOwner the owner tyring to move the item too. 
         * @param item the name of the item to be moved.
         */
        public String MoveItem(String oldOwner, String newOwner, String item)
        {
            var command = new sqliteCommand("select * from " + itemTableName + " where owner =:owner AND name =:item ", Database);
            command.Parameters.Add("owner", DbType.String).Value = oldOwner;
            command.Parameters.Add("item", DbType.String).Value = item;

            try
            {
                var reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    if (ChangeItemOwner(reader["uniqueID"].ToString(), newOwner))
                    {
                        return "You ";
                    }

                }
            }
            catch (Exception ex)
            {
                Console.Write("Item Not recognised: " + ex);
            }
           
            return "Failed To ";
        }

        /**
         *This function actually changes owner of an item.
         * @param itemID the unique ID of the item in question
         * @param owner the name of the new owner of the item.
         */
        public bool ChangeItemOwner(String itemID, String owner)
        {
            var command = new sqliteCommand(Database);
            command.CommandText = "update " + itemTableName + " set owner =:own where uniqueID=:item";
            command.Parameters.Add("own", DbType.String).Value = owner;
            command.Parameters.Add("item", DbType.String).Value = itemID;
            try
            {
                command.ExecuteNonQuery();
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
            
        }

        /**
         *Get all items with where the owner field is equal to that of the players name
         * @param owner the entity to whom the items belong.
         */
        public String GetOwnedItems(String owner)
        {
            String rString = U.NL("");
            var command = new sqliteCommand("select * from " + itemTableName + " where owner =:name ", Database);
            command.Parameters.Add("name", DbType.String).Value = owner;

            try
            {
                var reader = command.ExecuteReader();
                while(reader.Read())
                {
                    //make the description
                    String id = reader["itemID"].ToString();
                    rString += U.NL("Name: " +gameObjectList.GetItem(id).itemName);
                    rString += U.NL("Description: " +gameObjectList.GetItem(id).description);
                    U.NL("");
                }
            }
            catch (Exception ex)
            {
                Console.Write("Failed to get inventory" + ex);
                rString += "Could not find items";
            }
            return rString;
        }

        /**
         *Adds a new player to the player table 
         * @param tempPlayer the player so far that we will add.
         * @param password the salted-hashes password the player used to login
         * @param salt the salt used to hash the password and used for encryption
         */
        public bool AddPlayer(Player tempPlayer, String password, String salt)
        {

            var command = new sqliteCommand("select * from " + playerTableName + " where name =:id", Database);
            command.Parameters.Add("id", DbType.String).Value = tempPlayer.PlayerName;
            var reader = command.ExecuteReader();

            if (reader.HasRows == false && !U.HasBadChars(tempPlayer.PlayerName) && !U.HasBadChars(password))
            {

                try
                {
					command = new sqliteCommand("INSERT INTO " + playerTableName + 
					                            " (name, password, salt,  rIndex) "+ 
					                            "VALUES ($n, $p, $s, $i) ", Database);

                    command.Parameters.Add("$n", DbType.String).Value = tempPlayer.PlayerName;
                    command.Parameters.Add("$p", DbType.String).Value = password;
                    command.Parameters.Add("$s", DbType.String).Value = salt;
                    command.Parameters.Add("$i", DbType.Int32).Value = tempPlayer.RoomIndex;
                    command.ExecuteNonQuery();

                    ActivePlayers.Add(tempPlayer.PlayerName);
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed Adding to DB: " + ex);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /**
         * Given a user name this will return the salt for that username
         * if no user by that name exists it will return blank
         * @param username the user to check for.
         */
        public String GetSalt(String username)
        {
            var command = new sqliteCommand("select * from " + playerTableName + " where name =:id", Database);
            command.Parameters.Add("id", DbType.String).Value = username;
            var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return reader["salt"].ToString();
            }
            else return "";
        }

        /**
         *Given a players login credientials it will edit the input player with the 
         * matching table entry IF the credentials all match up.
         * @param player the player which will be written to.
         * @param username the name of the player trying to login
         * @param the password the player is trying to login with
         */
        public bool GetPlayerLogin(ref Player player, String username, String password)
        {
            var command = new sqliteCommand("select * from " + playerTableName + " where name =:id", Database);
            command.Parameters.Add("id", DbType.String).Value = username;
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                //check if passwords match
                if (password != reader["password"].ToString())
                {
                    return false;
                }
                else
                {
                    player.PlayerName = reader["name"].ToString();
                    player.RoomIndex = Int32.Parse(reader["rIndex"].ToString());
                    ActivePlayers.Add(player.PlayerName);
                    return true;
                }
            }
            return false;
        }

        /**
         *Gets all the name of all the players in room index provided. Filtering 
         * offline players in that room
         * @param i the index of the room in question
         */
        public String GetPlayersInRoom(int i)
        {
            var command = new sqliteCommand("select * from " + playerTableName + " where rIndex = '" + i + "'", Database);
            var reader = command.ExecuteReader();
            try
            {
                String players = U.NL("Players In room:");
                while (reader.Read())
                {
                    String name = reader["name"].ToString();
                    if (ActivePlayers.Contains(name))
                    {
                        players += " | " + name + " | ";
                    }
                }
                return players;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting players in room" + ex);
            }
            return "";
        
        }

        /**
         *Update the database to players move.
         * @param player the player that has moved
         */
        public string UpdatePlayerPos(Player player)
        {
            var command = new sqliteCommand(Database);
            command.CommandText ="update " + playerTableName + " set rIndex = :i where name=:id";
            command.Parameters.Add("i", DbType.Int32).Value = player.RoomIndex;
            command.Parameters.Add("id", DbType.String).Value = player.PlayerName;
            try
            {
                command.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
                Console.WriteLine("failed to write player: " + player.PlayerName + ex);
            }
            return GetRoomDescrption(player.RoomIndex);
        }

        /**
         *Looks up the required database entries to form the String that makes up the room description
         * Really boring function just a bunch of look ups or calls to other functions that do lookups.
         * @param i the index of the room to get the description for
         */
        public String GetRoomDescrption(int i)
        {
            String rString = "";
            var command = new sqliteCommand("select * from " + dungeonTableName + " where rIndex = '" + i + "'", Database);
            try
            {
                var reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    rString += U.NL(reader["name"].ToString());
                    rString += U.NL(reader["description"].ToString());

                    rString += U.NL("");

                    int north = Int32.Parse(reader["north"].ToString());
                    int east = Int32.Parse(reader["east"].ToString());
                    int south = Int32.Parse(reader["south"].ToString());
                    int west = Int32.Parse(reader["west"].ToString());

                    rString += "Exits are: ";
                    if (north != -1) rString += "|NORTH| ";
                    if (east != -1) rString += "|EAST| ";
                    if (south != -1) rString += "|SOUTH| ";
                    if (west != -1) rString += "|WEST| ";


                    rString += U.NL("\r\n");

                    rString += U.NL("Grafiti: ");
                    rString += U.NL(reader["grafiti"].ToString());
                    rString += U.NL(" ");


                    rString += U.NL("Items on the floor: ");
                    rString += GetOwnedItems("room" + reader["rIndex"]);

                    rString += U.NL("");

                    rString += U.NL(GetPlayersInRoom(i));

                    rString += U.NL("");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting room description" + ex);
            }
            return rString;
        }

        /**
         *Adds grafiti to a room
         * @param graf the message to write to the wall
         * @param roomindex the index of the room to write too
         */
        public void AddGrafiti(String graf, int roomIndex)
        {
            var command = new sqliteCommand(Database);
            command.CommandText = "update " + dungeonTableName + " set grafiti = :graf where rIndex=:id";
            command.Parameters.Add("graf",DbType.String ).Value = graf;
            command.Parameters.Add("id", DbType.Int32).Value = roomIndex.ToString();
            try
            {
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("failed to write grafiti: " + graf + ex);
            }
        }

        /**
         *Given a reference of a dungeon. This will write it to the database.
         * Only used after a new dungeon is created
         */
        public void AddDungeon(Dungeon d)
        {

            //check to see if there is an exxsisting dungeon table, if so cya!
           new sqliteCommand("drop table if exists " + dungeonTableName, Database).ExecuteNonQuery();

            //create the new table
            new sqliteCommand("create table "+ dungeonTableName + " (name varchar(50), " +
                  "description varchar(300), rIndex int , north int , " +
                  "east int, south int, west int, grafiti varchar(300))", Database).ExecuteNonQuery();


            //cycle through the rooms in the dungeon and write them to the table
            List <Room> roomList =  d.GetRoomList();
            DungeonSize = roomList.Count;
            foreach (Room r in roomList)
            {
               
                sqliteCommand command;
                try
                {
                    command = new sqliteCommand("INSERT INTO " + dungeonTableName +
                            "  (name, description, rIndex, north, east, south, west, grafiti) " +
                            "VALUES (?,?,?,?,?,?,?,?) ", Database);
                    command.Parameters.Add("@name", DbType.String).Value = r.name;
                    command.Parameters.Add("@password", DbType.String).Value = r.desc;
                    command.Parameters.Add("@rIndex", DbType.Int32).Value = r.RoomIndex;
                    command.Parameters.Add("@north", DbType.Int32).Value = r.north;
                    command.Parameters.Add("@east", DbType.Int32).Value = r.east;
                    command.Parameters.Add("@south", DbType.Int32).Value = r.south;
                    command.Parameters.Add("@west", DbType.Int32).Value = r.west;
                    command.Parameters.Add("@grafiti", DbType.String).Value = "";
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to add: " + r.name + ex);
                }

            }
        }

        /**
         *Used if loading from an exsisitng dungeon. It will cycle through the dungeon table 
         * getting all the rooms and adding them to the new dungeons room list
         * Having the dungeon as dynamic memory, allows for quicker sanity checks incase
         * players try and break things. The databse is always written to if the player
         * actuallty does anything.
         */
        public Dungeon GetDungeon()
        {
            Dungeon d = new Dungeon();
            try
            {
                var command = new sqliteCommand("select * from " + dungeonTableName, Database);
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Room r = new Room();
                    r.name = reader["name"].ToString();
                    r.desc = reader["description"].ToString();
                    r.RoomIndex = Int32.Parse(reader["rIndex"].ToString());
                    r.north = Int32.Parse(reader["north"].ToString());
                    r.east = Int32.Parse(reader["east"].ToString());
                    r.south = Int32.Parse(reader["south"].ToString());
                    r.west = Int32.Parse(reader["west"].ToString());
                    d.GetRoomList().Add(r);
                }

                reader.Close();
                Console.WriteLine("");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to display DB" + ex);
            }
            return d;
        }

        /**
         *When a player disconnects remove them from the active players list
         *@param name of the player to be removed.
         */
        public void RemoveActivePlayer(String name)
        {
            if (ActivePlayers.Contains(name))
            {
                ActivePlayers.Remove(name);
            }
        }

    }
}
