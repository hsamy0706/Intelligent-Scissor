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
        public MainForm()
        {
            InitializeComponent();
        }
        static List<Point> Anchors;
        bool moving;
        RGBPixel[,] ImageMatrix;

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Open the browsed image and display it
                string OpenedFilePath = openFileDialog1.FileName;
                ImageMatrix = ImageOperations.OpenImage(OpenedFilePath);
                ImageOperations.DisplayImage(ImageMatrix, pictureBox1);
                Console.WriteLine(OpenedFilePath);
                GraphOperation.graph_construction(ImageMatrix);
                GraphOperation.print_graph(OpenedFilePath);
            }
            txtWidth.Text = ImageOperations.GetWidth(ImageMatrix).ToString();
            txtHeight.Text = ImageOperations.GetHeight(ImageMatrix).ToString();
            Anchors = new List<Point>();
  
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
                    pictureBox1.Refresh();
                    Console.WriteLine(Anchors.Count);
                    
             
                }
             
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            SolidBrush redBrush = new SolidBrush(Color.Red);
            int widt = 4;
            int height = 4;
            Console.WriteLine("print");
            if (ImageMatrix != null)
            {
                Console.WriteLine("in print");
                for(int i=0;i<Anchors.Count;i++)
                {
                    
                    e.Graphics.FillEllipse(redBrush, Anchors[i].X, Anchors[i].Y, widt, height);
                }
             
            }
        
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
        }
    }
}