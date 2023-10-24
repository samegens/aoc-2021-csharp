using System.Linq;

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

class Cell
{
    public Cell(Point pos, int energyLevel)
    {
        Pos = pos;
        EnergyLevel = energyLevel;
        Neighbours = new List<Cell>();
    }

    public Point Pos { get; set; }

    public int EnergyLevel { get; set; }

    public List<Cell> Neighbours { get; set; }

    public bool HasFlashed { get; set; }

    public void Increase()
    {
        EnergyLevel++;
        if (EnergyLevel > 9 && !HasFlashed)
        {
            HasFlashed = true;
            Neighbours.ForEach(c => c.Increase());
        }
    }

    public void EndStep()
    {
        HasFlashed = false;
        if (EnergyLevel > 9)
        {
            EnergyLevel = 0;
        }
    }
}

class Board
{
    private readonly Cell[,] _cells;

    public Board(int width, int height)
    {
        Width = width;
        Height = height;
        _cells = new Cell[width, height];
    }

    public Cell GetAt(int x, int y)
    {
        return _cells[x, y];
    }

    public Cell GetAt(Point pos)
    {
        return _cells[pos.X, pos.Y];
    }

    public void SetEnergyLevelAt(int x, int y, int energyLevel)
    {
        _cells[x, y] = new Cell(new Point(x, y), energyLevel);
    }

    public void SetNeighbours()
    {
        ForEachCell(c => {
            Point p = c.Pos;
            if (p.Y > 0)
            {
                c.Neighbours.Add(GetAt(p.Move(0, -1)));
            }
            if (p.Y > 0 && p.X < Width - 1)
            {
                c.Neighbours.Add(GetAt(p.Move(1, -1)));
            }
            if (p.X < Width - 1)
            {
                c.Neighbours.Add(GetAt(p.Move(1, 0)));
            }
            if (p.Y < Height - 1 && p.X < Width - 1)
            {
                c.Neighbours.Add(GetAt(p.Move(1, 1)));
            }
            if (p.Y < Height - 1)
            {
                c.Neighbours.Add(GetAt(p.Move(0, 1)));
            }
            if (p.Y < Height - 1 && p.X > 0)
            {
                c.Neighbours.Add(GetAt(p.Move(-1, 1)));
            }
            if (p.X > 0)
            {
                c.Neighbours.Add(GetAt(p.Move(-1, 0)));
            }
            if (p.X > 0 && p.Y > 0)
            {
                c.Neighbours.Add(GetAt(p.Move(-1, -1)));
            }
        });
    }

    public int Width { get; set; }

    public int Height { get; set; }

    public IEnumerable<Cell> Cells
    {
        get
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    yield return _cells[x, y];
                }
            }
        }
    }

    public void ForEachCell(Action<Cell> action)
    {
        foreach (Cell cell in Cells)
        {
            action(cell);
        }
    }

    internal void Print()
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                Console.Write(_cells[x, y].EnergyLevel);
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }

    internal int ExecuteStep()
    {
        ForEachCell(c => c.Increase());
        int nrFlashes = Cells.Count(c => c.HasFlashed);
        ForEachCell(c => c.EndStep());

        return nrFlashes;
    }
}

internal class Program
{
    private static void Main(string[] args)
    {
        var lines = File.ReadAllLines("input.txt");
        int width = lines.First().Length;
        int height = lines.Length;
        Board board = CreateBoardFromLines(lines, width, height);
        SolvePart1(board);

        board = CreateBoardFromLines(lines, width, height);
        SolvePart2(board);
    }

    private static Board CreateBoardFromLines(string[] lines, int width, int height)
    {
        var board = new Board(width, height);
        for (int y = 0; y < height; y++)
        {
            var line = lines[y];
            for (int x = 0; x < width; x++)
            {
                board.SetEnergyLevelAt(x, y, line[x] - '0');
            }
        }

        board.SetNeighbours();
        return board;
    }

    private static void SolvePart1(Board board)
    {
        int nrFlashes = 0;
        for (int i = 0; i < 100; i++)
        {
            nrFlashes += board.ExecuteStep();
        }
        Console.WriteLine($"Part 1: {nrFlashes}");
    }

    private static void SolvePart2(Board board)
    {
        int step = 1;
        do
        {
            if (board.ExecuteStep() == 100)
            {
                Console.WriteLine($"Part 2: {step}");
                return;
            }
            step++;
        }
        while (true);
    }
}