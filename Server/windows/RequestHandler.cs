using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using DungeonNamespace;
using PlayerN;
using Utilities;


namespace Request
{
    class RequestHandler
    {
        Dungeon dungeon;

        public RequestHandler(ref Dungeon d)
        {
            dungeon = d;
        }

        public String PlayerAction(String action, Player p)
        {
            return dungeon.PlayerAction(action,p );
        }

        public void SetPlayerRandomRoom(Player p)
        { 
                    p.currentRoom = dungeon.GetRandomRoom();
        }
        public Room GetPlayerRandomRoom()
        {
             return dungeon.GetRandomRoom();
        }
    }
}
