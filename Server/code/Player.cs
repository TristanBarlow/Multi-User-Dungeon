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
        float health;

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

        public CombatHandler(ref Player Agressor,ref Player Defender)
         {

         }
    }

}
