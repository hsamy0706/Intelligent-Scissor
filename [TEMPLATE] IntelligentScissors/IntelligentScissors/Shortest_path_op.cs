using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Diagnostics;


namespace IntelligentScissors
{
    class PriorityQueue
    {
        private List<KeyValuePair<int, double>> list;
        public int Count { get { return list.Count; } }
        public readonly bool IsDescending;

        public PriorityQueue()
        {
            list = new List<KeyValuePair<int, double>>();
        }

        public PriorityQueue(bool isdesc)
            : this()
        {
            IsDescending = isdesc;
        }

        public PriorityQueue(int capacity)
            : this(capacity, false)
        { }

        public PriorityQueue(IEnumerable<KeyValuePair<int, double>> collection)
            : this(collection, false)
        { }

        public PriorityQueue(int capacity, bool isdesc)
        {
            list = new List<KeyValuePair<int, double>>(capacity);
            IsDescending = isdesc;
        }

        public PriorityQueue(IEnumerable<KeyValuePair<int, double>> collection, bool isdesc)
            : this()
        {
            IsDescending = isdesc;
            foreach (var item in collection)
                Enqueue(item);
        }


        public void Enqueue(KeyValuePair<int, double> x)
        {
            list.Add(x);
            int i = Count - 1;

            while (i > 0)
            {
                int p = (i - 1) / 2;
                if ((IsDescending ? -1 : 1) * list[p].Value.CompareTo(x.Value) <= 0) break;

                list[i] = list[p];
                i = p;
            }

            if (Count > 0) list[i] = x;
        }

        public KeyValuePair<int, double> Dequeue()
        {
            KeyValuePair<int, double> target = Peek();
            KeyValuePair<int, double> root = list[Count - 1];
            list.RemoveAt(Count - 1);

            int i = 0;
            while (i * 2 + 1 < Count)
            {
                int a = i * 2 + 1;
                int b = i * 2 + 2;
                int c = b < Count && (IsDescending ? -1 : 1) * list[b].Value.CompareTo(list[a].Value) < 0 ? b : a;

                if ((IsDescending ? -1 : 1) * list[c].Value.CompareTo(root.Value) >= 0) break;
                list[i] = list[c];
                i = c;
            }

            if (Count > 0) list[i] = root;
            return target;
        }

        public KeyValuePair<int, double> Peek()
        {
            if (Count == 0) throw new InvalidOperationException("Queue is empty.");
            return list[0];
        }
        public bool IsEmpty()
        {
            if (list.Count == 0)//if you had run out of them , say true :D
                return true;
            return false;
        }
        public void Clear()
        {
            list.Clear();
        }
    }

    class Shortest_path_op
    {
        public static int[] pre;
        static Stopwatch stopwatch;

        public static void Dijkstra(int src, Dictionary<int, List<KeyValuePair<int, double>>> graph)
        {
            int n = graph.Count;
            const double oo = 10000000000000000000;
            double[] dist = new double[graph.Count];
            pre = new int[graph.Count];
            bool[] visited = new bool[graph.Count];
            PriorityQueue nextTovisit = new PriorityQueue();
            for (int i = 0; i < n; i++)
            {
                dist[i] = oo;
                pre[i] = -1;
            }
            dist[src] = 0;
            pre[src] = src;
            nextTovisit.Enqueue(new KeyValuePair<int, double>(src, 0));

            while (!nextTovisit.IsEmpty())
            {
                int u = nextTovisit.Dequeue().Key;
                if (visited[u])
                    continue;
                visited[u] = true;
                foreach (var child in graph[u])
                {
                    int v = child.Key;
                    double c = child.Value;

                    if (dist[u] + c < dist[v])
                    {
                        dist[v] = dist[u] + c;
                        pre[v] = u;
                        nextTovisit.Enqueue(new KeyValuePair<int, double>(v, dist[v]));
                    }

                }

            }

        }

        public static void createPath(int[] parent, int fpoint, int src, List<Point> pointPath, int width)
        {
            if (parent[fpoint] == -1 || parent[fpoint] == src)
                return;

            createPath(parent, parent[fpoint], src, pointPath, width);

            var point = GraphOperation.get_XY_ofNode(fpoint, width);
            pointPath.Add(new Point((int)point.X, (int)point.Y));
        }

        public static List<Point> Backtracking(int[] parent, int fpoint, int src, int width)
        {
            List<Point> xy_Path = new List<Point>();

            createPath(parent, fpoint, src, xy_Path, width);

            return xy_Path;
        }

        
    }
}
