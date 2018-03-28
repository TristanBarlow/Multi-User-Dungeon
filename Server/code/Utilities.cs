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
            int opposite = 0;
            if (direction == 0) opposite = 2;
            if (direction == 1) opposite = 3;
            if (direction == 2) opposite = 0;
            if (direction == 3) opposite = 1;

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
        
        public static List<Room> GenerateDungeon(int size, int spread)
        {
            if (size < 4)
            {
                size = 4;
            }
            size += 1;
           //s size = (int)((((size) /4) * 4)+1);
            List<Room> rRooms = new List<Room>();
            Random rand = new Random();
            int iter = 0;
            while(rRooms.Count < size)
            {
                rRooms.Add(new Room("Room" + iter, iter));
                iter++;
            }
            //north 0
            //east 1
            //south 2
            //west 3

            int RoomLength = (int)(size / 4)-1;

            int i = 1;

            MakeConnections(ref i, RoomLength, 0,ref rRooms);
            MakeConnections(ref i, RoomLength, 1, ref rRooms);
            MakeConnections(ref i, RoomLength, 2, ref rRooms);
            MakeConnections(ref i, RoomLength, 3, ref rRooms);

            Console.Write(U.GenerateDungeonString(rRooms));
            //rRooms[0].AddConection(1, i);
            //max = i + RoomLength;
            //while (i < max)
            //{
            //    rRooms[i].AddConection(3, i - 1);
            //    rRooms[i].AddConection(1, i + 1);
            //    i++;
            //}
            //rRooms[i].AddConection(3, i - 1);
            int foob = 0;

            //int currentRoom = 0;
            //rRooms[0].AddConection(0, 1);
            //rRooms[1].AddConection(2, 0);
            //currentRoom++;
            //iter = currentRoom;
            //while (currentRoom < iter + RoomLength)
            //{
            //    rRooms[currentRoom].AddConection(2, currentRoom - 1);
            //    rRooms[currentRoom].AddConection(0, currentRoom + 1);
            //    currentRoom++;
            //}
            //rRooms[0].AddConection(1, currentRoom);
            //rRooms[currentRoom].AddConection(3, 0);
            //currentRoom++;
            //iter = currentRoom;
            //while (currentRoom < iter + RoomLength)
            //{
            //    rRooms[currentRoom].AddConection(3, currentRoom - 1);
            //    rRooms[currentRoom].AddConection(1, currentRoom + 1);
            //    currentRoom++;
            //}
            //rRooms[0].AddConection(2, currentRoom);
            //rRooms[currentRoom].AddConection(0, 0);
            //currentRoom++;
            //iter = currentRoom;
            //while (currentRoom < iter + RoomLength)

            //{
            //    rRooms[currentRoom].AddConection(0, currentRoom - 1);
            //    rRooms[currentRoom].AddConection(2, currentRoom + 1);
            //    currentRoom++;
            //}
            //rRooms[0].AddConection(3, currentRoom);
            //rRooms[currentRoom].AddConection(1, 0);
            //currentRoom++;
            //iter = currentRoom;
            //while (currentRoom < iter + RoomLength)
            //{
            //    rRooms[currentRoom].AddConection(1, currentRoom - 1);
            //    rRooms[currentRoom].AddConection(3, currentRoom + 1);
            //    currentRoom++;
            //}

            return rRooms;
        }

        public static bool GetRandomChance(Random r, int probability)
        {
            return r.Next(0, 100) < probability;
        }
    }
}
