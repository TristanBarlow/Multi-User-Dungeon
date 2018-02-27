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

        private List<Item> itemList;

        private int health = 100;

        public List<Item> GetItemList() { if (itemList.Count() > 0) return itemList; else return null; }

        public void AddItemToList(Item nItem) { itemList.Add(nItem); }

        public String GetStance() { return Stance;}

        public void SetStance(String newStance) {Stance = newStance;}

        public int GetHealth(){ return health;}

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
            itemList = new List<Item>();
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
