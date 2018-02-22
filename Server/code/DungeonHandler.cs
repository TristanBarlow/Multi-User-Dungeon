using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Dungeon;

namespace DungeonHandler
{
    class DungeonHandlerS
    {
        DungeonS dungeon;

        public DungeonHandlerS(ref DungeonS d)
        {
            dungeon = d;
        }


        public String playerAction(String action, String player)
        {
            return dungeon.playerAction(action, player);
        }

        public void addPlayer(String name)
        {
            dungeon.newClient(name);
        }

    }
}
