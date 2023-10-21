
class Cell
{
    public Cell(int value)
    {
        Value = value;
    }

    public int Value { get; set; }
    public bool IsDrawn { get; set; }
}

class Board
{
    private readonly Cell[,] _board = new Cell[5, 5];

    public void SetAt(int x, int y, int value)
    {
        _board[x, y] = new Cell(value);
    }

    public void Reset()
    {
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                _board[x, y].IsDrawn = false;
            }
        }
    }

    /// <summary>
    /// If present on the board, marks the number as drawn. Returns true if the board has bingo.
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    public bool Draw(int number)
    {
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                if (_board[x, y].Value == number)
                {
                    _board[x, y].IsDrawn = true;
                    return HasBingo();
                }
            }
        }

        return false;
    }

    public bool HasBingo()
    {
        return HasBingoInRow() || HasBingoInColumn();
    }

    public void Print()
    {
        for (int y = 0; y < 5; y++)
        {
            for (int x = 0; x < 5; x++)
            {
                if (_board[x, y].IsDrawn)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                Console.Write($"{_board[x, y].Value,2} ");
                Console.ResetColor();
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }

    public int GetSumOfUnmarkedNumbers()
    {
        int sum = 0;
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                if (!_board[x, y].IsDrawn)
                {
                    sum += _board[x, y].Value;
                }
            }
        }

        return sum;
    }

    private bool HasBingoInRow()
    {
        for (int y = 0; y < 5; y++)
        {
            bool hasBingo = true;
            for (int x = 0; x < 5; x++)
            {
                if (!_board[x, y].IsDrawn)
                {
                    hasBingo = false;
                    break;
                }
            }

            if (hasBingo)
            {
                return true;
            }
        }

        return false;
    }

    private bool HasBingoInColumn()
    {
        for (int x = 0; x < 5; x++)
        {
            bool hasBingo = true;
            for (int y = 0; y < 5; y++)
            {
                if (!_board[x, y].IsDrawn)
                {
                    hasBingo = false;
                    break;
                }
            }

            if (hasBingo)
            {
                return true;
            }
        }

        return false;
    }
}

internal class Program
{
    private static void Main(string[] args)
    {
        var lines = File.ReadAllLines("input.txt");
        var numbersToDraw = lines[0].Split(",").Select(x => int.Parse(x));

        List<Board> boards = ParseBoards(lines);

        PrintBoards(boards);

        SolvePart1(numbersToDraw, boards);
        SolvePart2(numbersToDraw, boards);
    }

    private static List<Board> ParseBoards(string[] lines)
    {
        var boards = new List<Board>();
        var lineIndex = 2;
        Board? currentBoard = null;
        while (lineIndex < lines.Length)
        {
            if (currentBoard == null)
            {
                currentBoard = new Board();
                boards.Add(currentBoard);
            }

            string line = lines[lineIndex].Trim();
            if (line.Length <= 0)
            {
                currentBoard = null;
            }
            else
            {
                var numbersInRow = line.Split(" ").Where(x => x.Length > 0).Select(x => int.Parse(x)).ToList();
                int y = (lineIndex - 2) % 6;
                for (int x = 0; x < 5; x++)
                {
                    currentBoard.SetAt(x, y, numbersInRow[x]);
                }
            }

            lineIndex++;
        }

        return boards;
    }

    private static void SolvePart1(IEnumerable<int> numbersToDraw, List<Board> boards)
    {
        foreach (var number in numbersToDraw)
        {
            foreach (var board in boards)
            {
                if (board.Draw(number))
                {
                    int sum = board.GetSumOfUnmarkedNumbers();
                    Console.WriteLine($"Part 1: {sum * number}");
                    return;
                }
            }
        }
    }

    private static void SolvePart2(IEnumerable<int> numbersToDraw, List<Board> boards)
    {
        var lastBoard = FindLastBoard(numbersToDraw, boards);
        lastBoard.Print();
        foreach (var number in numbersToDraw)
        {
            if (lastBoard.Draw(number))
            {
                Console.WriteLine($"Last number = {number}");
                lastBoard.Print();
                Console.WriteLine($"Part 2: {number * lastBoard.GetSumOfUnmarkedNumbers()}");
                return;
            }
        }
    }

    private static Board FindLastBoard(IEnumerable<int> numbersToDraw, List<Board> boards)
    {
        foreach (var board in boards)
        {
            board.Reset();
        }
        var boardsLeft = new HashSet<Board>(boards);
        foreach (var number in numbersToDraw)
        {
            foreach (var board in boards)
            {
                if (boardsLeft.Contains(board) && board.Draw(number))
                {
                    Console.WriteLine("Removing board:");
                    board.Print();
                    boardsLeft.Remove(board);
                }
            }

            if (boardsLeft.Count == 1)
            {
                break;
            }
        }

        if (boardsLeft.Count > 1)
        {
            throw new Exception("Teveel borden nog over!");
        }
        return boardsLeft.First();
    }

    private static void PrintBoards(List<Board> boards)
    {
        foreach (var board in boards)
        {
            board.Print();
        }
    }
}
