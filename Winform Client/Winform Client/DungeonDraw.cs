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
        private Pen PenW = new Pen(Color.Pink, 2F);

        public int Walls { get; set; } = 0;

        public int Enemy { get; set; } = 0;

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
        }
        public void DrawLine(int SXPos, int SYPos, int EXPos, int EYPos)
        {
            Walls++;
            PointF pt1D = new PointF();
            PointF pt2D = new PointF();
            pt1D.X = SXPos;
            pt1D.Y = SYPos;
            pt2D.X = EXPos;
            pt2D.Y = EYPos;
            DrawDict.Add(""+ Walls, () => G.DrawLine(PenW, pt1D, pt2D));
            Draw();
        }

        public void DrawWall(int SXPos, int SYPos, int EXPos, int EYPos)
        {

            Walls++;
            PointF pt1D = new PointF();
            PointF pt2D = new PointF();
            pt1D.X = SXPos;
            pt1D.Y = SYPos;
            pt2D.X = EXPos;
            pt2D.Y = EYPos;
            DrawDict.Add("Wall" + Walls, () => G.DrawLine(PenW, pt1D, pt2D));
            Draw();
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
            G.Clear(Color.Black);
            for (int i = 0; i < this.DrawDict.Count(); i++)
            {
               DrawDict.ElementAt(i).Value();
            }
        }
    }
}
