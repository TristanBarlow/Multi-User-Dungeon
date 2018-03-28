using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeon;

namespace Utilities
{
    public static class U
    {
        public static String NewLineS(String s)
        {
            String newline = "\r\n";
            String finalString = s + newline;
            return finalString;
        }
        public static String GenerateDungeonString(List<Room> RoomList)
        {
            String rStr = "&";
            foreach (Room r in RoomList)
            {
                bool GoodRoom = false;
                int[] temp = r.GetExitIndexs();
                if (temp[0] != -1)
                {
                    rStr += "n" + temp[0] + " ";
                    GoodRoom = true;
                }
                if (temp[1] != -1)
                {
                    rStr += "e" + temp[1] + " ";
                    GoodRoom = true;
                }
                if (temp[2] != -1)
                {
                    rStr += "s" + temp[2] + " ";
                    GoodRoom = true;
                }
                if (temp[3] != -1)
                {
                    rStr += "w" + temp[3] + " ";
                    GoodRoom = true;
                }
                if (GoodRoom) rStr += "&";
            }

            return rStr;
        }

        public static void MakeConnections(ref int i, int RoomLength, int direction, ref List<Room> rRooms)
        {
            int opposite = -1*direction;
            rRooms[0].AddConection(direction, i);
            rRooms[i].AddConection(opposite, 0);
            int max = i + RoomLength;
            while (i < max)
            {
                int ti = i - 1;
                rRooms[i].AddConection(opposite,ti );
                rRooms[i].AddConection(direction, i + 1);
                i++;
            }
            rRooms[i].AddConection(opposite, i - 1);
            i++;
        }

        public static void MakeBranch(ref int i, int BranchStart, int NumberOfRooms, int direction, ref List<Room>rRooms)
        {
            int opposite = -1 * direction;
            if(rRooms[BranchStart].AddConection(direction, i))
            {
                rRooms[i].AddConection(opposite, BranchStart);
                i++;
            }
        }
        
        public static List<Room> GenerateDungeon(int size, int spread)
        {
            if (size < 4)
            {
                size = 4;
            }
            size += 1;
            List<Room> rRooms = new List<Room>();
            Random rand = new Random();
            int iter = 0;
            while(rRooms.Count < size)
            {
                rRooms.Add(new Room("Room" + iter, iter));
                iter++;
            }
            int RoomLength = (int)(size / spread)-1;

            int i = 1;

            MakeConnections(ref i, RoomLength, 1,ref rRooms);
            MakeConnections(ref i, RoomLength, -1, ref rRooms);
            MakeConnections(ref i, RoomLength, 2, ref rRooms);
            MakeConnections(ref i, RoomLength, -2, ref rRooms);

            int RoomsLeftOver = rRooms.Count - i;
            int[] Dir = { 1, -1, -2, 2 };

            while(i < rRooms.Count)
            {
                MakeBranch(ref i, rand.Next(0, i), 0,Dir[rand.Next(0,3)] ,ref rRooms);
            }

           // Console.Write(U.GenerateDungeonString(rRooms));
            return rRooms;
        }

        public static bool GetRandomChance(Random r, int probability)
        {
            return r.Next(0, 100) < probability;
        }
    }
}
