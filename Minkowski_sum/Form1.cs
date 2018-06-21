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

        //Obstacle color
        Brush Redbrush;

        //Lines pen
        Pen BlackPen;

        //List of obstacles vertices points
        List<PointF[]> Obstacles;

        public Form1()
        {
            InitializeComponent();
            Graphics = pictureBox1.CreateGraphics();

        }

        ////Generte obstacles button
        private void obstaclesButton_Click(object sender, EventArgs e)
        {
            Graphics.Clear(Color.White);
            rand = new Random();
            Redbrush = new SolidBrush(Color.Red);
            BlackPen = new Pen(Color.Black);
            Obstacles = new List<PointF[]>();

            GenerateObstacles();
        }


        private void generateButton_Click(object sender, EventArgs e)
        {
            Minkowski();
        }

        private void GenerateObstacles()
        {

            //Check user input
            int retNum;
            if (string.IsNullOrEmpty(quantityTextBox.Text) || string.IsNullOrEmpty(sizeTextBox.Text))
            {
                MessageBox.Show("No input","Error");
                return;
            }
            if (!int.TryParse(quantityTextBox.Text, out retNum) || !int.TryParse(sizeTextBox.Text, out retNum))
            {
                MessageBox.Show("Wrong input format", "Error");
                return;
            }

            //Quantity of obstacles
            int quantity = Int32.Parse(quantityTextBox.Text);  

            //Size of obstacles
            int size = Int32.Parse(sizeTextBox.Text); ;

            //Create obstacle in random place
            for (int i = 0; i < quantity; i++)
            {
                // a, b, c, d - obstacle vertices
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

                //Draw obstacle
                Rectangle Obst = new Rectangle(Convert.ToInt32(a.X), Convert.ToInt32(a.Y), size, size);
                Graphics.FillRectangle(Redbrush, Obst);
            }
        }


        
        //Generate shells for obstacles using Minkowski addition
        private void Minkowski()
        {
            List<PointF>[] ObstaclesShellPoints = new List<PointF>[Obstacles.Count];


            for (int i = 0; i < Obstacles.Count; i++)
            {
                GeneratePoints(Obstacles[i]);
            }
            
        }

        private void GeneratePoints(PointF[] obstacle)
        {

            List<PointF> ShellPoints = new List<PointF>();
            List<PointF> ShellPointsToDelete = new List<PointF>();

            //Start (Robot) Coordinates 
            PointF t1 = new PointF(-3, 3);
            PointF t2 = new PointF(3, 3);
            PointF t3 = new PointF(0, -6);
            

            for (int i = 0; i < obstacle.Length; i++)
            {
                // create shell points for each vertice of obstacle
                PointF Sp1 = new PointF(obstacle[i].X + t1.X, obstacle[i].Y + t1.Y);
                PointF Sp2 = new PointF(obstacle[i].X + t2.X, obstacle[i].Y + t2.Y);
                PointF Sp3 = new PointF(obstacle[i].X + t3.X, obstacle[i].Y + t3.Y);

                //Add shell points in correct order (for drawing)
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

            //Find points of shell that are overlap obstacle or aren't part of the shell 
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

            //Detele points of shell that are overlap obstacle or aren't part of the shell (they are not external)
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

            //Draw shells
            PointF[] PointsToDraw = ShellPoints.ToArray();

            Graphics.DrawLines(BlackPen, PointsToDraw);
            Graphics.DrawLine(BlackPen, PointsToDraw[0], PointsToDraw[PointsToDraw.Length-1]);

            //return ShellPoints;
        }

        //Check if point belongs to shell 
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

        //Check if point overlap figure
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

      
        //Check if point a is between points b and c
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

       //Check if lines a-b and p-r are crossing
        private bool IsCrossing(PointF a, PointF b, PointF p, PointF r)
        {         
            return (Math.Sign(Det(p, r, a)) != Math.Sign(Det(p, r, b))) &&
                   (Math.Sign(Det(a, b, p)) != Math.Sign(Det(a, b, r)));
        }

        //Determinant of matrix
        private static float Det(PointF a, PointF b, PointF c)
        {
            return (a.X * c.Y + b.Y * c.X + b.X * a.Y - c.X * a.Y - a.X * b.Y - b.X * c.Y);
        }

       
    }
}
