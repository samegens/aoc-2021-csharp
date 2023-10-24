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

enum FoldDirection
{
    X,
    Y
}

class FoldInstruction
{
    public FoldInstruction(FoldDirection direction, int foldLine)
    {
        Direction = direction;
        FoldLine = foldLine;
    }

    public FoldDirection Direction { get; set; }

    public int FoldLine { get; set; }
}

internal class Program
{
    private static void Main(string[] args)
    {
        var lines = File.ReadAllLines("input.txt");
        var points = new List<Point>();
        var instructions = new List<FoldInstruction>();
        foreach (var line in lines)
        {
            if (line.Contains(','))
            {
                var parts = line.Split(',');
                points.Add(new Point(int.Parse(parts[0]), int.Parse(parts[1])));
            }
            else if (line.Contains('='))
            {
                var parts = line.Split('=');
                int foldLine = int.Parse(parts[1]);
                FoldDirection foldDirection = parts[0].Split(' ')[2] == "x" ? FoldDirection.X : FoldDirection.Y;
                instructions.Add(new FoldInstruction(foldDirection, foldLine));
            }
        }

        SolvePart1(points, instructions);
        SolvePart2(points, instructions);
    }

    private static void SolvePart1(List<Point> points, List<FoldInstruction> instructions)
    {
        var foldedPoints = Fold(points, instructions.First());

        Console.WriteLine($"Part 1: {foldedPoints.Count}");
    }

    private static List<Point> Fold(List<Point> points, FoldInstruction foldInstruction)
    {
        // We need to make a copy because we're going to modify the list in place.
        var foldedPoints = new List<Point>(points);
        if (foldInstruction.Direction == FoldDirection.Y)
        {
            int foldY = foldInstruction.FoldLine;
            foldedPoints.Where(p => p.Y > foldY)
                .ToList()
                .ForEach(p => p.Y = foldY - (p.Y - foldY));
        }
        else
        {
            int foldX = foldInstruction.FoldLine;
            foldedPoints.Where(p => p.X > foldX)
                .ToList()
                .ForEach(p => p.X = foldX - (p.X - foldX));
        }
        return new HashSet<Point>(foldedPoints).ToList();
    }

    private static void SolvePart2(List<Point> points, List<FoldInstruction> instructions)
    {
        var foldedPoints = instructions.Aggregate(new List<Point>(points), (acc, instruction) => acc = Fold(acc, instruction));
        int width = foldedPoints.Max(p => p.X) + 1;
        int height = foldedPoints.Max(p => p.Y) + 1;
        char[,] display = new char[width, height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                display[x, y] = '.';
            }
        }
        foldedPoints.ForEach(p => display[p.X, p.Y] = '#');
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Console.Write(display[x, y]);
            }
            Console.WriteLine();
        }
    }

}