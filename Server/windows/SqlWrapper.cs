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
    public class SqlWrapper
    {
        private sqliteConnection Database = null;

        private const String dungeonTableName = "DungeonTable";
        private const String playerTableName = "PlayerTable";
        private const String itemTableName = "ItemTable";

        private const String createTable = "create table if not exists ";

        private const String itemColumns = "(name varchar(30), itemID varchar(10), uniqueID varchar(32), owner varchar(36))";
        private const String playerColumns = " (name varchar(30), password varchar(150), salt varchar(64), rIndex int)";

        private GameObjectList gameObjectList;

        private List<String> ActivePlayers = new List<string>();

        private int DungeonSize = 0;

        public SqlWrapper(GameObjectList objectList)
        {
            gameObjectList = objectList;
            sqliteCommand cmd = new sqliteCommand();
            String disableRollback = "PRAGMA journal_mode = OFF";


            Database = new sqliteConnection("Data Source=Dungeon" + ";Version=3;FailIfMissing=True");
            try
            {
                Database.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Open existing DB failed: So creating one");
                sqliteConnection.CreateFile("Dungeon");
                Database = new sqliteConnection("Data Source=Dungeon" + ";Version=3;FailIfMissing=True");
                Database.Open();
            }
            cmd.Connection = Database;
            cmd.CommandText = disableRollback;
            cmd.ExecuteNonQuery();

            new sqliteCommand(createTable + itemTableName + itemColumns, Database).ExecuteNonQuery();

            new sqliteCommand(createTable + playerTableName + playerColumns ,  Database).ExecuteNonQuery();
        }

        public void AddItems(int numberOfItems, int numberOfWeapons)
        {
            sqliteCommand command;
            Random r = new Random();



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

        public String MoveItem(String oldOwner, String newOwner, String item)
        {
            var command = new sqliteCommand("select * from " + itemTableName + " where owner = '" + oldOwner + "' AND name = "+ "'"+ item + "'", Database);

            try
            {
                var reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    if (ChangeItemOwner(reader["uniqueID"].ToString(), itemTableName, newOwner))
                    {
                        return "Moved Item: " + item;
                    }

                }
            }
            catch (Exception ex)
            {
                Console.Write("Item Not recognised");
            }
           
            return "Failed to move: " + item;
        }

        public bool ChangeItemOwner(String itemID, String tableName, String owner)
        {
            var command = new sqliteCommand(Database);
            command.CommandText = "update " + tableName + " set owner =:own where uniqueID=:item";
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

        public String GetOwnedItems(String owner)
        {
            String rString = U.NewLineS("");
            var command = new sqliteCommand("select * from " + itemTableName + " where owner = '" + owner + "'", Database);
            try
            {
                var reader = command.ExecuteReader();
                while(reader.Read())
                {
                    String id = reader["itemID"].ToString();
                    rString += U.NewLineS("Name: " +gameObjectList.GetItem(id).itemName);
                    rString += U.NewLineS("Description: " +gameObjectList.GetItem(id).description);
                    U.NewLineS("");
                }
            }
            catch (Exception ex)
            {
                Console.Write("Failed to get inventory" + ex);
                rString += "Could not find items";
            }
            return rString;
        }

        public bool AddPlayer(Player tempPlayer, String password, String salt)
        {

            var command = new sqliteCommand(Database);
            command.CommandText = ("select * from " + playerTableName + " where name = '" + tempPlayer.PlayerName+ "'");
            var reader = command.ExecuteReader();

            if (reader.HasRows == false)
            {

                try
                {
					command = new sqliteCommand("INSERT INTO " + playerTableName + 
					                            " (name, password, salt,  rIndex) "+ 
					                            "VALUES ($n, $p, $s, $i) ", Database);
                    command.Parameters.Add("$n", DbType.String).Value = tempPlayer.PlayerName;
                    command.Parameters.Add("$p", DbType.String).Value = password;
                    command.Parameters.Add("$s", DbType.String).Value = salt;
                    command.Parameters.Add("$i", DbType.Int32).Value = tempPlayer.roomIndex;
                    command.ExecuteNonQuery();
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

        public String GetSalt(String Username)
        {
            var command = new sqliteCommand("select * from " + playerTableName + " where name = '" + Username + "'", Database);
            var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return reader["salt"].ToString();
            }
            else return null;
        }

        public bool GetPlayerLogin(ref Player p, String Username, String password)
        {
            var command = new sqliteCommand("select * from " + playerTableName + " where name = '" + Username + "'", Database);
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                if (password != reader["password"].ToString())
                {
                    return false;
                }
                else
                {
                    p.PlayerName = reader["name"].ToString();
                    p.roomIndex = Int32.Parse(reader["rIndex"].ToString());
                    ActivePlayers.Add(p.PlayerName);
                    return true;
                }
            }
            return false;
        }

        public String GetPlayersInRoom(int i)
        {
            var command = new sqliteCommand("select * from " + playerTableName + " where rIndex = '" + i + "'", Database);
            var reader = command.ExecuteReader();
            try
            {
                String players = U.NewLineS("Players In room:");
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

        public string UpdatePlayerPos(Player p)
        {
            var command = new sqliteCommand(Database);
            command.CommandText ="update " + playerTableName + " set rIndex = :i where name=:id";
            command.Parameters.Add("i", DbType.Int32).Value = p.roomIndex;
            command.Parameters.Add("id", DbType.String).Value = p.PlayerName;
            try
            {
                command.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
                Console.WriteLine("failed to write player: " + p.PlayerName + ex);
            }
            return GetRoomDescrption(p.roomIndex);
        }

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
                    rString += U.NewLineS(reader["name"].ToString());
                    rString += U.NewLineS(reader["description"].ToString());

                    rString += U.NewLineS("");

                    int north = Int32.Parse(reader["north"].ToString());
                    int east = Int32.Parse(reader["east"].ToString());
                    int south = Int32.Parse(reader["south"].ToString());
                    int west = Int32.Parse(reader["west"].ToString());

                    rString += "Exits are: ";
                    if (north != -1) rString += "|NORTH| ";
                    if (east != -1) rString += "|EAST| ";
                    if (south != -1) rString += "|SOUTH| ";
                    if (west != -1) rString += "|WEST| ";

                    rString += U.NewLineS("");

                    rString += U.NewLineS("Items on the floor: ");
                    rString += GetOwnedItems("room" + reader["rIndex"]);

                    rString += U.NewLineS("");

                    rString += U.NewLineS(GetPlayersInRoom(i));

                    rString += U.NewLineS("");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting room description" + ex);
            }
            return rString;
        }

        public void AddDungeon(Dungeon d)
        {

           new sqliteCommand("drop table if exists " + dungeonTableName, Database).ExecuteNonQuery();

            new sqliteCommand("create table "+ dungeonTableName + " (name varchar(50), " +
                  "description varchar(300), rIndex int , north int , " +
                  "east int, south int, west int)", Database).ExecuteNonQuery();


            List <Room> roomList =  d.GetRoomList();
            DungeonSize = roomList.Count;
            foreach (Room r in roomList)
            {
               
                sqliteCommand command;
                try
                {
                    command = new sqliteCommand("INSERT INTO " + dungeonTableName +
                            "  (name, description, rIndex, north, east, south, west) " +
                            "VALUES (?,?,?,?,?,?,?) ", Database);
                    command.Parameters.Add("@name", DbType.String).Value = r.name;
                    command.Parameters.Add("@password", DbType.String).Value = r.desc;
                    command.Parameters.Add("@rIndex", DbType.Int32).Value = r.RoomIndex;
                    command.Parameters.Add("@north", DbType.Int32).Value = r.north;
                    command.Parameters.Add("@east", DbType.Int32).Value = r.east;
                    command.Parameters.Add("@south", DbType.Int32).Value = r.south;
                    command.Parameters.Add("@west", DbType.Int32).Value = r.west;
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to add: " + r.name + ex);
                }

            }
        }

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
                Console.WriteLine("Failed to display DB");
            }
            return d;
        }

        public void RemoveActivePlayer(String name)
        {
            if (ActivePlayers.Contains(name))
            {
                ActivePlayers.Remove(name);
            }
        }

    }
}
