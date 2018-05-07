using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    public enum ActionID {SAY, MOVE, NORMAL, UPDATE };

    /**
     *A class to represent any actions that went on whilst insde the dungeon 
     */
    public class ActionResponse
    {
        public ActionID ID = ActionID.NORMAL; 
        public string message = "";
        public void Set(String str, ActionID id)
        {
            ID = id;
            message = str;
        }
    }

}
