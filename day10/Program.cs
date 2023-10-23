


internal class Program
{
    readonly static Dictionary<char, char> MatchingChar = new Dictionary<char, char> {
        { '(', ')' },
        { ')', '(' },
        { '{', '}' },
        { '}', '{' },
        { '[', ']' },
        { ']', '[' },
        { '<', '>' },
        { '>', '<' },
    };

    readonly static Dictionary<char, int> IllegalCharScore = new Dictionary<char, int> {
        { ')', 3 },
        { ']', 57 },
        { '}', 1197 },
        { '>', 25137 }
    };

    readonly static Dictionary<char, int> AutocompleteScore = new Dictionary<char, int> {
        { ')', 1 },
        { ']', 2 },
        { '}', 3 },
        { '>', 4 }
    };

    private static void Main(string[] args)
    {
        var lines = File.ReadAllLines("input.txt");
        SolvePart1(lines);
        SolvePart2(lines);
    }

    private static void SolvePart1(string[] lines)
    {
        int answer = lines.Select(l => ParsePart1(l))
            .Where(ch => ch != null)
            .Select(ch => IllegalCharScore[ch ?? '?'])
            .Sum();
        Console.WriteLine($"Part 1: {answer}");
    }

    private static char? ParsePart1(string line)
    {
        Stack<char> parsedLine = new();
        foreach (char ch in line)
        {
            switch (ch)
            {
                case '(':
                case '[':
                case '{':
                case '<':
                    parsedLine.Push(ch);
                    break;
                case ')':
                case ']':
                case '}':
                case '>':
                    if (parsedLine.Peek() == MatchingChar[ch])
                    {
                        parsedLine.Pop();
                    }
                    else
                    {
                        // Mismatching char.
                        return ch;
                    }
                    break;
            }
        }

        // Either end of string or empty stack, both are not interesting.
        return null;
    }

    private static void SolvePart2(string[] lines)
    {
        List<long> scores = lines.Select(l => ParsePart2(l))
            .Where(s => s != null)
            .Select(s => ComputeScore(s ?? new Stack<char>()))
            .Order()
            .ToList();
        scores.ForEach(s => Console.WriteLine(s));
        long answer = scores[scores.Count / 2];
        Console.WriteLine($"Part 2: {answer}");
    }

    private static Stack<char>? ParsePart2(string line)
    {
        Stack<char> parsedLine = new();
        foreach (char ch in line)
        {
            switch (ch)
            {
                case '(':
                case '[':
                case '{':
                case '<':
                    parsedLine.Push(ch);
                    break;
                case ')':
                case ']':
                case '}':
                case '>':
                    if (parsedLine.Peek() == MatchingChar[ch])
                    {
                        parsedLine.Pop();
                    }
                    else
                    {
                        // Mismatching char, ignore.
                        return null;
                    }
                    break;
            }
        }

        if (parsedLine.Count == 0)
        {
            return null;
        }

        return parsedLine;
    }

    public static long ComputeScore(Stack<char> unmatchedChars)
    {
        return unmatchedChars
            .Aggregate(0L, (acc, ch) => acc * 5 + AutocompleteScore[MatchingChar[ch]]);
    }
}
