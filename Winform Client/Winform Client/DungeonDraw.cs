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
        private Pen PenB = new Pen(Color.Blue, 4F);

        public int Walls { get; set; } = 0;

        public int Enemy { get; set; } = 0;

        int Scale = 2;

        int Connectors = 0;

        public Color FillColor { get; set; } = Color.Black;

       private int RoomWidth = 20;
       private int RoomHeight = 20;
       private int RoomGapX = 20;
       private int RoomGapY = 20;

        public delegate void Del();
        public Dictionary<String, Del> DrawDict { get; set; } = new Dictionary<String, Del>();

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
            DrawDict.Add(""+ Walls, () => G.DrawLine(PenW, x, y, x2, y2));
            G.DrawLine(PenW, x, y, x2, y2);
        }

        public void DrawWall(int SXPos, int SYPos, int EXPos, int EYPos)
        {
            Walls++;
            DrawDict.Add("Wall" + Walls, () => G.DrawLine(PenW, SXPos, SYPos, EXPos, EYPos));
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
            DrawDict.Add("Connector" + Connectors, () => G.DrawRectangle(PenB, x, y, xSize, ySize));
            G.DrawRectangle(PenB, x, y, xSize, ySize);
        }

        public void DrawRoom(Room r)
        {
            String rN = "Room" + r.RoomNum;
            int tx = r.XPos - RoomWidth / 2;
            int ty = r.YPos - RoomHeight / 2;
            if (!DrawDict.ContainsKey(rN))
            {
                DrawDict.Add(rN, () => G.DrawRectangle(PenW, tx, ty, RoomWidth, RoomHeight));
                G.DrawRectangle(PenW, tx, ty, RoomWidth, RoomHeight);
            }
        }

        public void DrawPlayer(int XPos, int YPos, int Radius)
        {
            SolidBrush t = new SolidBrush(Color.Green);

            if (DrawDict.ContainsKey("player"))
            {
                DrawDict.Remove("player");
                DrawDict.Add("player", () => G.FillEllipse(SolidBrushG, XPos, YPos, Radius, Radius));
            }
            else
            {
                DrawDict.Add("player", () => G.FillEllipse(SolidBrushG, XPos, YPos, Radius, Radius));
            }
            Draw();
        }

        public void DrawEnemy(int XPos, int YPos, int Radius, String EnemyName  = " ")
        {

            if (DrawDict.ContainsKey(EnemyName))
            {
                DrawDict.Remove(EnemyName);
                DrawDict.Add(EnemyName, () => G.FillEllipse(SolidBrushR, XPos, YPos, Radius, Radius));
            }

            else
            {
                Enemy++;
                DrawDict.Add(EnemyName, () => G.FillEllipse(SolidBrushR, XPos, YPos, Radius, Radius));
            }
            Draw();
        }

        public void Draw()
        {
            G.Clear(FillColor);
            for (int i = 0; i < this.DrawDict.Count(); i++)
            {
               DrawDict.ElementAt(i).Value();
            }
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
