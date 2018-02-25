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


        public String PlayerAction(String action, String player)
        {
            return dungeon.PlayerAction(action, player);
        }

        public void AddPlayer(String name)
        {
            dungeon.NewClient(name);
        }

    }
}
