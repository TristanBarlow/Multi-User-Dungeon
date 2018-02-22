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

        public String getPlayerName() { return playerName; }
       public Player(String clientName, Room startRoom)
        {
            playerName = clientName;
            currentRoom = startRoom;
        }
    }
}
