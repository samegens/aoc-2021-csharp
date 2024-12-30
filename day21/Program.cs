
class Die
{
    private int _nextRoll = 1;
    public int NrRolls { get; set; }

    public int Roll3Times()
    {
        int result = 0;
        for (int i = 0; i < 3; i++)
        {
            result += _nextRoll;
            _nextRoll = (_nextRoll % 100) + 1;
            NrRolls++;
        }
        return result;
    }
}

internal class Program
{
    private static void Main(string[] args)
    {
        // Note: the puzzle uses 1-10, but we use 0-9 to keep the logic easier.
        // We just need to make sure we add an extra 1 to the score each time.
        var startingPositions = File.ReadLines("input.txt").Select(x => int.Parse(x.Split(' ').Last()) - 1).ToList();

        SolvePart1(startingPositions);
    }

    private static void SolvePart1(IEnumerable<int> startingPositions)
    {
        List<int> positions = new List<int>(startingPositions);
        List<int> scores = new List<int>(startingPositions.Select(x => 0));
        Die die = new Die();
        while (scores.Max() < 1000)
        {
            for (int i = 0; i < positions.Count; i++)
            {
                positions[i] = (positions[i] + die.Roll3Times()) % 10;
                scores[i] += positions[i] + 1;
                Console.WriteLine($"Player {i + 1} rolled {positions[i] + 1} and has {scores[i]} points");
                if (scores[i] >= 1000)
                {
                    break;
                }
            }
        }

        int answer = die.NrRolls * scores.Min();
        Console.WriteLine($"Part 1: {answer}");
    }
}