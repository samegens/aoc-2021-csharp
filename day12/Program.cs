

class GraphNode
{
    public GraphNode(string name)
    {
        Name = name;
        Neighbours = new List<GraphNode>();
    }

    public string Name { get; set; }

    public List<GraphNode> Neighbours { get; set; }

    internal void AddNeighbour(GraphNode node)
    {
        Neighbours.Add(node);
    }

    internal void Print()
    {
        Neighbours.ForEach(n => Console.WriteLine($"{Name}-{n.Name}"));
    }

    public bool IsSmallCave
    { 
        get
        {
            return char.IsLower(Name[0]);
        }
    }

    public bool CanVisitTwice
    {
        get
        {
            return IsSmallCave && Name != "start" && Name != "end";
        }
    }
}

class Graph
{
    private List<GraphNode> _nodes = new List<GraphNode>();
    private Dictionary<string, GraphNode> _nameToNodeMap = new Dictionary<string, GraphNode>();

    public void AddEdge(string node1Name, string node2Name)
    {
        GraphNode node1 = GetOrCreate(node1Name);
        GraphNode node2 = GetOrCreate(node2Name);
        node1.AddNeighbour(node2);
        node2.AddNeighbour(node1);
    }

    private GraphNode GetOrCreate(string nodeName)
    {
        if (!_nameToNodeMap.ContainsKey(nodeName))
        {
            var node = new GraphNode(nodeName);
            _nodes.Add(node);
            _nameToNodeMap[nodeName] = node;
        }

        return _nameToNodeMap[nodeName];
    }

    public void Print()
    {
        _nodes.ForEach(n => n.Print());
        Console.WriteLine();
    }

    public GraphNode Start
    {
        get
        {
            return _nameToNodeMap["start"];
        }
    }

    public GraphNode End
    {
        get
        {
            return _nameToNodeMap["end"];
        }
    }

    public List<List<GraphNode>> GetPaths()
    {
        return GetPathsForNode(Start, new Stack<GraphNode>());
    }

    private List<List<GraphNode>> GetPathsForNode(GraphNode node, Stack<GraphNode> currentPath)
    {
        var paths = new List<List<GraphNode>>();
        currentPath.Push(node);
        if (node == End)
        {
            // PrintPath(currentPath);
            paths.Add(new List<GraphNode>(currentPath));
        }
        else
        {
            foreach (var neighbour in node.Neighbours)
            {
                if (!neighbour.IsSmallCave || !currentPath.Contains(neighbour))
                {
                    paths.AddRange(GetPathsForNode(neighbour, currentPath));
                }
            }
        }
        currentPath.Pop();
        return paths;
    }

    public List<List<GraphNode>> GetPathsPart2()
    {
        return GetPathsForNodePart2(Start, new Stack<GraphNode>(), false);
    }

    private List<List<GraphNode>> GetPathsForNodePart2(GraphNode node, Stack<GraphNode> currentPath, bool hasVisitedSmallCaveTwice)
    {
        var paths = new List<List<GraphNode>>();
        currentPath.Push(node);
        if (node == End)
        {
            // PrintPath(currentPath);
            paths.Add(new List<GraphNode>(currentPath));
        }
        else
        {
            foreach (var neighbour in node.Neighbours)
            {
                if (!neighbour.IsSmallCave)
                {
                    // We can always visit a large cave as often as we want.
                    paths.AddRange(GetPathsForNodePart2(neighbour, currentPath, hasVisitedSmallCaveTwice));
                }
                else
                {
                    // Neighbour is small cave.
                    if (currentPath.Contains(neighbour))
                    {
                        if (!hasVisitedSmallCaveTwice && neighbour.CanVisitTwice)
                        {
                            paths.AddRange(GetPathsForNodePart2(neighbour, currentPath, true));
                        }
                    }
                    else
                    {
                        paths.AddRange(GetPathsForNodePart2(neighbour, currentPath, hasVisitedSmallCaveTwice));
                    }
                }
            }
        }
        currentPath.Pop();
        return paths;
    }

    private static void PrintPath(Stack<GraphNode> currentPath)
    {
        Console.WriteLine(string.Join(',', currentPath.Reverse().Select(n => n.Name)));
    }
}

internal class Program
{
    private static void Main(string[] args)
    {
        var graph = new Graph();
        File.ReadAllLines("input.txt")
            .ToList()
            .ForEach(l => graph.AddEdge(l.Split('-')[0], l.Split('-')[1]));

        SolvePart1(graph);
        SolvePart2(graph);
    }

    private static void SolvePart1(Graph graph)
    {
        List<List<GraphNode>> paths = graph.GetPaths();
        Console.WriteLine($"Part 1: {paths.Count}");
    }


    private static void SolvePart2(Graph graph)
    {
        List<List<GraphNode>> paths = graph.GetPathsPart2();
        Console.WriteLine($"Part 2: {paths.Count}");
    }
}
