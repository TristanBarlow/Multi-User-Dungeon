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
        String playerName;

        private String Stance = "attack";

        public bool isInCombat;

        public bool isDead;

        public Room currentRoom;

        public Inventory inventory;

        private int health = 100;

        public String GetStance() { return Stance;}

        public void SetStance(String newStance) {Stance = newStance;}

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

       public String GetPlayerName() { return playerName; }

        public void SetPlayerName(String newName) { playerName = newName; }

       public Player(String clientName, Room startRoom)
        {
            playerName = clientName;
            currentRoom = startRoom;
            inventory = new Inventory();
        }
    }

    public class PlayerHandler
    {
        private static List<Player> playerList = new List<Player>();

        public List<Player> GetPlayerList()
        {
            return playerList;
        }

        public Player AddNewPlayer(String clientName, Room randomRoom)
        {
            Player newPlayer = new Player(clientName, randomRoom);
            randomRoom.AddPlayer(newPlayer);
            playerList.Add(newPlayer);
            return newPlayer;
        }

        public Player GetPlayerReference(String PlayerName)
        {
            lock (playerList)
            {
                foreach (Player player in playerList)
                {
                    if (player.GetPlayerName() == PlayerName)
                    {
                        return player;
                    }
                }
                return null;
            }
        }

        public void UpdatePlayerName(String oldName, String newName)
        {
            lock (playerList)
            {
                foreach (Player iter in playerList)
                {
                    if (iter.GetPlayerName() == oldName)
                    {
                        iter.SetPlayerName(newName);
                        break;
                    }
                    else
                    {
                        // check
                    }
                }
            }
        }

    }


}
