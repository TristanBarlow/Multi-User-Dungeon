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

        public PictureBox PB { get; set; }
        public Graphics G { get; set; }

        private SolidBrush SolidBrushG = new SolidBrush(Color.Green);
        private SolidBrush SolidBrushR = new SolidBrush(Color.Red);
        private Pen PenW = new Pen(Color.White, 4F);
        private Pen PenB = new Pen(Color.DarkCyan, 4F);

        public int Walls { get; set; } = 0;

        public int Enemy { get; set; } = 0;

        int Scale = 2;

        int Connectors = 0;

        public Color FillColor { get; set; } = Color.Black;

       private int RoomWidth = 20;
       private int RoomHeight = 20;
       private int RoomGapX = 20;
       private int RoomGapY = 20;
       private int PlayerSize = 8;
       private int EnemySize = 6;

        public delegate void Del();
        private Dictionary<String, Del> MainDrawQueue = new Dictionary<String, Del>();
        private Dictionary<String, Del> LastDrawQueue = new Dictionary<String, Del>();
        private Dictionary<String, Del> DynamicDrawQueue = new Dictionary<String, Del>();

        public DungeonDraw(PictureBox DG)
        {
            Width = DG.Width;
            Height = DG.Height;
            XPos = DG.Location.X;
            YPos = DG.Location.Y;
            PB = DG;
            G = DG.CreateGraphics();
            RoomWidth*= Scale;
            RoomHeight *= Scale;
            RoomGapX = (int)(RoomWidth * 1.5);
            RoomGapY = (int)(RoomHeight * 1.5);

        }

        public void DrawLine(int x, int y, int x2, int y2)
        {
            Walls++;
            MainDrawQueue.Add(""+ Walls, () => G.DrawLine(PenW, x, y, x2, y2));
            G.DrawLine(PenW, x, y, x2, y2);
        }

        public void DrawWall(int SXPos, int SYPos, int EXPos, int EYPos)
        {
            Walls++;
            MainDrawQueue.Add("Wall" + Walls, () => G.DrawLine(PenW, SXPos, SYPos, EXPos, EYPos));
            G.DrawLine(PenW, SXPos, SYPos, EXPos, EYPos);
        }

        public void DrawConnector(int x, int y, bool isHorizontal)
        {
            int xSize = RoomWidth /2 ;
            int ySize = RoomHeight /2;
            x -= RoomWidth / 2;
            y -= RoomHeight / 2;
            if (isHorizontal)
            {
                ySize = ySize / 2;
                x -= xSize;
                y += ySize + ySize/2 ;
            }
            else
            {
                xSize = xSize / 2;
                x += xSize + xSize/2 ;
                y -= ySize;
            }

            Connectors++;
            LastDrawQueue.Add("Connector" + Connectors, () => G.DrawRectangle(PenB, x, y, xSize, ySize));
            G.DrawRectangle(PenB, x, y, xSize, ySize);
        }

        public void DrawRoom(Room r)
        {
            String rN = "Room" + r.RoomNum;
            int tx = r.XPos - RoomWidth / 2;
            int ty = r.YPos - RoomHeight / 2;
            if (!MainDrawQueue.ContainsKey(rN))
            {
                MainDrawQueue.Add(rN, () => G.DrawRectangle(PenW, tx, ty, RoomWidth, RoomHeight));
                G.DrawRectangle(PenW, tx, ty, RoomWidth, RoomHeight);
            }
        }

        private void DrawPlayer(int XPos, int YPos)
        {

            if (DynamicDrawQueue.ContainsKey("player"))
            {   
                DynamicDrawQueue.Remove("player");
                DynamicDrawQueue.Add("player", () => G.FillEllipse(SolidBrushG, XPos, YPos, PlayerSize, PlayerSize));
            }   
            else
            {   
                DynamicDrawQueue.Add("player", () => G.FillEllipse(SolidBrushG, XPos, YPos, PlayerSize, PlayerSize));
            }
        }

        private void DrawEnemy(int XPos, int YPos, String EnemyName  = " ")
        {

            if (DynamicDrawQueue.ContainsKey(EnemyName))
            {
                DynamicDrawQueue.Remove(EnemyName);
                DynamicDrawQueue.Add(EnemyName, () => G.FillEllipse(SolidBrushR, XPos, YPos, EnemySize, EnemySize));
            }

            else
            {
                Enemy++;
                DynamicDrawQueue.Add(EnemyName, () => G.FillEllipse(SolidBrushR, XPos, YPos, EnemySize, EnemySize));
            }
        }

        private void DrawDynamics()
        {
            for (int i = 0; i < this.DynamicDrawQueue.Count(); i++)
            {
                DynamicDrawQueue.ElementAt(i).Value();
            }
        }

        public void Draw()
        {
            G.Clear(FillColor);

            for (int i = 0; i < this.LastDrawQueue.Count(); i++)
            {
                LastDrawQueue.ElementAt(i).Value();
            }
            for (int i = 0; i < this.MainDrawQueue.Count(); i++)
            {
                MainDrawQueue.ElementAt(i).Value();
            }
            DrawDynamics();
        }

        public void AddRoomDraws(List<Room> RoomList)
        {
            RoomList.OrderBy(o => o.RoomNum).ToList();
            int midX = (PB.Width / 2) ;
            int midY = (PB.Height / 2);

            foreach (Room r in RoomList)
            {
                if (r.RoomNum == -1) break;
                if (r.RoomNum == 0)
                {
                    r.XPos = midX;
                    r.YPos = midY;
                    DrawRoom(r);
                    r.isDraw = true;
                }
                if (r.North != -1 && !RoomList.ElementAt(r.North).isDraw)
                {
                    RoomList.ElementAt(r.North).isDraw = true;
                    RoomList.ElementAt(r.North).XPos = r.XPos;
                    RoomList.ElementAt(r.North).YPos = r.YPos - RoomGapY;
                    DrawConnector(r.XPos, r.YPos, false);
                    DrawRoom(RoomList.ElementAt(r.North));
                }
                if (r.East != -1 && !RoomList.ElementAt(r.East).isDraw)
                {
                    RoomList.ElementAt(r.East).isDraw = true;
                    RoomList.ElementAt(r.East).XPos = r.XPos + RoomGapX;
                    RoomList.ElementAt(r.East).YPos = r.YPos;
                    DrawConnector(r.XPos + RoomGapX, r.YPos, true);
                    DrawRoom(RoomList.ElementAt(r.East));
                }
                if (r.South!= -1 && !RoomList.ElementAt(r.South).isDraw)
                {
                    RoomList.ElementAt(r.South).isDraw = true;
                    RoomList.ElementAt(r.South).XPos = r.XPos;
                    RoomList.ElementAt(r.South).YPos = r.YPos + RoomGapY;
                    DrawConnector(r.XPos, r.YPos + RoomGapY, false);
                    DrawRoom(RoomList.ElementAt(r.South));
                }
                if (r.West != -1 && !RoomList.ElementAt(r.West).isDraw)
                {
                    RoomList.ElementAt(r.West).isDraw = true;
                    RoomList.ElementAt(r.West).XPos = r.XPos - RoomGapX;
                    RoomList.ElementAt(r.West).YPos = r.YPos;
                    DrawConnector(r.XPos, r.YPos, true);
                    DrawRoom(RoomList.ElementAt(r.West));
                }


            }
        }

        public void UpdateClient(Enemy e, Room r)
        { }

        public void DrawClients(List<Enemy> enemies, List<Room> roomList, int PlayerRoomNum)
        {
            if (roomList.Count > 0)
            {
                foreach (Room r in roomList)
                {
                    int iter = 0;
                    int xMove = RoomWidth / 3 ;
                    int yMove = RoomHeight / 3;
                    foreach (Enemy e in enemies)
                    {
                        if (e.RoomNum == r.RoomNum)
                        {
                            
                            if (iter * PlayerSize *2 > RoomWidth)
                            {
                                yMove -= PlayerSize;
                                xMove = RoomWidth/3;
                                iter = 0;
                            }
                            DrawEnemy(r.XPos - xMove,  r.YPos - yMove , e.Name);
                            xMove -= PlayerSize;
                            iter++;
                        }
                    }
                }
                DrawPlayer(roomList[PlayerRoomNum].XPos, roomList[PlayerRoomNum].YPos);
                DrawDynamics();
            }   
        }
    }

    public class Enemy
    {
        public int RoomNum { set; get; } = -1;
        public String Name { set; get; } = "No Name";
        public Enemy(String s)
        {
            Name = s;
        }
        public Enemy(String s, int Rn)
        {
            Name = s;
            RoomNum = Rn;
        }
    }

    public class Room
    {
        public int XPos { get; set; } = 0;
        public int YPos { get; set; } = 0;
        public int RoomNum { set; get; } = -1;
        public int North { set; get; } =-1;
        public int East { set; get; } = -1;
        public int South { set; get; } =-1;
        public int West { set; get; } = -1;
        public bool isDraw { set; get; } = false;
        public Room(int rN)
        {
            RoomNum = rN;
        }

    }
}
