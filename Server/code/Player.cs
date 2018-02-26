using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeon;

namespace PlayerN
{
    public class Player
    {
        String playerName;

        public Room currentRoom;

        private int health = 100;

        public int GetHealth(){ return health;}

        public void ChangeHealth(int x) { health = health + x; }

       public String GetPlayerName() { return playerName; }

        public void SetPlayerName(String newName) { playerName = newName; }

       public Player(String clientName, Room startRoom)
        {
            playerName = clientName;
            currentRoom = startRoom;
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

    public class CombatHandler
    {
        Player Agro;
        Player Def;

        public CombatHandler(ref Player Agressor,ref Player Defender)
         {
            Agro = Agressor;
            Def = Defender;
         }

        public Player combatLoop()
        {
            Player Winner = Agro; 

            return Winner;
        }

    }


}
