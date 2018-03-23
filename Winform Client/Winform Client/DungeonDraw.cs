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
        private Pen PenB = new Pen(Color.Black, 4F);

        public int Walls { get; set; } = 0;

        public int Enemy { get; set; } = 0;

        int Scale = 4 ;

        int Connectors = 0;

        public Color FillColor { get; set; } = Color.Black;

        int roomWidth = 20;
        int roomHeight = 20;

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
            roomWidth*= Scale;
            roomHeight *= Scale;
 
        }

        public void DrawLine(int x, int y, int x2, int y2)
        {
            Walls++;
            DrawDict.Add(""+ Walls, () => G.DrawLine(PenW, x, y, x2, y2));
            Draw();
        }

        public void DrawWall(int SXPos, int SYPos, int EXPos, int EYPos)
        {
            Walls++;
            DrawDict.Add("Wall" + Walls, () => G.DrawLine(PenW, SXPos, SYPos, EXPos, EYPos));
            Draw();
        }

        public void DrawConnector(int x, int y, bool isHorizontal)
        {
            int xSize = roomWidth/2;
            int ySize = roomHeight/2;
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
            Draw();
        }

        public void DrawRoom(int x, int y, String RoomName)
        {
            if (!DrawDict.ContainsKey(RoomName))
            {
                DrawDict.Add(RoomName, () => G.DrawRectangle(PenW, x, y, roomWidth, roomHeight));
                Draw();
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
        public int RoomNum { set; get; } = -1;
        public int North { set; get; } =-1;
        public int East { set; get; } = -1;
        public int South { set; get; } =-1;
        public int West { set; get; } = -1;
        public Room(int rN)
        {
            RoomNum = rN;
        }

    }
}
