using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Dungeon;
using PlayerN;

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

        public void UpdatePlayerName(String oldName, String newName)
        {
            foreach (Player iter in dungeon.GetPlayerList())
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

        public Player GetPlayer(String playerName){ return dungeon.GetPlayerReference(playerName); }

    }
}
