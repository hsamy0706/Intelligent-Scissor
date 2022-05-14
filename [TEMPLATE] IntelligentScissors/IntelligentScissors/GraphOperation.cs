using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace IntelligentScissors
{
    class GraphOperation
    {
        static Stopwatch stopwatch;
        public static double inf = 1e+16;
        public static Dictionary<int, List<KeyValuePair<int, double>>> graph;
       
        public static int node_num(int x,int y,int width)
        {
            return x + (y * width);
        }
       
        public static Vector2D get_XY_ofNode(int node,int width)
        {
            Vector2D position = new Vector2D();
            position.X = (int)node % (int)width;
            position.Y = (int)node / width;
            return position;
        }

        public static double GetDistance(int x1, int y1, int x2, int y2)
        {
            return Math.Sqrt(Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2));
        }

        public static void graph_construction(RGBPixel[,] ImageMatrix)
        {
            stopwatch = new Stopwatch();
            stopwatch.Start();
            int height = ImageOperations.GetHeight(ImageMatrix);
            int width = ImageOperations.GetWidth(ImageMatrix);

            ///////////key = current node , value = (destination, wieght) 
             graph = new Dictionary<int, List<KeyValuePair<int, double>>>();
            //i>>row  j>> column
            for (int i=0;i<height;i++)
            {
                for(int j=0;j<width;j++)
                {
                    int node = node_num(j, i, width);
                    
                    Vector2D G = ImageOperations.CalculatePixelEnergies(j, i, ImageMatrix);
                    
                    if (j+1 < width)//there is right pixel
                    {
                        int r_node = node_num( j+1,i, width);
                        if (!graph.ContainsKey(node))
                        {
                            List<KeyValuePair<int, double>> edges = new List<KeyValuePair<int, double>>();
                            if (G.X == 0)
                            {
                                KeyValuePair<int, double> edge = new KeyValuePair<int, double>(r_node,inf);
                                edges.Add(edge);

                                graph.Add(node, edges);
                            }
                            else
                            {
                                KeyValuePair<int, double> edge = new KeyValuePair<int, double>(r_node, (1 / G.X));
                                edges.Add(edge);

                                graph.Add(node, edges);
                            }  
                        }
                        else
                        {
                            if (G.X == 0)
                            {
                                KeyValuePair<int, double> edge = new KeyValuePair<int, double>(r_node, inf);
                                graph[node].Add(edge);
                            }
                            else
                            {
                                KeyValuePair<int, double> edge = new KeyValuePair<int, double>(r_node, (1 / G.X));
                                graph[node].Add(edge);
                            }
                        }
                        /////////////////////////////////////add edge to (right node) to be undirected 
                        if (!graph.ContainsKey(r_node))
                        {
                            List<KeyValuePair<int, double>> edges = new List<KeyValuePair<int, double>>();
                            if (G.X == 0)
                            {
                                KeyValuePair<int, double> edge = new KeyValuePair<int, double>(node, inf);
                                edges.Add(edge);

                                graph.Add(r_node, edges);
                            }
                            else
                            {
                                KeyValuePair<int, double> edge = new KeyValuePair<int, double>(node, (1 / G.X));
                                edges.Add(edge);

                                graph.Add(r_node, edges);
                            }
                        }
                        else
                        {
                            if (G.X == 0)
                            {
                                KeyValuePair<int, double> edge = new KeyValuePair<int, double>(node, inf);
                                graph[r_node].Add(edge);
                            }
                            else
                            {
                                KeyValuePair<int, double> edge = new KeyValuePair<int, double>(node, (1 / G.X));
                                graph[r_node].Add(edge);
                            }
                        }

                    }
                    ////////////////////////////////////////////////////////////////// 
                  

                    if(i+1 < height)// there is bottom pixel?
                    {
                        int b_node = node_num(j,i + 1, width);
                        if (!graph.ContainsKey(node))
                        {
                            List<KeyValuePair<int, double>> edges = new List<KeyValuePair<int, double>>();
                            if (G.Y == 0)
                            {
                                KeyValuePair<int, double> edge = new KeyValuePair<int, double>(b_node, inf);
                                edges.Add(edge);

                                graph.Add(node, edges);
                            }
                            else
                            {
                                KeyValuePair<int, double> edge = new KeyValuePair<int, double>(b_node, (1 / G.Y));
                                edges.Add(edge);

                                graph.Add(node, edges);
                            }
                        }
                        else
                        {
                            if (G.Y == 0)
                            {
                                KeyValuePair<int, double> edge = new KeyValuePair<int, double>(b_node, inf);
                                graph[node].Add(edge);
                            }
                            else
                            {
                                KeyValuePair<int, double> edge = new KeyValuePair<int, double>(b_node, (1 / G.Y));
                                graph[node].Add(edge);
                            }
                        }
                        /////////////////////////////////////add edge to (bottom node) to be undirected 
                        if (!graph.ContainsKey(b_node))
                        {
                            List<KeyValuePair<int, double>> edges = new List<KeyValuePair<int, double>>();
                            if (G.Y == 0)
                            {
                                KeyValuePair<int, double> edge = new KeyValuePair<int, double>(node, inf);
                                edges.Add(edge);

                                graph.Add(b_node, edges);
                            }
                            else
                            {
                                KeyValuePair<int, double> edge = new KeyValuePair<int, double>(node, (1 / G.Y));
                                edges.Add(edge);

                                graph.Add(b_node, edges);
                            }
                        }
                        else
                        {
                            if (G.Y == 0)
                            {
                                KeyValuePair<int, double> edge = new KeyValuePair<int, double>(node, inf);
                                graph[b_node].Add(edge);
                            }
                            else
                            {
                                KeyValuePair<int, double> edge = new KeyValuePair<int, double>(node, (1 / G.Y));
                                graph[b_node].Add(edge);
                            }
                        }
                    }

                }
            }

            stopwatch.Stop();
            Console.WriteLine("Elapsed Time is {0} s", stopwatch.ElapsedMilliseconds / 1000.0);
        }
      
        public static void print_graph(string filePath)
        {
            string[] s = filePath.Split('.');
            string path = s[0] + ".txt";
            if (!File.Exists(path))
            {
                try
                {
                    StreamWriter sw = new StreamWriter(path);

                    sw.WriteLine("Constructed Graph: (Format: node_index|edges:(from, to, weight)(from, to, weight)...)");

                    for (int i = 0; i < graph.Count(); i++)
                    {
                        sw.Write(i + "|edges");

                        //sw.WriteLine("Edges \n");
                        foreach (var item in graph[i])
                        {
                            sw.Write("(" + i + "," + item.Key + "," + (item.Value) + ")");

                        }

                        sw.Write("\n");
                    }
                    sw.Write("Elapsed Time is {0} s", stopwatch.ElapsedMilliseconds / 1000.0);
                    sw.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    
    }
}
