using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.ComponentModel;

using DungeonNamespace;
using System.Net.Sockets;

namespace PlayerN
{
    /**
     *This class is only realy used to make it easier to pass around who we're dealing with
     * The only game stuff it stores is name and the room index the player is in
     * Also stores the socket. I prefer it in here as it just easier and quicker to access 
     */
    public class Player
    {
        public String PlayerName { set; get;}

        public String Salt { set; get; }

        public int RoomIndex { set; get; }

        public void SetRoom(int r)
        {
            RoomIndex = r;
        }

        public Socket Socket { set; get; }

        /**
         * Constructor used for when a client has logged in successfuly
         */
        public Player(String clientName, Socket s)
        {
            PlayerName = clientName;
            Socket = s;
            RoomIndex = -1;
        }

        /**
         *Players are created but if they exit before loggin in they are not always used. 
         */
        public Player(String name)
        {
            PlayerName = name;
            RoomIndex = -1;
        }

        /**
        *Players are created but if they exit before loggin in they are not always used. 
        */
        public Player(Socket s)
        {
            Socket = s;
            Salt = "";
            RoomIndex = -1;
        }

    }
}
