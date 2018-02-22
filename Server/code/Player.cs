using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Dungeon
{
    class Player
    {
        String playerName;
        public Room currentRoom;
        float health;

       public Player(String clientName, Room startRoom)
        {
            playerName = clientName;
            currentRoom = startRoom;
        }
    }
}
