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

class Box
{
    private readonly Point _topLeft;
    private readonly Point _bottomRight;

    public int Width => _bottomRight.X - _topLeft.X + 1;

    public int Height => _topLeft.Y - _bottomRight.Y + 1;

    public Point TopLeft => _topLeft;

    public Point BottomRight => _bottomRight;

    public Box(Point topLeft, Point bottomRight)
    {
        _topLeft = topLeft;
        _bottomRight = bottomRight;
    }

    public bool Contains(Point point)
    {
        return point.X >= _topLeft.X && point.X <= _bottomRight.X &&
               point.Y <= _topLeft.Y && point.Y >= _bottomRight.Y;
    }

    public bool IsLeftOf(Point point)
    {
        return _bottomRight.X < point.X;
    }

    public bool IsAbove(Point point)
    {
        return _bottomRight.Y > point.Y;
    }

    public void Print()
    {
        Console.WriteLine($"Box: {_topLeft} to {_bottomRight}");
    }
}

internal class Program
{
    private static void Main(string[] args)
    {
        var line = File.ReadAllText("input.txt");
        var match = Regex.Match(line, @"x=(-?\d+)\.\.(-?\d+), y=(-?\d+)\.\.(-?\d+)");
        var x1 = int.Parse(match.Groups[1].Value);
        var x2 = int.Parse(match.Groups[2].Value);
        var y1 = int.Parse(match.Groups[3].Value);
        var y2 = int.Parse(match.Groups[4].Value);
        var topLeft = new Point(Math.Min(x1, x2), Math.Max(y1, y2));
        var bottomRight = new Point(Math.Max(x1, x2), Math.Min(y1, y2));
        var box = new Box(topLeft, bottomRight);
        List<Tuple<Point, int>> vectorsThatHitBox = ComputeVectorsThatHitBox(box);

        SolvePart1(vectorsThatHitBox);
        SolvePart2(vectorsThatHitBox);
    }

    private static void SolvePart1(List<Tuple<Point, int>> vectorsThatHitBox)
    {
        int answer = vectorsThatHitBox.Select(v => v.Item2).Max();
        Console.WriteLine($"Part 1: {answer}");
    }

    private static void SolvePart2(List<Tuple<Point, int>> vectorsThatHitBox)
    {
        int answer = vectorsThatHitBox.Count;
        Console.WriteLine($"Part 2: {answer}");
    }

    private static List<Tuple<Point, int>> ComputeVectorsThatHitBox(Box box)
    {
        var vectorsThatHitBox = new List<Tuple<Point, int>>();
        // Iterate in diagonal lines over (x, y) coordinates
        // to try vectors (x, y) = (1, 0), (0, 1), (2, 0), (1, 1), (0, 2), etc...
        // until we find a vector that hits the box
        for (int x = 1; x < box.BottomRight.X; x++)
        {
            for (int y = -x; y <= x; y++)
            {
                var vector = new Point(x - y, y);
                // Console.WriteLine($"Trying vector {vector}");
                Tuple<bool, int> result = VectorHitsBox(vector, box);
                if (result.Item1)
                {
                    vectorsThatHitBox.Add(Tuple.Create(vector, result.Item2));
                }
            }
        }
        return vectorsThatHitBox;
    }

    private static Tuple<bool, int> VectorHitsBox(Point vector, Box box)
    {
        var currentPoint = new Point(0, 0);
        var xVelocity = vector.X;
        var yVelocity = vector.Y;
        int highestY = int.MinValue;
        while (true)
        {
            currentPoint = currentPoint.Move(xVelocity, yVelocity);
            if (currentPoint.Y > highestY)
            {
                highestY = currentPoint.Y;
            }
            if (box.Contains(currentPoint))
            {
                // Console.WriteLine($"Vector {vector} hits box at {currentPoint}");
                return Tuple.Create(true, highestY);
            }

            // We can stop if it's right of the box.           
            if (box.IsLeftOf(currentPoint))
            {
                // Console.WriteLine($"Vector {vector} misses box at {currentPoint} because it's left of the box");
                return Tuple.Create(false, highestY);
            }

            // We can stop if it's below the box.
            if (box.IsAbove(currentPoint))
            {
                // Console.WriteLine($"Vector {vector} misses box at {currentPoint} because it's below the box");
                return Tuple.Create(false, highestY);
            }

            if (xVelocity == 0)
            {
                if (currentPoint.X < box.TopLeft.X)
                {
                    // Console.WriteLine($"Vector {vector} misses box at {currentPoint} because it's left of the box and has no x velocity");
                    return Tuple.Create(false, highestY);
                }
            }
            else
            {
                if (xVelocity > 0)
                {
                    xVelocity--;
                }
                else
                {
                    xVelocity++;
                }
            }

            yVelocity--;
        }
    }
}
