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
    public class Player
    {
        public String PlayerName { set; get;}

        private String Stance = "attack";

        public bool isInCombat;

        public bool isDead = false;

        private Room currentRoom;

        public int roomIndex = -1;

        private int health = 100;

        public String GetStance() { return Stance;}

        public Room GetRoom() { return currentRoom; }

        public void SetRoom(Room r)
        {
            roomIndex = r.RoomIndex;
            currentRoom = r;
        }

        public Socket socket { set; get; }

        public void SetStance(String newStance) {Stance = newStance;}

        public Player(String clientName, Socket s)
        {
            PlayerName = clientName;
            socket = s;
        }

        public Player(String name)
        {
            PlayerName = name;
        }

        public Player(Socket s)
        {
            socket = s;
        }

        public int GetHealth(){ return health;}

        public void ChangeHealth(int x)
        {
            health = health + x;
            if (health <= 0) { isDead = true; }
        }

        public String GetPlayerName() { return PlayerName; }

        public void SetPlayerName(String newName) { PlayerName = newName;}

        public Player CopyPlayer()
        {
            Player p = new Player(PlayerName);
            p.roomIndex = roomIndex;
            return p;
        }
    }
}
