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
                case PublicChatMsg.ID:
                    m = new PublicChatMsg();
                    break;

                case PrivateChatMsg.ID:
                    m = new PrivateChatMsg();
                    break;

                case ClientListMsg.ID:
                    m = new ClientListMsg();
                    break;

                case ClientNameMsg.ID:
                    m = new ClientNameMsg();
                    break;

                case DungeonCommand.ID:
                    m = new DungeonCommand();
                    break;

                case DungeonResponse.ID:
                    m = new DungeonResponse();
                    break;

                case HealthMessage.ID:
                    m = new HealthMessage();
                    break;

                case AttackMessage.ID:
                    m = new AttackMessage();
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

    public class PublicChatMsg : Msg
    {
        public const int ID = 1;

        public String msg;

        public PublicChatMsg() { mID = ID; }

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

    public class PrivateChatMsg : Msg
    {
        public const int ID = 2;

        public String msg;

        public String destination;

        public PrivateChatMsg() { mID = ID; }

        public override MemoryStream WriteData()
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter write = new BinaryWriter(stream);
            write.Write(ID);
            write.Write(msg);
            write.Write(destination);

            write.Close();

            return stream;
        }

        public override void ReadData(BinaryReader read)
        {
            msg = read.ReadString();
            destination = read.ReadString();
        }
    };

    public class ClientListMsg : Msg
    {
        public const int ID = 3;

        public List<String> clientList;

        public ClientListMsg() 
        { 
            mID = ID;

            clientList = new List<String>();
        }

        public override MemoryStream WriteData()
        {
            
            MemoryStream stream = new MemoryStream();
            BinaryWriter write = new BinaryWriter(stream);

            write.Write(ID);
            write.Write(clientList.Count);
            foreach (String s in clientList)
            {
                write.Write(s);
            }

            write.Close();

            return stream;
        }

        public override void ReadData(BinaryReader read)
        {
            int count = read.ReadInt32();

            clientList.Clear();

            for (int i = 0; i < count; i++)
            {
                clientList.Add(read.ReadString());
            }
        }
    };

    public class ClientNameMsg : Msg
    {
        public const int ID = 4;

        public String name;

        public ClientNameMsg() { mID = ID; }

        public override MemoryStream WriteData()
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter write = new BinaryWriter(stream);
            write.Write(ID);
            write.Write(name);

            write.Close();

            return stream;
        }

        public override void ReadData(BinaryReader read)
        {
            name = read.ReadString();
        }
    };

    public class DungeonCommand : Msg
    {
        public const int ID = 5;

        public String command;

        public String getCommand()
       {
            return command;
        }
 
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
        public const int ID = 6;

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

    public class HealthMessage : Msg
    {
        public const int ID = 7;

        public int health;

        public HealthMessage() { mID = ID; }

        public override MemoryStream WriteData()
        {
            String healthString = "h" + health;
            MemoryStream stream = new MemoryStream();
            BinaryWriter write = new BinaryWriter(stream);
            write.Write(ID);
            write.Write(healthString);

            Console.Write("sending: " + healthString);

            write.Close();
            return stream;
        }

        public override void ReadData(BinaryReader read)
        {
           String temp = read.ReadString();
           var index = temp.Split('h');

           if (int.TryParse(index[1], out health)) { }
           else { health = 1; }
        }
    }

    public class AttackMessage : Msg
    {
        public const int ID = 8;

        public String action;

        public String opponent;

        public AttackMessage() { mID = ID; }

        public override MemoryStream WriteData()
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter write = new BinaryWriter(stream);
            write.Write(ID);
            write.Write(action);
            write.Write(opponent);

            Console.Write("sending: " + action + opponent);

            write.Close();
            return stream;
        }

        public override void ReadData(BinaryReader read)
        {
            action = read.ReadString();
            opponent = read.ReadString();
        }
    }
}