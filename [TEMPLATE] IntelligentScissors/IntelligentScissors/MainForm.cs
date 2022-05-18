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
        Point AnchorSize = new Point(6, 6);
        bool clicking,d_clicked=false;
        //Graphics g;
        Point curPOS;
        Point prePos;
        Pen pen;

        string OpenedFilePath;

        public MainForm()
        {
            InitializeComponent();
           // g = pictureBox1.CreateGraphics();
            pen = new Pen(Color.Gold, 2);
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Open the browsed image and display it
                 OpenedFilePath = openFileDialog1.FileName;
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

            if (ImageMatrix != null && Anchors.Count > 0&&!d_clicked)
            {
                curPOS.X = e.X;
                curPOS.Y = e.Y;

                double distance = GraphOperation.GetDistance(curPOS.X, curPOS.Y, prePos.X, prePos.Y);


                if (curPOS != prePos   && distance >2)
                {
                    
                    int src = GraphOperation.node_num(Anchors[Anchors.Count - 1].X, Anchors[Anchors.Count - 1].Y,
                                                      ImageOperations.GetWidth(ImageMatrix));
                    int fpoint = GraphOperation.node_num(curPOS.X, curPOS.Y, ImageOperations.GetWidth(ImageMatrix));

                    curPath = Shortest_path_op.Backtracking(Shortest_path_op.pre, fpoint, src, ImageOperations.GetWidth(ImageMatrix));
                    prePos = curPOS;

                   

                    if (curPath != null && curPath.Count>1)
                    {
                        Console.WriteLine("Refresh");
                        pictureBox1.Refresh();
                    }

                }

            }
        }
      
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            d_clicked = false;
            if (e.Button == MouseButtons.Left)
            {    
                if(pictureBox1.Image != null)
                {
                    
                    int x = e.Location.X;
                    int y = e.Location.Y;
                    
                    //prePos = new Point(e.X, e.Y);
                    Point p = new Point(x, y);
                    int width = ImageOperations.GetWidth(ImageMatrix);
                    int node = GraphOperation.node_num(x, y, width);
                    
                    if (Anchors.Count>0)
                    {
                        if(p != Anchors[Anchors.Count-1])
                        {
                            int dest = GraphOperation.node_num(Anchors[0].X, Anchors[0].Y, width);
                            Shortest_path_op.Dijkstra(node, GraphOperation.graph);
                            Anchors.Add(p);
                            Path.AddRange(curPath);
                            pictureBox1.Refresh();
                        }
                    }
                    else
                    {
                        Shortest_path_op.Dijkstra(node, GraphOperation.graph);
                        Anchors.Add(p);
                        pictureBox1.Refresh();
                    }
                
                }
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            SolidBrush redBrush = new SolidBrush(Color.Red);
            int widt = 6;
            int height = 6;
            
            if (ImageMatrix != null)
            {        
                for(int i=0;i<Anchors.Count;i++)
                {
                    e.Graphics.FillEllipse(redBrush,Anchors[i].X - AnchorSize.X / 2, Anchors[i].Y - AnchorSize.Y / 2,
                        widt, height);

                }
                
                if(Path.Count>3&&Path != null)
                {
                    e.Graphics.DrawCurve(pen, Path.ToArray());
                }

                if (curPath.Count > 1 && curPath != null)
                {
                    e.Graphics.DrawCurve(pen, curPath.ToArray());
                }
               
            }

        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            clicking = true;
            prePos = new Point(e.X, e.Y);
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            clicking = false;
        }

        private void Show_Path_btn_Click(object sender, EventArgs e)
        {
            Point point1 = new Point();
            Point point2 = new Point();

            int width = ImageOperations.GetWidth(ImageMatrix);
            int height = ImageOperations.GetHeight(ImageMatrix);

            point1.X = int.Parse(X1.Text);
            point1.Y = int.Parse(Y1.Text);
            point2.X = int.Parse(X2.Text);
            point2.Y = int.Parse(Y2.Text);

            if (point1.X > width || point2.X > width)
                if (point1.Y > height || point2.Y > height)
                    return;

            Shortest_path_op.printBacktraking(OpenedFilePath, width, point1, point2);

        }

        private void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            
            if(pictureBox1.Image != null && Anchors.Count>1)
            {
                
                if (Anchors[Anchors.Count-1] != Anchors[0])
                {
                    int fpoint = GraphOperation.node_num(Anchors[0].X, Anchors[0].Y,
                                                      ImageOperations.GetWidth(ImageMatrix));
                    int src = GraphOperation.node_num(Anchors[Anchors.Count - 1].X, Anchors[Anchors.Count - 1].Y, ImageOperations.GetWidth(ImageMatrix));

                    curPath = Shortest_path_op.Backtracking(Shortest_path_op.pre, fpoint, src, ImageOperations.GetWidth(ImageMatrix));
                    Anchors[Anchors.Count - 1] = Anchors[0];
                    if(curPath.Count>0)
                    {
                        d_clicked = true;
                        Path.AddRange(curPath);
                        Anchors.Add(e.Location);
                        pictureBox1.Refresh();
                        Anchors.Clear();
                        Path.Clear();
                    }




                }
            }
        }


    }
}