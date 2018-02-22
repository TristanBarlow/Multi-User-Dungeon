using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungeon
{
    public class Room
    {
        public Room(String name, String desc)
        {
            this.desc = desc;
            this.name = name;
        }

        public String North
        {
            get { return exits[0]; }
            set { exits[0] = value; }
        }

        public String South
        {
            get { return exits[1]; }
            set { exits[1] = value; }
        }

        public String East
        {
            get { return exits[2]; }
            set { exits[2] = value; }
        }
        public String West
        {
            get { return exits[3]; }
            set { exits[3] = value; }
        }



        public String name = "";
        public String desc = "";
        public String[] exits = new String[4];
        public String[] users;
        public static String[] exitNames = { "NORTH", "SOUTH", "EAST", "WEST" };

    }

}
