using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Winform_Client
{
    public class DungeonDraw
    {
        public int Height { get; set; }
        public int Width { get; set; }
        public int YPos { get; set; }
        public int XPos { get; set; }
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


        private int Scale = 1;
        public void ChangeScale(int Offset)
        {
            Scale += Offset;
            if (Scale <= 0) Scale = 1;
            if (Scale >= 20) Scale = 19;
            UpdateScale();
            
        }

        public PictureBox PB { get; set; }
        public Graphics G { get; set; }
        public Graphics GU { get; set; }

        public int Enemy { get; set; } = 0;

        public Color FillColor { get; set; } = Color.Black;

        private List<Room> currentMap = new List<Room>();
        private List<DrawObject> MapObjects = new List<DrawObject>();
        private Dictionary<String, User> UserDrawDict = new Dictionary<String, User>();
        private String CurrentMapString = " ";
        private String CurrentPlayerLocations = " ";
        public List<int> ClientNumberList = new List<int>();

        private SolidBrush b = new SolidBrush(Color.Purple);
        private SolidBrush g = new SolidBrush(Color.Green);

        public int Rooms()
        {
            return currentMap.Count();
        }

        public DungeonDraw(PictureBox DG)
        {
            Width = DG.Width;
            Height = DG.Height;
            XPos = DG.Location.X;
            YPos = DG.Location.Y;
            PB = DG;
            G = DG.CreateGraphics();
            GU = DG.CreateGraphics();
        }

        public void Draw()
        {
            
            lock (MapObjects)
            {
                G.Clear(FillColor);
                foreach (DrawObject d in MapObjects)
                {
                    d.DrawMe(G, XOffset, YOffset);
                }
            }
            UpdateUserPositions();
        }

        public void DrawUsers()
        {
                lock (UserDrawDict)
                {
                    lock (GU)
                    {
                        foreach (KeyValuePair<String, User> u in UserDrawDict)
                        {
                            u.Value.size = Scale*3;
                            RoomSlot rs = currentMap[u.Value.RoomNum].GetNextRoomSlot(u.Value.size);
                            u.Value.DrawMe(GU, XOffset, YOffset);
                        }
                    }
                }
        }

        public void UpdateScale()
        { 
            currentMap.Clear();
            MapObjects.Clear();
            String temp =CurrentMapString;
            CurrentMapString = "";
            MapParser(temp);
            Draw();
        }

        public void UpdateUserPositions()
        {
            lock (currentMap)
            {
                foreach (Room r in currentMap)
                {
                    r.FreeAllSlots();
                }
                foreach (int i in ClientNumberList)
                {
                    RoomSlot rs = currentMap[i].GetNextRoomSlot(3 * Scale);
                    if (rs != null)
                    {
                        GU.FillEllipse(b, rs.XPos + XOffset, rs.YPos + YOffset, 3 * Scale, 3 * Scale);
                    }
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
                    MapObjects.Add(new Connector(r, false));
                }
                if (r.East != -1 && !currentMap.ElementAt(r.East).isDraw)
                {
                    currentMap.ElementAt(r.East).isDraw = true;
                    currentMap.ElementAt(r.East).XPos = r.XPos + r.RoomGapX;
                    currentMap.ElementAt(r.East).YPos = r.YPos;
                    MapObjects.Add(new Connector(currentMap.ElementAt(r.East), true));
                }
                if (r.South != -1 && !currentMap.ElementAt(r.South).isDraw)
                {
                    currentMap.ElementAt(r.South).isDraw = true;
                    currentMap.ElementAt(r.South).XPos = r.XPos;
                    currentMap.ElementAt(r.South).YPos = r.YPos + r.RoomGapY;
                    MapObjects.Add(new Connector(currentMap.ElementAt(r.South), false));
                }
                if (r.West != -1 && !currentMap.ElementAt(r.West).isDraw)
                {
                    currentMap.ElementAt(r.West).isDraw = true;
                    currentMap.ElementAt(r.West).XPos = r.XPos - r.RoomGapX;
                    currentMap.ElementAt(r.West).YPos = r.YPos;
                    MapObjects.Add(new Connector(r, true));
                }
            }
            Draw();
        }

        public void AddClientDraw(String s, int RoomNum)
        {

            if (UserDrawDict.ContainsKey(s))
            {
                return;
            }
            User tu = new User(s,RoomNum,Scale);
            RoomSlot rs = currentMap[RoomNum].GetNextRoomSlot(tu.size);
            tu.MoveUser(rs,RoomNum);
            UserDrawDict.Add(s, tu);
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
                Room r = new Room(iter, Scale);
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
                                    r.North = Int32.Parse(w.Remove(0,1));
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

        public void DrawPlayers(String str)
        {
            if (str == CurrentPlayerLocations)
            {
                return;
            }
            CurrentPlayerLocations = str;
            String[]words =  CurrentPlayerLocations.Split('&');
            foreach(String w in words)
            {
                String[]s =  w.Split(' ');
                if (s.Count() >= 2)
                {
                    int i = Int32.Parse(s[1]);
                    lock (UserDrawDict)
                    {
                        if (!UserDrawDict.ContainsKey(s[0]))
                        {
                            User u = new User(s[0], i, Scale);
                            RoomSlot rs = currentMap[i].GetNextRoomSlot(u.size);
                            u.XPos = rs.XPos; u.YPos = rs.YPos;
                            UserDrawDict.Add(s[0], u);
                        }
                        else
                        {
                            User u = UserDrawDict[s[0]];
                            if (u.RoomNum != i)
                            {

                                currentMap[u.RoomNum].FreeRoomSlot(u.RoomSlotIndex);
                                RoomSlot rs = currentMap[i].GetNextRoomSlot(u.size);
                                UserDrawDict[s[0]].MoveUser(rs, i);
                            }

                        }
                    }
                }
                    
            }
            Draw();
        }
    }
 }


    public class User: DrawObject
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
        public User(String s, int Rn, int scale = 3, bool IsPlayer = false)
        {
            Name = s;
            RoomNum = Rn;
            if (IsPlayer)
            {
                b = new SolidBrush(Color.Green);
            }
             size = scale;
        }
        public void MoveUser(RoomSlot rs, int num)
        {
            XPos = rs.XPos;
            YPos = rs.YPos;
            RoomSlotIndex = rs.IndexNumber;
            RoomNum = num;
        }
        public override void DrawMe(Graphics G,int XOff, int YOff)
        {
            G.FillEllipse(b, XPos+XOff, YPos+YOff, size, size);
            base.DrawMe(G, XOff, YOff);
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
            int index = RoomSlotList.Count/2;
            int iter = 0;
            bool flipFlop = true;
            while(index >= 0 && index < RoomSlotList.Count)
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
            if (RoomSlotList.Count >= IndexNumber&& IndexNumber >=0)
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

        public Room(int rN, int Scale)
        {
            RoomNum = rN;
            RoomHeight *= Scale;
            RoomWidth *= Scale;
            XEnemySpawn = RoomHeight / 3;
            YEnemySpawn = RoomWidth / 3;
            RoomGapX = (int)(RoomWidth * 1.5);
            RoomGapY = (int)(RoomHeight * 1.5);
            Margin*= Scale;
        }
        private void MakeRoomSpaces(int PlayerSize)
        {
            int xOffset = XPos + Margin - RoomWidth/2;
            int yOffset = YPos + Margin - RoomHeight/2;
            int maxXOffset = XPos + RoomWidth/2 - (Margin * 2) ;
            int maxYOffset = YPos + RoomHeight/2 - (Margin * 2);

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

        public override void DrawMe(Graphics G,int XOff, int YOff)
        {
            int tx = this.XPos - RoomWidth / 2;
            int ty = this.YPos - RoomHeight / 2;
            G.DrawRectangle(PenW, tx+XOff, ty+YOff, RoomWidth, RoomHeight);
            base.DrawMe(G,XOff,YOff);
        }
    }

    public class Connector : DrawObject
    {
        private bool isHorizontal = false;
        private int xSize = 0;
        private int ySize = 0;
        private Pen PenB = new Pen(Color.DarkCyan, 4F);

        public Connector(Room r, bool H)
        {
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
        public override void DrawMe(Graphics G,int XOff, int YOff)
        {
            G.DrawRectangle(PenB, XPos+XOff, YPos+YOff, xSize, ySize);
            base.DrawMe(G, XOff,YOff);

        }
    }

    public abstract class DrawObject
    {
        public int XPos { get; set; } = 0;
        public int YPos { get; set; } = 0;
        public virtual void DrawMe(Graphics G, int XOff, int YOff) { }
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
