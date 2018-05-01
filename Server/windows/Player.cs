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

        private bool moved = false;

        private bool hasSpoken = false;
        private String say = "&";

        public int roomIndex = -1;

        public Inventory inventory;

        private int health = 100;

        public String GetStance() { return Stance;}

        public Room GetRoom() { return currentRoom; }
        public void SetRoom(Room r)
        {
            roomIndex = r.RoomIndex;
            currentRoom = r;
            moved = true;
        }

        public bool GetHasMoved()
        {
            if (moved)
            {
                moved = false;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void SetHasSpoken(String speech)
        {
            say = speech;
            hasSpoken = true;
        }
        public bool GetHasSpoken()
        {
            if (hasSpoken)
            {
                hasSpoken = false;
                return true;
            }
            else
            {
                return false;
            }
        }

        public Socket socket { set; get; }

        public void SetStance(String newStance) {Stance = newStance;}
        public Inventory GetInventory() { return inventory; }

        public Player(String clientName, Socket s)
        {
            PlayerName = clientName;
            socket = s;
            inventory = new Inventory();
        }

        public Player(String name)
        {
            PlayerName = name;
            inventory = new Inventory();
        }

        public Player(Socket s)
        {
            socket = s;
            inventory = new Inventory();
        }

        public int GetHealth(){ return health;}

        public void DropItem(String nItem)
        {
            currentRoom.GetInventory().AddItem(inventory.TransfereItem(nItem));
        }

        public void ChangeHealth(int x)
        {
            health = health + x;
            if (health <= 0) { isDead = true; }
        }

        public String GetPlayerName() { return PlayerName; }

        public void SetPlayerName(String newName) { PlayerName = newName;}


    }
}
