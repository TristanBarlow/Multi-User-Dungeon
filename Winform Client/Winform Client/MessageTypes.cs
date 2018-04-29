using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MessageTypes
{
    public abstract class Msg
    {
        public Msg() { mID = 0; }

        public int mID;

        public abstract MemoryStream WriteData();

        public abstract void ReadData(BinaryReader read);

        public static Msg DecodeStream(BinaryReader read)
        {
            int id;
            Msg m = null;

            id = read.ReadInt32();

            switch (id)
            {
                case RoomMessage.ID:
                    m = new RoomMessage();
                    break;
                case LoginMessage.ID:
                    m = new LoginMessage();
                    break;

                case DungeonCommand.ID:
                    m = new DungeonCommand();
                    break;

                case DungeonResponse.ID:
                    m = new DungeonResponse();
                    break;

                case MapLayout.ID:
                    m = new MapLayout();
                    break;

                case PlayerLocations.ID:
                    m = new PlayerLocations();
                    break;
                case CreateUser.ID:
                    m = new CreateUser();
                    break;

                default:
                    throw (new Exception());
            }

            if (m != null)
            {
                m.mID = id;
                m.ReadData(read);
            }

            return m;
        }
    };

    public class RoomMessage : Msg
    {
        public const int ID = 1;

        public String msg;

        public RoomMessage() { mID = ID; }

        public override MemoryStream WriteData()
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter write = new BinaryWriter(stream);

            write.Write(ID);
            write.Write(msg);

            write.Close();

            return stream;
        }

        public override void ReadData(BinaryReader read)
        {
            msg = read.ReadString();
        }
    };

    public class LoginMessage : Msg
    {
        public const int ID = 2;

        public String name;
        private int nameLength = 0;

        public String password;
        private int passLength = 0;

        public LoginMessage() { mID = ID; }

        public void SetPassword(String p)
        {
            passLength = p.Length;
            password = p;
        }

        public void SetName(String n)
        {
            nameLength = n.Length;
            name = n;
        }
        public override MemoryStream WriteData()
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter write = new BinaryWriter(stream);
            write.Write(ID);
            write.Write(nameLength);
            write.Write(name);
            write.Write(passLength);
            write.Write(password);

            write.Close();

            return stream;
        }

        public override void ReadData(BinaryReader read)
        {
            nameLength = read.ReadInt32();
            name = read.ReadString();
            passLength = read.ReadInt32();
            password = read.ReadString();

        }
    };

    public class CreateUser : Msg
    {
        public const int ID = 7;

        public String name;
        private int nameLength = 0;

        public String password;
        private int passLength = 0;

        public CreateUser() { mID = ID; }

        public void SetPassword(String p)
        {
            passLength = p.Length;
            password = p;
        }

        public void SetName(String n)
        {
            nameLength = n.Length;
            name = n;
        }
        public override MemoryStream WriteData()
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter write = new BinaryWriter(stream);
            write.Write(ID);
            write.Write(nameLength);
            write.Write(name);
            write.Write(passLength);
            write.Write(password);

            write.Close();

            return stream;
        }

        public override void ReadData(BinaryReader read)
        {
            nameLength = read.ReadInt32();
            name = read.ReadString();
            passLength = read.ReadInt32();
            password = read.ReadString();

        }
    }

    public class DungeonCommand : Msg
    {
        public const int ID = 3;

        public String command;

        public DungeonCommand() { mID = ID; }

        public override MemoryStream WriteData()
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter write = new BinaryWriter(stream);
            write.Write(ID);
            write.Write(command);

            Console.Write(command);

            write.Close();

            return stream;
        }

        public override void ReadData(BinaryReader read)
        {
            command = read.ReadString();
        }
    }

    public class DungeonResponse : Msg
    {
        public const int ID = 4;

        public String response;

        public DungeonResponse() { mID = ID; }

        public override MemoryStream WriteData()
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter write = new BinaryWriter(stream);
            write.Write(ID);
            write.Write(response);

            Console.Write("sending: " + response);

            write.Close();
            return stream;
        }

        public override void ReadData(BinaryReader read)
        {
            response = read.ReadString();
        }
    }

    public class MapLayout : Msg
    {
        public const int ID = 5;

        public String mapInfo;

        public MapLayout() { mID = ID; }

        public override MemoryStream WriteData()
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter write = new BinaryWriter(stream);
            write.Write(ID);
            write.Write(mapInfo);

            Console.Write("sending: " + mapInfo);

            write.Close();
            return stream;
        }

        public override void ReadData(BinaryReader read)
        {
            mapInfo = read.ReadString();
        }
    }

    public class PlayerLocations : Msg
    {
        public const int ID = 6;

        public String LocationString = " ";

        public PlayerLocations() { mID = ID; }

        public override MemoryStream WriteData()
        {

            MemoryStream stream = new MemoryStream();
            BinaryWriter write = new BinaryWriter(stream);
            write.Write(ID);
            write.Write(LocationString);

            write.Close();
            return stream;
        }

        public override void ReadData(BinaryReader read)
        {
            LocationString = read.ReadString();
        }
    }
}