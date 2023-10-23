



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
        // Use a simple approach for generating hash code.
        // For larger systems or more complex objects, consider a more robust hashing strategy.
        return X * 17 + Y;
    }
}

class DepthMap
{
    private readonly int _width;
    private readonly int _height;
    private readonly int[,] _depths;

    public DepthMap(int width, int height)
    {
        _width = width;
        _height = height;
        _depths = new int[width, height];
    }

    public void SetAt(int x, int y, int value)
    {
        _depths[x, y] = value;
    }

    public int GetAt(Point point)
    {
        return _depths[point.X, point.Y];
    }

    public void Print()
    {
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                Console.Write($"{_depths[x, y]}");
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }

    public List<Point> GetLowPoints()
    {
        var lowPoints = new List<Point>();
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                int v = _depths[x, y];
                bool isLowPoint = true;
                if (y > 0 && _depths[x, y-1] <= v)
                {
                    isLowPoint = false;
                }
                if (y < _height-1 && _depths[x, y+1] <= v)
                {
                    isLowPoint = false;
                }
                if (x > 0 && _depths[x-1, y] <= v)
                {
                    isLowPoint = false;
                }
                if (x < _width-1 && _depths[x+1, y] <= v)
                {
                    isLowPoint = false;
                }

                if (isLowPoint)
                {
                    lowPoints.Add(new Point(x, y));
                }
            }
        }

        return lowPoints;
    }

    public Dictionary<Point, int> GetBasinSizes(List<Point> lowPoints)
    {
        var basinSizes = new Dictionary<Point, int>();
        foreach (Point point in lowPoints)
        {
            basinSizes[point] = GetBasinSize(point);
        }
        return basinSizes;
    }

    private int GetBasinSize(Point point)
    {
        var visitedPoints = new HashSet<Point>();
        return ComputeVisitedPoints(point, visitedPoints);
    }

    private int ComputeVisitedPoints(Point point, HashSet<Point> visitedPoints)
    {
        if (visitedPoints.Contains(point) ||
            point.Y < 0 ||
            point.Y >= _height ||
            point.X < 0 ||
            point.X >= _width ||
            GetAt(point) == 9)
        {
            return 0;
        }

        int nrVisitedPoints = 1;
        visitedPoints.Add(point);

        nrVisitedPoints += ComputeVisitedPoints(point.Move(0, -1), visitedPoints);
        nrVisitedPoints += ComputeVisitedPoints(point.Move(0, 1), visitedPoints);
        nrVisitedPoints += ComputeVisitedPoints(point.Move(-1, 0), visitedPoints);
        nrVisitedPoints += ComputeVisitedPoints(point.Move(1, 0), visitedPoints);

        return nrVisitedPoints;
    }
}

internal class Program
{
    private static void Main(string[] args)
    {
        var lines = File.ReadAllLines("input.txt");
        var depths = new DepthMap(lines.First().Length, lines.Length);
        for (int y = 0; y < lines.Length; y++)
        {
            for (int x = 0; x < lines[y].Length; x++)
            {
                depths.SetAt(x, y, lines[y][x] - '0');
            }
        }

        SolvePart1(depths);
        SolvePart2(depths);
    }

    private static void SolvePart1(DepthMap depths)
    {
        List<Point> lowPoints = depths.GetLowPoints();
        int answer = lowPoints.Select(p => depths.GetAt(p) + 1).Sum();
        Console.WriteLine($"Part 1: {answer}");
    }

    private static void SolvePart2(DepthMap depths)
    {
        List<Point> lowPoints = depths.GetLowPoints();
        Dictionary<Point, int> basinSizes = depths.GetBasinSizes(lowPoints);
        int answer = basinSizes.Values
            .OrderDescending()
            .Take(3)
            .Aggregate(1, (acc, n) => acc * n); // Product
        Console.WriteLine($"Part 2: {answer}");
    }
}
