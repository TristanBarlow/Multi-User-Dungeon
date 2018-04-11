using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.ComponentModel;

using Dungeon;

namespace PlayerN
{
    public class Player
    {
        public String PlayerName { set; get;}

        private String Stance = "attack";

        public bool isInCombat;

        public bool isDead;

        public Room currentRoom;

        public Inventory inventory;

        private int health = 100;

        public String GetStance() { return Stance;}

        public void SetStance(String newStance) {Stance = newStance;}

        public Player(String name)
        {
            PlayerName = name;
            isDead = true;
        }

        public int GetHealth(){ return health;}

        public void DropItem(String nItem)
        {
            currentRoom.inventory.AddItem(inventory.TransfereItem(nItem));
        }

        public void ChangeHealth(int x)
        {
            health = health + x;
            if (health <= 0) { isDead = true; }
        }

        public String GetPlayerName() { return PlayerName; }

        public void SetPlayerName(String newName) { PlayerName = newName;}

        public Player(String clientName, Room startRoom)
        {
            PlayerName = clientName;
            currentRoom = startRoom;
            inventory = new Inventory();
            isDead = false;
        }
    }
}
