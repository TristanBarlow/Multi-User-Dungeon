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
