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
                lock (PlayerHandle)
                {
                    return dungeon.PlayerAction(action, PlayerHandle.GetPlayerReference(clientName));
                }
            }
        }

        public void SetPlayerStance(String stance, String playerName)
        {
            lock(PlayerHandle)
            {
                PlayerHandle.GetPlayerReference(playerName).SetStance(stance);
            }
        }

        public void AddPlayer(String name)
        {
            lock (PlayerHandle)
            {
                lock (dungeon)
                {
                    PlayerHandle.AddNewPlayer(name, dungeon.GetRandomRoom());
                }
            }
        }

        public Room GetPlayerRoom(String playerName)
        {
            Player p;
            lock (PlayerHandle)
            {
                p = PlayerHandle.GetPlayerReference(playerName);
            }
            lock (dungeon)
            {
                return p.currentRoom;
            }
        }

        public void PlayerNameChange(String oldName, String newName)
        {
            lock (PlayerHandle)
            {
                PlayerHandle.UpdatePlayerName(oldName, newName);
            }
         }

        public void RemovePlayer(String PlayerName)
        {
            lock (dungeon)
            {
                lock (PlayerHandle)
                {
                    PlayerHandle.RemovePlayer(PlayerName);
                }
            }
        }

        public Player GetPlayer(String playerName)
        {
            lock (PlayerHandle)
            {
                return PlayerHandle.GetPlayerReference(playerName);
            }
        }

        private int DoAttack(ref Player Agro, ref Player Def)
        {
            String agroStance = Agro.GetStance().ToLower();
            String defStance = Def.GetStance().ToLower();

            int dmg = 0;

            switch (agroStance.ToLower())
            {
                case "attack":
                    if (defStance == "attack") { Agro.ChangeHealth(-10); Def.ChangeHealth(-10); dmg = 10; }
                    if (defStance == "wildattack") { Agro.ChangeHealth(-20); Def.ChangeHealth(-10); dmg = 10; }
                    if (defStance == "defend") { Agro.ChangeHealth(-5); Def.ChangeHealth(0); dmg = 10; }
                    break;

                case "wildattack":
                    if (defStance == "attack") { Agro.ChangeHealth(-10); Def.ChangeHealth(-20); dmg = 20; }
                    if (defStance == "wildattack") { Agro.ChangeHealth(-30); Def.ChangeHealth(-30); dmg = 30; }
                    if (defStance == "defend") { Agro.ChangeHealth(-15); Def.ChangeHealth(-5); dmg = 10; }
                    break;

                case "defend":
                    dmg = 0;
                    break;
            }
            return dmg;
        }

        public String StartFight(String agroName, String defName)
        {
            Player pA;
            Player pD;
            int tempH;
            String returnString = "You did ";

            lock (PlayerHandle)
            {
                 pA =PlayerHandle.GetPlayerReference(agroName);
                 pD =PlayerHandle.GetPlayerReference(defName);
            }
           
            if ((pD != null && pA != null) && (!pD.isDead && !pA.isDead))
            {
                if (pA.currentRoom == pD.currentRoom)
                {
                    int tempHealth = pA.GetHealth();
                    returnString += DoAttack(ref pA, ref pD).ToString() + " damage to " + defName;
                    returnString += " You took " + (tempHealth - pA.GetHealth()).ToString() + " Damage ";
                    returnString += U.NewLineS("") + "Your health is now: " + pA.GetHealth().ToString();
                }
                else
                { returnString = "You are not in the same room, How do you expect to attack them!?"; }
            }
            else
            {
                returnString = "You cannot attack this character they are dead";
            }
            return returnString;
        }
    }
}
