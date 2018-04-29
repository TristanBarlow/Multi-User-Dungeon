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
		sqliteConnection DungeonDatabase = null;
        sqliteConnection PlayerDatabase = null;

        String DungeonName = "DungeonTable";
        String PlayersName = "PlayerTable";

       public SqlWrapper()
        {
            DungeonDatabase = new sqliteConnection("Data Source=Dungeon" + ";Version=3;FailIfMissing=True");
            try
            {
                DungeonDatabase.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Open existing DB failed: So creating one");
                sqliteConnection.CreateFile("Dungeon");
                DungeonDatabase = new sqliteConnection("Data Source=Dungeon" + ";Version=3;FailIfMissing=True");
                DungeonDatabase.Open();
            }


            PlayerDatabase = new sqliteConnection("Data Source=Players" + ";Version=3;FailIfMissing=True");
            try
            {
                PlayerDatabase.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Open existing DB failed: So creating one");
                sqliteConnection.CreateFile("Players");
                PlayerDatabase = new sqliteConnection("Data Source=Players" + ";Version=3;FailIfMissing=True");
                PlayerDatabase.Open();
            }

        }

        public bool AddPlayer(Player tempPlayer, String password)
        {
            new sqliteCommand("create table if not exists " + PlayersName + " (name varchar(30), " +
                  "password varchar(150), rIndex int)", PlayerDatabase).ExecuteNonQuery();

            var command = new sqliteCommand(PlayerDatabase);
            command.CommandText = ("select * from " + PlayersName + " where name = '" + tempPlayer.PlayerName+ "'");
            var reader = command.ExecuteReader();

            if (reader.HasRows == false)
            {
                try
                {
					command = new sqliteCommand("INSERT INTO " + PlayersName + 
					                            " (name, password, rIndex) "+ 
					                            "VALUES ($n, $p, $i) ",PlayerDatabase);
                    command.Parameters.Add("$n", DbType.String).Value = tempPlayer.PlayerName;
                    command.Parameters.Add("$p", DbType.String).Value = password;
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

        public bool GetPlayerLogin(ref Player p, String Username, String password)
        {
            
            new sqliteCommand("create table if not exists " + PlayersName + " (name varchar(30), " +
                              "password varchar(150), rIndex int)", PlayerDatabase).ExecuteNonQuery();

            var command = new sqliteCommand("select * from " + PlayersName + " where name = '" + Username + "'", PlayerDatabase);
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
                    return true;
                }
            }
            return false;
        }

        public void WritePlayer(Player p)
        {
            var command = new sqliteCommand(PlayerDatabase);
            command.CommandText ="update " + PlayersName + " set rIndex = :i where name=:id";
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
        }

        public void WriteDungeon(Dungeon d)
        {

           new sqliteCommand("drop table if exists " + DungeonName, DungeonDatabase).ExecuteNonQuery();

            new sqliteCommand("create table "+ DungeonName + " (name varchar(30), " +
                  "description varchar(150), rIndex int , north int , " +
                  "east int, south int, west int)", DungeonDatabase).ExecuteNonQuery();


            List <Room> roomList =  d.GetRoomList();
            foreach (Room r in roomList)
            {
               
                sqliteCommand command;

                try
                {
                    command = new sqliteCommand("INSERT INTO " + DungeonName +
                            "  (name, description, rIndex, north, east, south, west) " +
                            "VALUES (?,?,?,?,?,?,?) ", DungeonDatabase);
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
                var command = new sqliteCommand("select * from " + "dungeonTable", DungeonDatabase);
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

    }
}
