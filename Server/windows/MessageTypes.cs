using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Server;

namespace MessageTypes
{
    /**
     *Abstract class from which all messages sent over the network derive from. 
     */
    public abstract class Msg
    {
        public Msg() { mID = 0; }

        //ID of the message
        public int mID;

        //if you set this to true, I think its similar to how a packet sniffer would work
        //It will grab the buffer sent over the network and try and read it with a binary reader
        // I think my encryption works, because if you do try this it will error :).
        private static bool attemptPacketsniff = false;

        /**
         *This will write the data to a stream, If the data is of a type that should be encrypted it will use
         * the salt to encrypt the data.
         * @param salt the salt to be used IF the message type should be encrypted.
         */
        public abstract MemoryStream WriteData(String salt);

        /**
         *This will simply use the binary reader to extract the data from the stream. 
         */
        public abstract void ReadData(BinaryReader read);

        /**
         *The main function for convertting the buffer received over the network back into the message types we know
         * @param buffer the buffer to be converted into a message type
         * @param salt the unique salt to be used for decrypting
         * @param IsEncrypted if this is true it will try and decrypt the buffer recieved.
         */
        public static Msg DecodeStream(byte[] buffer, String salt, bool IsEncrypted)
        {
            BinaryReader read = null;
            MemoryStream stream = null;


            if (IsEncrypted)
            {
                //decrypt the buffer
                stream = new MemoryStream(Encryption.Decrypt(buffer, salt));

                //if you wana try and packet sniff(dont)
                if (attemptPacketsniff)
                {
                    MemoryStream packetSniffer = new MemoryStream(buffer);
                    BinaryReader packetReader = new BinaryReader(packetSniffer);
                    int i = packetReader.ReadInt32();
                    String s = packetReader.ReadString();
                }

            }
            else
            {
                //normal data, not decryption needed.
                 stream = new MemoryStream(buffer);
            }
            read = new BinaryReader(stream);
            int id;
            Msg m = null;

            id = read.ReadInt32();

            switch (id)
            {
                case LoginResponse.ID:
                    m = new LoginResponse();
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
                case UpdateChat.ID:
                    m = new UpdateChat();
                    break;

                case SaltMessage.ID:
                    m = new SaltMessage();
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

    public class LoginResponse : Msg
    {
        public const int ID = 1;

        private int breakInt = 0;

        public String message;

        public String loggedIn;

        public LoginResponse() { mID = ID; }

        public override MemoryStream WriteData(String salt)
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter write = new BinaryWriter(stream);
            write.Write(ID);
            write.Write(message);
            write.Write(breakInt);
            write.Write(loggedIn);

            write.Close();

            return stream;
        }

        public override void ReadData(BinaryReader read)
        {
            message = read.ReadString();
            breakInt = read.ReadInt32();
            loggedIn = read.ReadString();

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

        public void SetPassword(String p, String salt)
        {
            passLength = p.Length;
            password = Encryption.GenerateSaltedHash(p, salt);
        }

        public void SetName(String n)
        {
            nameLength = n.Length;
            name = n;
        }

        public override MemoryStream WriteData(String salt)
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter write = new BinaryWriter(stream );
            write.Write(ID);
            write.Write(nameLength);
            write.Write(name);
            write.Write(passLength);
            write.Write(password);

            write.Close();

            MemoryStream ms = new MemoryStream(Encryption.Encrypt(stream.ToArray(), salt));

            return ms;
        }

        public override void ReadData(BinaryReader read)
        {
            nameLength = read.ReadInt32();
            name = read.ReadString();
            passLength = read.ReadInt32();
            password = read.ReadString();

        }
    };

    public class DungeonCommand : Msg
    {
        public const int ID = 3;

        public String command;
 
        public DungeonCommand() { mID = ID; }

        public override MemoryStream WriteData(String salt)
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter write = new BinaryWriter(stream );
            write.Write(ID);
            write.Write(command);

            write.Close();

            MemoryStream ms = new MemoryStream(Encryption.Encrypt(stream.ToArray(), salt));

            return ms;
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

        public override MemoryStream WriteData(String salt)
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter write = new BinaryWriter(stream );
            write.Write(ID);
            write.Write(response);

            write.Close();
            MemoryStream ms = new MemoryStream(Encryption.Encrypt(stream.ToArray(), salt));

            return ms;
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

        public override MemoryStream WriteData(String salt)
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter write = new BinaryWriter(stream );
            write.Write(ID);
            write.Write(mapInfo);

            write.Close();

            MemoryStream ms = new MemoryStream(Encryption.Encrypt(stream.ToArray(), salt));

            return ms;
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

        public override MemoryStream WriteData(String salt)
        {

            MemoryStream stream = new MemoryStream();
            BinaryWriter write = new BinaryWriter(stream );
            write.Write(ID);
            write.Write(LocationString);

            write.Close();
            MemoryStream ms = new MemoryStream(Encryption.Encrypt(stream.ToArray(), salt));

            return ms;
        }

        public override void ReadData(BinaryReader read)
        {
            LocationString = read.ReadString();
        }
    }

    public class CreateUser : Msg
    {
        public const int ID = 7;

        public String name;
        private int nameLength = 0;

        public String password;
        private int passLength = 0;

        public String salt;
        private int saltLength = 0;

        public CreateUser() { mID = ID; }

        public void SetPassword(String p, String s)
        {
            passLength = p.Length;
            salt = s;
            saltLength = salt.Length;
            password = Encryption.GenerateSaltedHash(p,salt);
        }

        public void SetName(String n)
        {
            nameLength = n.Length;
            name = n;
        }
        public override MemoryStream WriteData(String s)
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter write = new BinaryWriter(stream );
            write.Write(ID);
            write.Write(nameLength);
            write.Write(name);
            write.Write(passLength);
            write.Write(password);
            write.Write(saltLength);
            write.Write(salt);

            write.Close();

            return stream;
        }

        public override void ReadData(BinaryReader read)
        {
            nameLength = read.ReadInt32();
            name = read.ReadString();
            passLength = read.ReadInt32();
            password = read.ReadString();
            saltLength = read.ReadInt32();
            salt = read.ReadString();

        }
    }

    public class UpdateChat:Msg
    {
        public const int ID = 8;

        public String message = "";
        public UpdateChat() { mID = ID; }

        public override MemoryStream WriteData(String salt)
        {

            MemoryStream stream = new MemoryStream();
            BinaryWriter write = new BinaryWriter(stream );
            write.Write(ID);
            write.Write(message);

            write.Close();

            MemoryStream ms = new MemoryStream(Encryption.Encrypt(stream.ToArray(), salt));

            return ms;
        }

        public override void ReadData(BinaryReader read)
        {
            message = read.ReadString();
        }
    }

    public class SaltMessage : Msg
    {
        public const int ID = 9;

        public String message = "";
        public SaltMessage() { mID = ID; }

        public override MemoryStream WriteData(String salt)
        {

            MemoryStream stream = new MemoryStream();
            BinaryWriter write = new BinaryWriter(stream );
            write.Write(ID);
            write.Write(message);

            write.Close();
            return stream;
        }

        public override void ReadData(BinaryReader read)
        {
            message = read.ReadString();
        }
    }

}