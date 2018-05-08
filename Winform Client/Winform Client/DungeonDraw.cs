using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Threading;

namespace Winform_Client
{
    /**
     *This handles everything to do with drawing the dungeon. It was fun to make, probably
     * could be more effecient. 
     */
    public class DungeonDraw
    {
        Random rand = new Random();

        //Picture box size
        public int Height { get; set; }
        public int Width { get; set; }

        //Move map, currently not used
        private int XOffset = 0;
        public void MoveX(int x)
        {
            if (x + XOffset < 2000 && XOffset - x > -2000)
            {
                XOffset += x;
            }
            Draw();
        }
        private int YOffset = 0;
        public void MoveY(int y)
        {
            if (y + XOffset < 2000 && XOffset - y > -2000)
            {
                YOffset += y;
            }
            Draw();
        }

        //zoom
        private int Scale = 7;

        //the picture box to draw to
        public PictureBox PB { get; set; }

        //Graphics used for the drawing
        public Graphics G { get; set; }

        //The background colour
        public Color FillColor { get; set; } = Color.Black;

        //The user object that is assosiated with the local client
        private User LocalClient = null;

        public bool IsInUse = false;

        //draw objects
        private List<Room> currentMap = new List<Room>();
        private List<DrawObject> MapObjects = new List<DrawObject>();
        private Dictionary<String, User> ClientDrawDict = new Dictionary<String, User>();

        //Current map stringed parsed
        private String CurrentMapString = " ";


        /**
         *Default constructor for the dungeon draw class
         * @param DG the picture box to draw all the lovely graphics too
         */
        public DungeonDraw(PictureBox DG)
        {
            Width = DG.Width;
            Height = DG.Height;
            PB = DG;
            G = DG.CreateGraphics();
        }

        /**
         * When called it will add an offset to the current dungeon scale
         * @param Offset the amount to change the zoom by
         */
        public void ChangeScale(int Offset)
        {
            Scale += Offset;
            //clamp
            if (Scale <= 2) Scale = 2;
            if (Scale >= 10) Scale = 10;

            //redraw
            UpdateScale();

        }

        /**
         * Draws evertyhing on screen
         */
        public void Draw()
        {
            G.Clear(FillColor);
            DrawClientPositions();
            lock (MapObjects)
            {

                foreach (DrawObject d in MapObjects)
                {
                    d.DrawMe(G, XOffset, YOffset);
                }

            }
        }

        private void UpdateScale()
        {
            currentMap.Clear();
            MapObjects.Clear();
            String temp = CurrentMapString;
            CurrentMapString = "";
            MapParser(temp);
            Draw();
        }

        private void DrawClientPositions()
        {
            foreach (Room r in currentMap)
            {
                r.FreeAllSlots();
            }
            if (LocalClient != null)
            {
                RoomSlot prs = currentMap[LocalClient.RoomNum].GetNextRoomSlot(3 * Scale);
                LocalClient.MoveUser(prs, Scale);
                XOffset = -LocalClient.XPos + Width / 2;
                YOffset = -LocalClient.YPos + Height / 2;
                LocalClient.DrawMe(G, XOffset, YOffset);
            }
            foreach (KeyValuePair<String, User> u in ClientDrawDict)
            {
                RoomSlot rs = currentMap[u.Value.RoomNum].GetNextRoomSlot(3 * Scale);
                if (rs != null && u.Key != LocalClient.Name)
                {
                    u.Value.MoveUser(rs, Scale);
                    u.Value.DrawMe(G, XOffset, YOffset);
                }
            }
        }

        public void AddConnectorDraws()
        {
            currentMap.OrderBy(o => o.RoomNum).ToList();
            int midX = (PB.Width / 2);
            int midY = (PB.Height / 2);

            foreach (Room r in currentMap)
            {
                if (r.RoomNum == -1) break;
                if (r.RoomNum == 0)
                {
                    r.XPos = midX;
                    r.YPos = midY;
                    r.isDraw = true;
                }
                if (r.North != -1 && !currentMap.ElementAt(r.North).isDraw)
                {
                    currentMap.ElementAt(r.North).isDraw = true;
                    currentMap.ElementAt(r.North).XPos = r.XPos;
                    currentMap.ElementAt(r.North).YPos = r.YPos - r.RoomGapY;
                    MapObjects.Add(new Connector(r, false, rand));
                }
                if (r.East != -1 && !currentMap.ElementAt(r.East).isDraw)
                {
                    currentMap.ElementAt(r.East).isDraw = true;
                    currentMap.ElementAt(r.East).XPos = r.XPos + r.RoomGapX;
                    currentMap.ElementAt(r.East).YPos = r.YPos;
                    MapObjects.Add(new Connector(currentMap.ElementAt(r.East), true, rand));
                }
                if (r.South != -1 && !currentMap.ElementAt(r.South).isDraw)
                {
                    currentMap.ElementAt(r.South).isDraw = true;
                    currentMap.ElementAt(r.South).XPos = r.XPos;
                    currentMap.ElementAt(r.South).YPos = r.YPos + r.RoomGapY;
                    MapObjects.Add(new Connector(currentMap.ElementAt(r.South), false, rand));
                }
                if (r.West != -1 && !currentMap.ElementAt(r.West).isDraw)
                {
                    currentMap.ElementAt(r.West).isDraw = true;
                    currentMap.ElementAt(r.West).XPos = r.XPos - r.RoomGapX;
                    currentMap.ElementAt(r.West).YPos = r.YPos;
                    MapObjects.Add(new Connector(r, true, rand));
                }
            }
            Draw();
        }

        public void MapParser(String str)
        {
            if (str == CurrentMapString)
            {
                return;
            }
            CurrentMapString = str;
            currentMap.Clear();
            MapObjects.Clear();
            String[] words = str.Split('&');
            int iter = 0;
            foreach (String room in words)
            {
                Room r = new Room(iter, Scale, rand);
                bool GoodRoom = false;

                String[] exits = room.Split(' ');
                foreach (String w in exits)
                {
                    if (w.Length > 0)
                    {
                        for (int i = 0; i < w.Length; i++)
                        {
                            switch (w[i].ToString().ToLower())
                            {
                                case "n":
                                    r.North = Int32.Parse(w.Remove(0, 1));
                                    GoodRoom = true;
                                    break;
                                case "e":
                                    r.East = Int32.Parse(w.Remove(0, 1));
                                    GoodRoom = true;
                                    break;
                                case "s":
                                    r.South = Int32.Parse(w.Remove(0, 1));
                                    GoodRoom = true;
                                    break;
                                case "w":
                                    r.West = Int32.Parse(w.Remove(0, 1));
                                    GoodRoom = true;
                                    break;
                            }
                        }
                    }
                }
                if (GoodRoom)
                {
                    currentMap.Add(r);
                    MapObjects.Add(r);
                    iter++;
                }
            }
            AddConnectorDraws();
        }

        public void AddClientsDraw(String str, String PlayerName)
        {
            String[] ClientLocations = str.Split('&');
            foreach (String Client in ClientLocations)
            {
                if (Client != "")
                {
                    int Room = Int32.Parse(Client.Split(' ')[1]);
                    String ClientName = Client.Split(' ')[0];

                    if (ClientDrawDict.ContainsKey(ClientName))
                    {
                        if (ClientDrawDict[ClientName].RoomNum != Room) ClientDrawDict[ClientName].RoomNum = Room;
                    }
                    else
                    {
                        User u;
                        if (ClientName == PlayerName)
                        {
                            u = new User(ClientName, Room, rand, Scale, true);
                            LocalClient = u;
                        }
                        else
                        {
                            u = new User(ClientName, Room, rand, Scale);
                        }
                        ClientDrawDict.Add(ClientName, u);
                    }
                }
            }
            Draw();
        }
    }



    public abstract class DrawObject
    {
        public int XPos { get; set; } = 0;
        public int YPos { get; set; } = 0;
        public virtual void DrawMe(Graphics G, int XOff, int YOff) { }
    }

    public class User : DrawObject
    {
        public int RoomNum { set; get; } = -1;
        public int RoomSlotIndex { set; get; } = -1;
        public String Name { set; get; } = "No Name";
        public int size { set; get; } = 3;
        private SolidBrush b = new SolidBrush(Color.Purple);
        public User(String s)
        {
            Name = s;
        }
        public User(String s, int Rn, Random r, int scale = 3, bool IsPlayer = false)
        {
            Name = s;
            RoomNum = Rn;
            if (IsPlayer)
            {
                b = new SolidBrush(Color.Green);
            }
            else
            {
                b.Color = Color.FromArgb(r.Next(30, 255), r.Next(30, 255), r.Next(30, 255));
            }
            size = scale;
        }
        public void MoveUser(RoomSlot rs, int Scale)
        {
            XPos = rs.XPos;
            YPos = rs.YPos;
            size = Scale * 3;
            RoomSlotIndex = rs.IndexNumber;
        }


        public override void DrawMe(Graphics G, int XOff, int YOff)
        {
            if (XPos + XOff + size > 0 && YPos + YOff + size > 0)
            {
                G.FillEllipse(b, XPos + XOff, YPos + YOff, size, size);
                base.DrawMe(G, XOff, YOff);
            }
        }
        public void DrawMeExact(Graphics G, int XLocation, int YLocation, int Scale)
        {
            if (XLocation + size > 0 && YLocation + size > 0)
            {
                G.FillEllipse(b, XLocation, YLocation, Scale, Scale);
                base.DrawMe(G, XLocation, YLocation);
            }
        }
    }

    public class Room : DrawObject
    {
        private Pen PenW = new Pen(Color.White, 4F);
        private List<RoomSlot> RoomSlotList = new List<RoomSlot>();
        private List<int> InUseSlots = new List<int>();
        private int Margin = 2;
        private bool HasSlots = false;
        public int RoomWidth { set; get; } = 20;
        public int RoomHeight { set; get; } = 20;
        public int RoomGapX { set; get; } = 20;
        public int RoomGapY { set; get; } = 20;
        public int XEnemySpawn { set; get; } = 0;
        public int YEnemySpawn { set; get; } = 0;
        public int MaxNumClients { get; set; } = 0;
        public int RoomNum { set; get; } = -1;
        public int North { set; get; } = -1;
        public int East { set; get; } = -1;
        public int South { set; get; } = -1;
        public int West { set; get; } = -1;
        public bool isDraw { set; get; } = false;

        public RoomSlot GetNextRoomSlot(int PlayerSize)
        {
            if (!HasSlots)
            {
                MakeRoomSpaces(PlayerSize);
            }
            int index = RoomSlotList.Count / 2;
            int iter = 0;
            bool flipFlop = true;
            while (index >= 0 && index < RoomSlotList.Count)
            {
                RoomSlot rs = RoomSlotList[index];
                if (!rs.InUse)
                {
                    rs.IndexNumber = index;
                    InUseSlots.Add(index);
                    rs.InUse = true;
                    return rs;
                }
                index = RoomSlotList.Count / 2;
                if (flipFlop)
                {
                    index += iter;
                    flipFlop = false;
                }
                else
                {
                    index -= iter;
                    iter++;
                    flipFlop = true;
                }

            }
            return null;
        }

        public void FreeRoomSlot(int IndexNumber)
        {
            if (RoomSlotList.Count >= IndexNumber && IndexNumber >= 0)
            {
                RoomSlotList[IndexNumber].InUse = false;
            }
        }

        public void FreeAllSlots()
        {
            if (HasSlots)
            {
                foreach (int i in InUseSlots)
                {
                    RoomSlotList[i].InUse = false;
                }
                InUseSlots.Clear();
            }
        }

        public Room(int rN, int Scale, Random r)
        {

            RoomNum = rN;
            RoomHeight *= Scale;
            RoomWidth *= Scale;
            XEnemySpawn = RoomHeight / 3;
            YEnemySpawn = RoomWidth / 3;
            PenW.Color = Color.FromArgb(r.Next(30, 255), r.Next(30, 255), r.Next(30, 255));
            RoomGapX = (int)(RoomWidth * 1.5);
            RoomGapY = (int)(RoomHeight * 1.5);
            Margin *= Scale;
        }

        private void MakeRoomSpaces(int PlayerSize)
        {
            int xOffset = XPos + Margin - RoomWidth / 2;
            int yOffset = YPos + Margin - RoomHeight / 2;
            int maxXOffset = XPos + RoomWidth / 2 - (Margin * 2);
            int maxYOffset = YPos + RoomHeight / 2 - (Margin * 2);

            while (yOffset <= maxYOffset)
            {
                xOffset = XPos + Margin - RoomWidth / 2;
                while (xOffset <= maxXOffset)
                {
                    RoomSlotList.Add(new RoomSlot(xOffset, yOffset));
                    xOffset += PlayerSize + Margin;
                }
                yOffset += PlayerSize + Margin;
            }
            MaxNumClients++;
            HasSlots = true;
        }

        public override void DrawMe(Graphics G, int XOff, int YOff)
        {
            int tx = this.XPos - RoomWidth / 2;
            int ty = this.YPos - RoomHeight / 2;
            if (tx + RoomWidth + XOff > 0 && ty + RoomHeight + YOff > 0)
            {
                G.DrawRectangle(PenW, tx + XOff, ty + YOff, RoomWidth, RoomHeight);
                base.DrawMe(G, XOff, YOff);
            }
        }
    }


    public class Connector : DrawObject
    {
        private bool isHorizontal = false;
        private int xSize = 0;
        private int ySize = 0;
        private Pen PenB = new Pen(Color.DarkCyan, 4F);

        public Connector(Room r, bool H, Random rand)
        {
            PenB.Color = Color.FromArgb(rand.Next(30, 255), rand.Next(30, 255), rand.Next(30, 255));
            isHorizontal = H;
            XPos = r.XPos;
            YPos = r.YPos;
            xSize = r.RoomWidth / 2;
            ySize = r.RoomHeight / 2;
            XPos -= r.RoomWidth / 2;
            YPos -= r.RoomHeight / 2;
            if (isHorizontal)
            {
                ySize = ySize / 2;
                XPos -= xSize;
                YPos += ySize + ySize / 2;
            }
            else
            {
                xSize = xSize / 2;
                XPos += xSize + xSize / 2;
                YPos -= ySize;
            }
        }

        public override void DrawMe(Graphics G, int XOff, int YOff)
        {
            if (XPos + XOff + xSize > 0 && YPos + YOff + ySize > 0)
            {
                G.DrawRectangle(PenB, XPos + XOff, YPos + YOff, xSize, ySize);
            }
            base.DrawMe(G, XOff, YOff);

        }
    }

    public class RoomSlot
    {
        public int XPos { get; set; } = 0;
        public int YPos { get; set; } = 0;
        public int IndexNumber { get; set; } = 0;
        public bool InUse { get; set; } = false;
        public RoomSlot(int x, int y)
        {
            XPos = x;
            YPos = y;
        }
    }
}
