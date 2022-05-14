using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace IntelligentScissors
{
    public partial class MainForm : Form
    {
        RGBPixel[,] ImageMatrix;
        static List<Point> Anchors;
        List<Point> curPath;
        List<Point> Path;

        bool clicking;
        Graphics g;
        Point curPOS;
        Point prePos;
        Pen pen;


        public MainForm()
        {
            InitializeComponent();
           // g = pictureBox1.CreateGraphics();
            pen = new Pen(Color.Blue, 2);
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Open the browsed image and display it
                string OpenedFilePath = openFileDialog1.FileName;
                ImageMatrix = ImageOperations.OpenImage(OpenedFilePath);
                ImageOperations.DisplayImage(ImageMatrix, pictureBox1);
                //Console.WriteLine(OpenedFilePath);
                GraphOperation.graph_construction(ImageMatrix);
                GraphOperation.print_graph(OpenedFilePath);
            }

            txtWidth.Text = ImageOperations.GetWidth(ImageMatrix).ToString();
            txtHeight.Text = ImageOperations.GetHeight(ImageMatrix).ToString();
            
            Anchors = new List<Point>();
            curPath = new List<Point>();
            Path = new List<Point>();

        }

        private void btnGaussSmooth_Click(object sender, EventArgs e)
        {
            double sigma = double.Parse(txtGaussSigma.Text);
            int maskSize = (int)nudMaskSize.Value ;
            ImageMatrix = ImageOperations.GaussianFilter1D(ImageMatrix, maskSize, sigma);
            ImageOperations.DisplayImage(ImageMatrix, pictureBox2);
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            textBox1.Text = e.X.ToString();
            textBox2.Text = e.Y.ToString();

            if (ImageMatrix != null && Anchors.Count > 0)
            {
                curPOS.X = e.X;
                curPOS.Y = e.Y;

                double distance = GraphOperation.GetDistance(curPOS.X, curPOS.Y, prePos.X, prePos.Y);


                if (curPOS != prePos  && clicking)
                {
                    
                    int src = GraphOperation.node_num(Anchors[Anchors.Count - 1].X, Anchors[Anchors.Count - 1].Y,
                                                      ImageOperations.GetWidth(ImageMatrix));
                    int fpoint = GraphOperation.node_num(curPOS.X, curPOS.Y, ImageOperations.GetWidth(ImageMatrix));

                    curPath = Shortest_path_op.Backtracking(Shortest_path_op.pre, fpoint, src, ImageOperations.GetWidth(ImageMatrix));
                    prePos = curPOS;

                   

                    if (Path.Count > 5 && clicking)
                    {
                        pictureBox1.Refresh();
                    }

                }

            }
        }
      
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left)
            {    
                if(pictureBox1.Image != null)
                {
                    
                    int x = e.Location.X;
                    int y = e.Location.Y;
                    Point p = new Point(x, y);
                    int width = ImageOperations.GetWidth(ImageMatrix);
                    int node = GraphOperation.node_num(x, y, width);
                    
                    Shortest_path_op.Dijkstra(node, GraphOperation.graph);
                    
                    Anchors.Add(p);
                    Path.AddRange(curPath);
                    pictureBox1.Refresh();
                }
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            SolidBrush redBrush = new SolidBrush(Color.Red);
            int widt = 4;
            int height = 4;
            
            if (ImageMatrix != null)
            {        
                for(int i=0;i<Anchors.Count;i++)
                {   
                    e.Graphics.FillEllipse(redBrush, Anchors[i].X, Anchors[i].Y, widt, height);
                }
                

                if (Path.Count > 5 && Path != null)
                {
                    //g = pictureBox1.CreateGraphics();

                    e.Graphics.DrawCurve(pen, Path.ToArray());
                }
               
            }

        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            clicking = true;
            curPOS = new Point(e.X, e.Y);
            prePos = new Point(e.X, e.Y);
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            clicking = false;
        }

        private void label8_Click(object sender, EventArgs e)
        {
        }
    }
}