using System.Text.RegularExpressions;

public class Point
{
    public int X { get; set; }
    public int Y { get; set; }

    public Point(int x, int y)
    {
        X = x;
        Y = y;
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

public class Line
{
    public Point Start { get; set; }
    public Point End { get; set; }

    public Line(Point start, Point end)
    {
        Start = start;
        End = end;
    }

    public override string ToString()
    {
        return $"{Start} -> {End}";
    }

    public bool IsHorizontalOrVertical
    {
        get
        {
            return Start.X == End.X || Start.Y == End.Y;
        }
    }

    public bool IsDiagonal
    {
        get
        {
            int deltaX = Math.Abs(End.X - Start.X);
            int deltaY = Math.Abs(End.Y - Start.Y);
            return deltaX == deltaY && deltaX != 0;
        }
    }

    public IEnumerable<Point> Points()
    {
        if (!IsHorizontalOrVertical && !IsDiagonal)
        {
            throw new NotImplementedException("Can only iterate over vertical, horizontal and diagonal lines)");
        }

        int x1 = Start.X, y1 = Start.Y;
        int x2 = End.X, y2 = End.Y;

        int dx = Math.Abs(x2 - x1);
        int dy = Math.Abs(y2 - y1);
        int sx = x1 < x2 ? 1 : -1;
        int sy = y1 < y2 ? 1 : -1;
        int err = (dx > dy ? dx : -dy) / 2, e2;

        while (true)
        {
            yield return new Point(x1, y1);
            if (x1 == x2 && y1 == y2)
            {
                break;
            }

            e2 = err;
            if (e2 > -dx) { err -= dy; x1 += sx; }
            if (e2 < dy) { err += dx; y1 += sy; }
        }
    }
}

internal class Program
{
    private static void Main(string[] args)
    {
        var lines = ParseLines(File.ReadAllLines("input.txt"));
        SolvePart1(lines);
        SolvePart2(lines);
   }

    private static List<Line> ParseLines(string[] textLines)
    {
        var lines = new List<Line>();
        var regex = new Regex(@"(\d+),(\d+) -> (\d+),(\d+)");

        foreach (var textLine in textLines)
        {
            var match = regex.Match(textLine);
            if (match.Success)
            {
                var startX = int.Parse(match.Groups[1].Value);
                var startY = int.Parse(match.Groups[2].Value);
                var endX = int.Parse(match.Groups[3].Value);
                var endY = int.Parse(match.Groups[4].Value);

                var startPoint = new Point(startX, startY);
                var endPoint = new Point(endX, endY);

                lines.Add(new Line(startPoint, endPoint));
            }
        }

        return lines;
    }

    private static void SolvePart1(List<Line> lines)
    {
        var heatMap = new Dictionary<Point, int>();
        var straightLines = FilterStraightLines(lines);
        PrintLines(straightLines);
        foreach (Line line in straightLines)
        {
            foreach (Point point in line.Points())
            {
                if (heatMap.ContainsKey(point))
                {
                    heatMap[point] = heatMap[point] + 1;
                }
                else
                {
                    heatMap[point] = 1;
                }
            }
        }

        int nrDangerousAreas = 0;
        foreach (KeyValuePair<Point, int> keyValue in heatMap)
        {
            if (keyValue.Value >= 2)
            {
                nrDangerousAreas++;
            }
        }

        Console.WriteLine($"Part 1: {nrDangerousAreas}");
    }

    private static void SolvePart2(List<Line> lines)
    {
        var heatMap = new Dictionary<Point, int>();
        foreach (Line line in lines)
        {
            foreach (Point point in line.Points())
            {
                if (heatMap.ContainsKey(point))
                {
                    heatMap[point] = heatMap[point] + 1;
                }
                else
                {
                    heatMap[point] = 1;
                }
            }
        }

        int nrDangerousAreas = 0;
        foreach (KeyValuePair<Point, int> keyValue in heatMap)
        {
            if (keyValue.Value >= 2)
            {
                nrDangerousAreas++;
            }
        }

        Console.WriteLine($"Part 2: {nrDangerousAreas}");
    }

    private static void PrintLines(List<Line> lines)
    {
        foreach (var line in lines)
        {
            Console.WriteLine(line);
        }
    }

    private static List<Line> FilterStraightLines(List<Line> lines)
    {
        return lines.Where(x => x.IsHorizontalOrVertical).ToList();
    }
}
