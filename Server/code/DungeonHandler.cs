using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Dungeon;
using PlayerN;

namespace Request
{
    class RequestHandler
    {
        DungeonS dungeon;
        PlayerHandler PlayerHandle;

        public RequestHandler(ref DungeonS d, ref PlayerHandler p)
        {
            dungeon = d;
            PlayerHandle = p;
        }

        public String PlayerAction(String action, String clientName)
        {
            lock (dungeon)
            { 
                return dungeon.PlayerAction(action, PlayerHandle.GetPlayerReference(clientName));
            }
        }

        public void AddPlayer(String name)
        {
            lock (PlayerHandle)
            {
                PlayerHandle.AddNewPlayer(name, dungeon.GetRandomRoom());
            }
        }

        public void playerNameChange(String oldName, String newName)
        {
            lock (PlayerHandle)
            {
                PlayerHandle.UpdatePlayerName(oldName, newName);
            }
         }

        public Player GetPlayer(String playerName)
        {
            lock (PlayerHandle)
            {
                return PlayerHandle.GetPlayerReference(playerName);
            }
        }
    }
}
