using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DungeonNamespace;
using System.Data.SQLite;
using PlayerN;

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
        SQLiteConnection DungeonDatabase = null;
        SQLiteConnection PlayerDatabase = null;

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

            var command = new sqliteCommand("select * from " + PlayersName + " where name == '" + tempPlayer.PlayerName+ "'", PlayerDatabase);
            var reader = command.ExecuteReader();

            if (reader.HasRows == false)
            {
                try
                {
                    var sqlString = "insert into " + PlayersName + " (name, password, rIndex) values ";
                    sqlString += "('" + tempPlayer.PlayerName + "' ";
                    sqlString += ", ";
                    sqlString += "'" + password + "' ";
                    sqlString += ", ";
                    sqlString += "'" + tempPlayer.currentRoom.RoomIndex + "' ";
                    sqlString += ")";
                    command = new sqliteCommand(sqlString, PlayerDatabase);
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

        public bool GetPlayerLogin()
        {
            return false;
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
                    var sqlString = "insert into " + DungeonName + " (name, description, rIndex, north, east, south, west) values ";
                    sqlString += "('" + r.name + "' ";
                    sqlString += ", ";
                    sqlString += "'" + r.desc + "' ";
                    sqlString += ", ";
                    sqlString += "'" + r.RoomIndex + "' ";
                    sqlString += ", ";
                    sqlString += "'" + r.north + "' ";
                    sqlString += ", ";
                    sqlString += "'" + r.east + "' ";
                    sqlString += ", ";
                    sqlString += "'" + r.south + "' ";
                    sqlString += ", ";
                    sqlString += "'" + r.west + "' ";
                    sqlString += ")";

                    command = new sqliteCommand(sqlString, DungeonDatabase);
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
