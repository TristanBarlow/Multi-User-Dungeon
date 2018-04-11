using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Dungeon;
using PlayerN;
using Utilities;


namespace Request
{
    class RequestHandler
    {
        DungeonS dungeon;

        public RequestHandler(ref DungeonS d)
        {
            dungeon = d;
        }

        public String PlayerAction(String action, Player p)
        {
            lock (dungeon)
            {
                    return dungeon.PlayerAction(action,p );
            }
        }

        public void SetPlayerRandomRoom(Player p)
        { 
                lock (dungeon)
                {
                    p.currentRoom = dungeon.GetRandomRoom();
                }
        }
        public Room GetPlayerRandomRoom()
        {
            lock (dungeon)
            {
                return dungeon.GetRandomRoom();
            }
        }
    }
}
