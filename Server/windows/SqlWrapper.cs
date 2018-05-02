using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DungeonNamespace;
using PlayerN;
using System.Data;

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
    class SqlWrapper
    {
		private sqliteConnection Database = null;

        private String dungeonTableName = "DungeonTable";
        private String playerTableName = "PlayerTable";
        private GameObjectList gameObjectList;

       public SqlWrapper(GameObjectList objectList)
        {
            gameObjectList = objectList;
			sqliteCommand cmd = new sqliteCommand ();
			String disableRollback  = "PRAGMA journal_mode = OFF";


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
			cmd.ExecuteNonQuery ();
        }

        public bool AddPlayer(Player tempPlayer, String password)
        {

            var command = new sqliteCommand(Database);
            command.CommandText = ("select * from " + playerTableName + " where name = '" + tempPlayer.PlayerName+ "'");
            var reader = command.ExecuteReader();

            if (reader.HasRows == false)
            {
                new sqliteCommand("create table if not exists " + playerTableName + " (name varchar(30), " +
                                  "password varchar(150), rIndex int, inventory varchar(300))", Database).ExecuteNonQuery();
                try
                {
					command = new sqliteCommand("INSERT INTO " + playerTableName + 
					                            " (name, password, rIndex, inventory) "+ 
					                            "VALUES ($n, $p, $i, $invent) ", Database);
                    command.Parameters.Add("$n", DbType.String).Value = tempPlayer.PlayerName;
                    command.Parameters.Add("$p", DbType.String).Value = password;
                    command.Parameters.Add("$i", DbType.Int32).Value = tempPlayer.roomIndex;
                    command.Parameters.Add("$invent", DbType.String).Value = tempPlayer.GetInventory().GetInventoryID();
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

        public bool GetPlayerLogin(ref Player p, String Username, String password)
        {
            
            new sqliteCommand("create table if not exists " + playerTableName + " (name varchar(30), " +
                              "password varchar(150), rIndex int, inventory varchar(300))", Database).ExecuteNonQuery();

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
                    p.GetInventory().MakeInventory(reader["inventory"].ToString(), gameObjectList);
                    return true;
                }
            }
            return false;
        }

        public void WritePlayer(Player p)
        {
            var command = new sqliteCommand(Database);
            command.CommandText ="update " + playerTableName + " set rIndex = :i, inventory = :inv where name=:id";
            command.Parameters.Add("i", DbType.Int32).Value = p.roomIndex;
            command.Parameters.Add("inv", DbType.String).Value = p.GetInventory().GetInventoryID();
            command.Parameters.Add("id", DbType.String).Value = p.PlayerName;
            try
            {
                command.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
                Console.WriteLine("failed to write player: " + p.PlayerName + ex);
            }
        }

        public void AddDungeon(Dungeon d)
        {

           new sqliteCommand("drop table if exists " + dungeonTableName, Database).ExecuteNonQuery();

            new sqliteCommand("create table "+ dungeonTableName + " (name varchar(50), " +
                  "description varchar(300), rIndex int , north int , " +
                  "east int, south int, west int, inventory varchar(300))", Database).ExecuteNonQuery();


            List <Room> roomList =  d.GetRoomList();
            foreach (Room r in roomList)
            {
               
                sqliteCommand command;

                try
                {
                    command = new sqliteCommand("INSERT INTO " + dungeonTableName +
                            "  (name, description, rIndex, north, east, south, west, inventory) " +
                            "VALUES (?,?,?,?,?,?,?,?) ", Database);
                    command.Parameters.Add("@name", DbType.String).Value = r.name;
                    command.Parameters.Add("@password", DbType.String).Value = r.desc;
                    command.Parameters.Add("@rIndex", DbType.Int32).Value = r.RoomIndex;
                    command.Parameters.Add("@north", DbType.Int32).Value = r.north;
                    command.Parameters.Add("@east", DbType.Int32).Value = r.east;
                    command.Parameters.Add("@south", DbType.Int32).Value = r.south;
                    command.Parameters.Add("@west", DbType.Int32).Value = r.west;
                    command.Parameters.Add("@inventory", DbType.String).Value = r.GetInventory().GetInventoryID();
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
                    r.GetInventory().MakeInventory(reader["inventory"].ToString(), gameObjectList);
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

        public void WriteRoom(Room r)
        {
            var command = new sqliteCommand(Database);
            command.CommandText = "update " + dungeonTableName + " set inventory = :inv where rIndex=:id";
            command.Parameters.Add("inv", DbType.String).Value = r.GetInventory().GetInventoryID();
            command.Parameters.Add("id", DbType.String).Value = r.RoomIndex;
            try
            {
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("failed to write Room: " + r.name + ex);
            }
            
        }
    }
}
