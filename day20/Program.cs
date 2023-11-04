using System.Text;

class BitBoard
{
    private readonly bool[,] _bits;

    public int Width { get; set; }
    public int Height { get; set; }
    public bool DefaultBit { get; set; }

    public BitBoard(int width, int height)
    {
        _bits = new bool[width, height];
        Width = width;
        Height = height;
    }

    public bool this[int x, int y]
    {
        get => (x >= 0 && x < Width && y >= 0 && y < Height) ? _bits[x, y] : DefaultBit;
        set => _bits[x, y] = value;
    }

    public void Print()
    {
        for (int y = 0; y < Height; y++)
        {
            var sb = new StringBuilder();
            for (int x = 0; x < Width; x++)
            {
                sb.Append(this[x, y] ? '#' : ' ');
            }

            Console.WriteLine(sb.ToString());
        }
        Console.WriteLine();
    }

    internal int GetBitValue(int x, int y)
    {
        int sum = 0;
        for (int dy = -1; dy <= 1; dy++)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                sum = (sum << 1) + (this[x + dx, y + dy] ? 1 : 0);
            }
        }

        return sum;
    }

    public int NrLitPixels
    {
        get
        {
            int count = 0;
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    count += this[x, y] ? 1 : 0;
                }
            }

            return count;
        }
    }
}

internal class Program
{
    private static void Main(string[] args)
    {
        // Read the first line from the file input.txt
        var lines = File.ReadAllLines("input.txt");
        List<bool> indexToBitMap = lines.First().Select(c => c == '#').ToList();

        var board = ParseLines(lines.Skip(2).ToList());

        SolvePart1(board, indexToBitMap);
        SolvePart2(board, indexToBitMap);
    }

    private static BitBoard ParseLines(List<string> lines)
    {
        var board = new BitBoard(lines.First().Length, lines.Count);
        int y = 0;
        foreach (string line in lines)
        {
            for (int x = 0; x < line.Length; x++)
            {
                board[x, y] = line[x] == '#';
            }

            y++;
        }

        return board;
    }

    private static void SolvePart1(BitBoard board, List<bool> indexToBitMap)
    {
        BitBoard newBoard = Enhance(board, indexToBitMap, 2);
        Console.WriteLine($"Part 1: {newBoard.NrLitPixels}");
    }

    private static void SolvePart2(BitBoard board, List<bool> indexToBitMap)
    {
        BitBoard newBoard = Enhance(board, indexToBitMap, 50);
        Console.WriteLine($"Part 2: {newBoard.NrLitPixels}");
    }

    private static BitBoard Enhance(BitBoard board, List<bool> indexToBitMap, int nrIterations)
    {
        BitBoard prevBoard = board;
        for (int i = 0; i < nrIterations; i++)
        {
            BitBoard currentBoard = new BitBoard(prevBoard.Width + 2, prevBoard.Height + 2);
            if (indexToBitMap[0])
            {
                // This means that when no lights are on, the center light is on.
                // This means that the area around the board is always on when i % 2 == 0.
                currentBoard.DefaultBit = (i % 2) == 0;
            }
            for (int y = 0; y < currentBoard.Height; y++)
            {
                for (int x = 0; x < currentBoard.Width; x++)
                {
                    currentBoard[x, y] = indexToBitMap[prevBoard.GetBitValue(x - 1, y - 1)];
                }
            }

            // currentBoard.Print();

            prevBoard = currentBoard;
        }

        return prevBoard;
    }
}
