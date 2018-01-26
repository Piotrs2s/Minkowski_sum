using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Minkowski_sum
{
    public partial class Form1 : Form
    {
        Graphics Graphics;
        Random rand;
        Brush Redbrush;
        Brush Blackbrush;
        Pen pen;
        List<PointF[]> Obstacles;

        public Form1()
        {
            InitializeComponent();
            Graphics = pictureBox1.CreateGraphics();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Graphics.Clear(Color.White);
            rand = new Random();
            Redbrush = new SolidBrush(Color.Red);
            Blackbrush = new SolidBrush(Color.Black);
            pen = new Pen(Color.Black);
            Obstacles = new List<PointF[]>();

            GenerateObstacles();
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Minkowski();
        }

        private void GenerateObstacles()
        {
            //Normal
            int quantity = Int32.Parse(textBox1.Text);  
            //int quantity = rand.Next(2, 6);
            int size = Int32.Parse(textBox2.Text); ;

            for (int i = 0; i < quantity; i++)
            {
                PointF[] Obstacle = new PointF[4];
                PointF a = new PointF(rand.Next(0, pictureBox1.Size.Width-size), rand.Next(0, pictureBox1.Size.Height-size));
                PointF b = new PointF(a.X + size, a.Y);
                PointF c = new PointF(a.X + size, a.Y + size);
                PointF d = new PointF(a.X, a.Y + size);

                Obstacle[0] = a;
                Obstacle[1] = b;
                Obstacle[2] = c;
                Obstacle[3] = d;

                Obstacles.Add(Obstacle);

                Rectangle Obst = new Rectangle(Convert.ToInt32(a.X), Convert.ToInt32(a.Y), size, size);
                Graphics.FillRectangle(Redbrush, Obst);
            }
        }

        private void Minkowski()
        {
            List<PointF>[] ObstaclesShellPoints = new List<PointF>[Obstacles.Count];


            for (int i = 0; i < Obstacles.Count; i++)
            {
                /*ObstaclesShellPoints[i] =*/ GeneratePoints(Obstacles[i]);
            }
            #region  
            //Draw Points
            //for (int i = 0; i < ObstaclesShellPoints.Length; i++)
            //{
            //    foreach (var point in ObstaclesShellPoints[i])
            //    {
            //        Graphics.FillEllipse(Blackbrush, point.X, point.Y, 2, 2);
            //    }
            //}
            #endregion
        }

        private/* List<PointF>*/ void GeneratePoints(PointF[] obstacle)
        {
            List<PointF> ShellPoints = new List<PointF>();
            List<PointF> ShellPointsToDelete = new List<PointF>();

            //Triangle Coordinates
            PointF t1 = new PointF(-3, 3);
            PointF t2 = new PointF(3, 3);
            PointF t3 = new PointF(0, -6);

            //good triangle
            //PointF t1 = new PointF(-1, 1.29f);
            //PointF t2 = new PointF(1, 1.29f);
            //PointF t3 = new PointF(0, -2.58f);


            for (int i = 0; i < obstacle.Length; i++)
            {
                //ShellPoints for each vertice
                PointF Sp1 = new PointF(obstacle[i].X + t1.X, obstacle[i].Y + t1.Y);
                PointF Sp2 = new PointF(obstacle[i].X + t2.X, obstacle[i].Y + t2.Y);
                PointF Sp3 = new PointF(obstacle[i].X + t3.X, obstacle[i].Y + t3.Y);

                switch (i)
                {
                    case 0:
                        ShellPoints.Add(Sp1);
                        ShellPoints.Add(Sp3);
                        ShellPoints.Add(Sp2);

                        break;

                    case 1:
                        ShellPoints.Add(Sp3);
                        ShellPoints.Add(Sp2);
                        ShellPoints.Add(Sp1);
                        break;

                    case 2:
                        ShellPoints.Add(Sp3);
                        ShellPoints.Add(Sp2);
                        ShellPoints.Add(Sp1);
                        break;

                    case 3:
                        ShellPoints.Add(Sp2);
                        ShellPoints.Add(Sp1);
                        ShellPoints.Add(Sp3);
                        break;
                }
            }

            foreach (var point in ShellPoints)
            {

                if (CheckPointIsPart(point, obstacle))
                {
                    ShellPointsToDelete.Add(point);
                }
              
                else if (CheckPointIsCrossing(point, obstacle))
                {
                    ShellPointsToDelete.Add(point);
                }

            }

            foreach (var point in ShellPointsToDelete)
            {
                for (int i = 0; i < ShellPoints.Count; i++)
                {
                    if (point == ShellPoints[i])
                    {
                        ShellPoints.Remove(ShellPoints[i]);
                    }
                }
            }

            //Draw Lines
            PointF[] PointsToDraw = ShellPoints.ToArray();

            Graphics.DrawLines(pen, PointsToDraw);
            Graphics.DrawLine(pen, PointsToDraw[0], PointsToDraw[PointsToDraw.Length-1]);

            //return ShellPoints;
        }

        private bool CheckPointIsPart(PointF point, PointF[] obstacle)
        {
            if (IsPart(point, obstacle[0], obstacle[1]) == 1 || IsPart(point, obstacle[1], obstacle[2]) == 1 || IsPart(point, obstacle[2], obstacle[3]) == 1
                || IsPart(point, obstacle[3], obstacle[0]) == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool CheckPointIsCrossing(PointF point, PointF[] obstacle)
        {
            PointF helpPoint = new PointF(point.X + 300, point.Y);
            int c = 0;

            if (IsCrossing(obstacle[0], obstacle[1], point, helpPoint))
            {
                c++;
            }
            if (IsCrossing(obstacle[1], obstacle[2], point, helpPoint))
            {
                c++;
            }
            if (IsCrossing(obstacle[2], obstacle[3], point, helpPoint))
            {
                c++;
            }
            if (IsCrossing(obstacle[3], obstacle[0], point, helpPoint))
            {
                c++;
            }

            if (c % 2 != 0)
            {
                return true;

            }
            else
            {
                return false;
            }
        }

      
        
        private int IsPart(PointF a, PointF b, PointF c)
        {
            float det = Det(a,b,c);

            if (det ==0 )
            {
                if ((Math.Min(c.X, b.X) <= a.X) && (a.X <= Math.Max(c.X, b.X)) &&
                (Math.Min(c.Y, b.Y) <= a.Y) && (a.Y <= Math.Max(c.Y, b.Y)))
                {
                    return 1;
                }
            }
            return 0;
        }

       
        private bool IsCrossing(PointF a, PointF b, PointF p, PointF r)
        {
          


            return (Math.Sign(Det(p, r, a)) != Math.Sign(Det(p, r, b))) &&
                   (Math.Sign(Det(a, b, p)) != Math.Sign(Det(a, b, r)));
        }

        private static float Det(PointF a, PointF b, PointF c)
        {
            return (a.X * c.Y + b.Y * c.X + b.X * a.Y - c.X * a.Y - a.X * b.Y - b.X * c.Y);
        }

    }
}
