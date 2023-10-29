


abstract class Element
{
    public Element()
    {
    }

    public Pair? Parent { get; set; }

    abstract public void Print();

    abstract public void DepthFirstTraversal(Action<RegularNumber> action);

    abstract public bool Explode(Pair root, int currentDepth);

    abstract public bool Split();

    abstract public int ComputeMagnitude();

    abstract public Element Copy();
}

class RegularNumber: Element
{
    public RegularNumber(int value)
    {
        Value = value;
    }

    public int Value { get; set; }

    public override void Print()
    {
        Console.Write(Value);
    }

    public override void DepthFirstTraversal(Action<RegularNumber> action)
    {
        action(this);
    }

    public override bool Explode(Pair root, int currentDepth)
    {
        // Ignore, Explode only works on Pairs.
        return false;
    }

    public override bool Split()
    {
        if (Value < 10)
        {
            return false;
        }

        int leftValue = Value / 2;
        int rightValue = Value - leftValue;

        Parent!.ReplaceRegularNumberByPair(this, leftValue, rightValue);

        return true;
    }

    public override int ComputeMagnitude()
    {
        return Value;
    }

    public override Element Copy()
    {
        return new RegularNumber(Value);
    }
}

class Pair: Element
{
    public Pair(Element left, Element right)
    {
        Left = left;
        left.Parent = this;
        Right = right;
        right.Parent = this;
    }

    public Element Left { get; set; }

    public Element Right { get; set; }

    public override void Print()
    {
        Console.Write("[");
        Left.Print();
        Console.Write(",");
        Right.Print();
        Console.Write("]");
        if (Parent == null)
        {
            Console.WriteLine();
        }
    }

    public override void DepthFirstTraversal(Action<RegularNumber> action)
    {
        Left.DepthFirstTraversal(action);
        Right.DepthFirstTraversal(action);
    }

    public void Reduce()
    {
        bool hasPerformedAction;
        do
        {
            hasPerformedAction = Explode(this, 1);
            hasPerformedAction = hasPerformedAction || Split();
        }
        while (hasPerformedAction);
    }

    public override int ComputeMagnitude()
    {
        return 3 * Left.ComputeMagnitude() + 2 * Right.ComputeMagnitude();
    }

    public override bool Explode(Pair root, int currentDepth)
    {
        bool hasExploded = false;
        if (currentDepth > 4 && Left is RegularNumber)
        {
            RegularNumber? firstRegularNumberOnLeft = null;
            bool foundLeft = false;
            root.DepthFirstTraversal(e => {
                if (!foundLeft && e != Left)
                {
                    firstRegularNumberOnLeft = e;
                }

                if (e == Left)
                {
                    foundLeft = true;
                }
            }
            );
            if (firstRegularNumberOnLeft != null)
            {
                firstRegularNumberOnLeft.Value += ((RegularNumber)Left).Value;
            }

            RegularNumber? firstRegularNumberOnRight = null;
            bool foundRight = false;
            root.DepthFirstTraversal(e => {
                if (foundRight && firstRegularNumberOnRight == null)
                {
                    firstRegularNumberOnRight = e;
                }

                if (e == Right)
                {
                    foundRight = true;
                }
            }
            );
            if (firstRegularNumberOnRight != null)
            {
                firstRegularNumberOnRight.Value += ((RegularNumber)Right).Value;
            }

            Parent!.ReplacePairByZero(this);
            hasExploded = true;
        }

        if (!hasExploded)
        {
            hasExploded = Left.Explode(root, currentDepth + 1);
            hasExploded = hasExploded || Right.Explode(root, currentDepth + 1);
        }
        return hasExploded;
    }

    public override bool Split()
    {
        bool hasSplit = Left.Split();
        hasSplit = hasSplit || Right.Split();
        return hasSplit;
    }

    private void ReplacePairByZero(Pair pair)
    {
        // Prevent memory leak:
        pair.Parent = null;
        var zero = new RegularNumber(0)
        {
            Parent = this
        };

        if (pair == Left)
        {
            Left = zero;
        }
        else if (pair == Right)
        {
            Right = zero;
        }
        else
        {
            throw new Exception("Invalid pair");
        }
    }

    public void ReplaceRegularNumberByPair(RegularNumber regularNumber, int leftValue, int rightValue)
    {
        // Prevent memory leak:
        regularNumber.Parent = null;

        var pair = new Pair(new RegularNumber(leftValue), new RegularNumber(rightValue))
        {
            Parent = this
        };

        if (regularNumber == Left)
        {
            Left = pair;
        }
        else if (regularNumber == Right)
        {
            Right = pair;
        }
        else
        {
            throw new Exception("Invalid regular number");
        }
    }

    public Pair AddTo(Pair other)
    {
        var sum = new Pair(this, other);
        sum.Reduce();
        return sum;
    }

    public override Element Copy()
    {
        return new Pair(Left.Copy(), Right.Copy());
    }
}

internal class Program
{
    private static void Main(string[] args)
    {
        var lines = File.ReadAllLines("input.txt");
        var pairs = lines.Select(l => ParseLine(l)).ToList();
        SolvePart1(pairs);
        SolvePart2(pairs);
    }

    private static Pair ParseLine(string l)
    {
        int index = 0;
        Pair pair = ParsePair(l, ref index);
        return pair;
    }

    private static Pair ParsePair(string chars, ref int index)
    {
        char ch = chars[index];
        if (ch != '[')
        {
            throw new Exception("Invalid input (expected '[')");
        }
        index++;
        ch = chars[index];

        Element left = char.IsDigit(ch) ? ParseRegularNumber(chars, ref index) : ParsePair(chars, ref index);

        ch = chars[index];
        if (ch != ',')
        {
            throw new Exception("Invalid input (expected ',')");
        }
        index++;

        ch = chars[index];
        Element right = char.IsDigit(ch) ? ParseRegularNumber(chars, ref index) : ParsePair(chars, ref index);

        ch = chars[index];
        if (ch != ']')
        {
            throw new Exception("Invalid input (expected ']')");
        }
        index++;

        return new Pair(left, right);
    }

    private static Element ParseRegularNumber(string chars, ref int index)
    {
        char ch = chars[index];
        if (!char.IsDigit(ch))
        {
            throw new Exception("Invalid input (expected digit)");
        }

        int value = 0;
        do
        {
            value = value * 10 + (ch - '0');
            index++;
            ch = chars[index];
        } while (char.IsDigit(ch));

        return new RegularNumber(value);
    }

    private static void SolvePart1(List<Pair> pairs)
    {
        // We need to make copies, because AddTo modifies the original pairs.
        List<Pair> pairsCopy = pairs.Select(p => (Pair)p.Copy()).ToList();
        Pair result = pairsCopy.Skip(1).Aggregate(pairsCopy.First(), (acc, p) => acc.AddTo(p));
        int answer = result.ComputeMagnitude();
        Console.WriteLine($"Part 1: {answer}");
    }

    private static void SolvePart2(List<Pair> pairs)
    {
        int highestMagnitude = 0;
        for (int i = 0; i < pairs.Count; i++)
        {
            for (int j = 0; j < pairs.Count; j++)
            {
                if (j != i)
                {
                    // We need to make copies, because AddTo modifies the original pairs.
                    Pair left = (Pair)pairs[i].Copy();
                    Pair right = (Pair)pairs[j].Copy();
                    Pair sum = left.AddTo(right);
                    highestMagnitude = Math.Max(highestMagnitude, sum.ComputeMagnitude());
                }
            }
        }

        Console.WriteLine($"Part 2: {highestMagnitude}");
    }
}
