using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MessageTypes;
using Utilities;

namespace Dungeon
{
    public class Room
    {
        public Room()
        {
            graffitiList = new List<string>();
        }

        public Room(String name, String desc)
        {
            this.desc = desc;
            this.name = name;
            graffitiList = new List<string>();
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

        public void addGraf(String graff)
        {
            graffitiList.Add(graff);
        }

        public String getDescription()
        {
            String returnString;
            returnString = U.newLineS(name) +
                           U.newLineS(desc)+
                           U.newLineS("Exits Are:");

            if (exits.Count() != 0)
            {
                for (var i = 0; i < exits.Length; i++)
                {
                    if (exits[i] != null)
                    {
                        returnString += Room.exitNames[i] + " ";
                    }
                }
                returnString += U.newLineS("//");
            }
            else
            {
                returnString = "no exits oh no";
            }

            returnString += U.newLineS("Graffiti: ");
            if (graffitiList.Count() != 0)
            {
                foreach (String iter in graffitiList)
                {
                    returnString += iter;
                }
            }
            else
            {
                returnString += U.newLineS("no Graffiti Be the first!!!");
            }

            return returnString;
        }

        public String getGraff()
        {
            String returnString = "";
            if (graffitiList.Count() != 0)
            {
                foreach (String graff in graffitiList)
                {
                    returnString += U.newLineS(graff);
                }

                return returnString;
            }
            else
            {
                return U.newLineS("no Graffiti");
            }
        }

        public String name = "";
        private String desc = "";
        public String[] exits = new String[4];
        public String[] users;
        public List<String> graffitiList;
        public static String[] exitNames = { "NORTH", "SOUTH", "EAST", "WEST" };

    }

}
