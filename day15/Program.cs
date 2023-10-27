public class Point
{
    public int X { get; set; }
    public int Y { get; set; }

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public Point Move(int dX, int dY)
    {
        return new Point(X + dX, Y + dY);
    }

    public override string ToString()
    {
        return $"({X},{Y})";
    }

    public override bool Equals(object? obj)
    {
        if (obj is Point otherPoint)
        {
            return X == otherPoint.X && Y == otherPoint.Y;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return X * 17 + Y;
    }

    internal void Print()
    {
        Console.WriteLine($"{X},{Y}");
    }
}

public class GraphNode
{
    public GraphNode(int riskLevel, Point pos)
    {
        RiskLevel = riskLevel;
        Pos = pos;
    }

    public int RiskLevel { get; set; }

    public Point Pos { get; set; }

    public override string ToString()
    {
        return $"{Pos}: {RiskLevel}";
    }

    public int ShortestPath { get; set; }
}

public class NonDirectedGraph
{
    private readonly Dictionary<GraphNode, List<GraphNode>> _adjacencyList;
    // Dictionary to speed up position lookups
    private readonly Dictionary<Point, GraphNode> _pointToNodeMap = new Dictionary<Point, GraphNode>();

    public NonDirectedGraph()
    {
        _adjacencyList = new Dictionary<GraphNode, List<GraphNode>>();
    }

    public void AddVertex(GraphNode vertex)
    {
        if (!_adjacencyList.ContainsKey(vertex))
        {
            _adjacencyList[vertex] = new List<GraphNode>();
            _pointToNodeMap[vertex.Pos] = vertex;
        }
    }

    public void AddEdge(GraphNode vertex1, GraphNode vertex2)
    {
        if (!_adjacencyList.ContainsKey(vertex1))
        {
            throw new Exception($"Vertex {vertex1} not found");
        }

        if (!_adjacencyList.ContainsKey(vertex2))
        {
            throw new Exception($"Vertex {vertex2} not found");
        }

        _adjacencyList[vertex1].Add(vertex2);
    }

    public List<GraphNode> GetNeighbors(GraphNode vertex)
    {
        if (!_adjacencyList.ContainsKey(vertex))
        {
            return new List<GraphNode>();
        }

        return _adjacencyList[vertex];
    }

    public GraphNode GetAt(Point pos)
    {
        return _pointToNodeMap[pos];
    }

    public void Print()
    {
        foreach (var kvp in _adjacencyList)
        {
            Console.Write($"{kvp.Key} -> ");
            Console.WriteLine(string.Join(", ", kvp.Value));
        }
    }

    public int ComputeShortestPathFromStartToEnd()
    {
        int maxX = _adjacencyList.Keys.Max(n => n.Pos.X);
        int maxY = _adjacencyList.Keys.Max(n => n.Pos.Y);
        GraphNode endNode = GetAt(new Point(maxX, maxY));

        var queue = new PriorityQueue<GraphNode, int>();
        var visited = new HashSet<GraphNode>();
        var shortestPaths = new Dictionary<GraphNode, int>();
        foreach (var kvp in _adjacencyList)
        {
            shortestPaths[kvp.Key] = int.MaxValue;
        }

        shortestPaths[GetAt(new Point(0, 0))] = 0;
        queue.Enqueue(GetAt(new Point(0, 0)), 0);
        while (queue.Count > 0)
        {
            GraphNode node = queue.Dequeue();
            if (visited.Contains(node))
            {
                continue;
            }

            visited.Add(node);
            foreach (GraphNode neighbor in GetNeighbors(node))
            {
                int distance = neighbor.RiskLevel;
                if (shortestPaths[node] + distance < shortestPaths[neighbor])
                {
                    shortestPaths[neighbor] = shortestPaths[node] + distance;
                }
                queue.Enqueue(neighbor, shortestPaths[neighbor]);
            }
        }

        return shortestPaths[endNode];
    }
}

internal class Program
{
    private static void Main(string[] args)
    {
        var lines = File.ReadAllLines("input.txt");
        var board = new int[lines[0].Length, lines.Length];
        for (int y = 0; y < lines.Length; y++)
        {
            for (int x = 0; x < lines[y].Length; x++)
            {
                board[x, y] = lines[y][x] - '0';
            }
        }

        NonDirectedGraph graph = BoardToGraph(board);
        SolvePart1(graph);

        board = ExtendBoard(board);
        // PrintBoard(board);

        graph = BoardToGraph(board);
        SolvePart2(graph);
    }

    private static void PrintBoard(int[,] board)
    {
        for (int y = 0; y < board.GetLength(1); y++)
        {
            for (int x = 0; x < board.GetLength(0); x++)
            {
                Console.Write(board[x, y]);
            }
            Console.WriteLine();
        }
    }

    private static int[,] ExtendBoard(int[,] board)
    {
        var extendedBoard = new int[board.GetLength(0) * 5, board.GetLength(1) * 5];
        var width = board.GetLength(0);
        var height = board.GetLength(1);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                extendedBoard[x, y] = board[x, y];
                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        if (i != 0 || j != 0)
                        {
                            extendedBoard[x + i * width, y + j * height] = ((board[x, y] + i + j - 1) % 9) + 1;
                        }
                    }
                }
            }
        }
        return extendedBoard;
    }

    private static NonDirectedGraph BoardToGraph(int[,] board)
    {
        var graph = new NonDirectedGraph();
        int width = board.GetLength(0);
        int height = board.GetLength(1);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                graph.AddVertex(new GraphNode(board[x, y], new Point(x, y)));
            }
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                GraphNode node1 = graph.GetAt(new Point(x, y));
                if (x > 0)
                {
                    GraphNode node2 = graph.GetAt(new Point(x - 1, y));
                    graph.AddEdge(node1, node2);
                }
                if (x < width - 1)
                {
                    GraphNode node2 = graph.GetAt(new Point(x + 1, y));
                    graph.AddEdge(node1, node2);
                }
                if (y > 0)
                {
                    GraphNode node2 = graph.GetAt(new Point(x, y - 1));
                    graph.AddEdge(node1, node2);
                }
                if (y < height - 1)
                {
                    GraphNode node2 = graph.GetAt(new Point(x, y + 1));
                    graph.AddEdge(node1, node2);
                }
            }
        }

        return graph;
    }

    private static void SolvePart1(NonDirectedGraph graph)
    {
        int answer = graph.ComputeShortestPathFromStartToEnd();
        Console.WriteLine($"Part 1: {answer}");
    }

    private static void SolvePart2(NonDirectedGraph graph)
    {
        int answer = graph.ComputeShortestPathFromStartToEnd();
        Console.WriteLine($"Part 2: {answer}");
    }
}
