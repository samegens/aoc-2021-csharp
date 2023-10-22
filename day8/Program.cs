


internal class Program
{
    static readonly HashSet<byte> validDigits = new()
    {
        0b1110111,
        0b0100100,
        0b1011101,
        0b1101101,
        0b0101110,
        0b1101011,
        0b1111011,
        0b0100101,
        0b1111111,
        0b1101111 
    };
    /*
        a    0      0000001
        b c  1 2  0000010 0000100
        d    3      0001000
        e f  4 5  0010000 0100000
        g    6      1000000
    */
    static readonly Dictionary<byte, int> segmentsToDigitMap = new()
    {
        { 0b1110111, 0 },   // 6 segments
        { 0b0100100, 1 },   // 2 segments
        { 0b1011101, 2 },   // 5 segments
        { 0b1101101, 3 },   // 5 segments
        { 0b0101110, 4 },   // 4 segments
        { 0b1101011, 5 },   // 5 segments
        { 0b1111011, 6 },   // 6 segments
        { 0b0100101, 7 },   // 3 segments
        { 0b1111111, 8 },   // 7 segments
        { 0b1101111, 9 }    // 6 segments
    };

    private static void Main(string[] args)
    {
        SolvePart1();
        SolvePart2();
    }

    private static void SolvePart1()
    {
        var easyLengths = new HashSet<int> { 2, 3, 4, 7 };
        var numbers = File.ReadAllLines("input.txt")
            .SelectMany(x => x.Split('|')[1].Split(' ').ToList())
            .Where(x => easyLengths.Contains(x.Length))
            .Count();
        Console.WriteLine($"Part 1: {numbers}");
    }

    private static void SolvePart2()
    {
        int totalValue = 0;
        var lines = File.ReadAllLines("input.txt");
        foreach (string line in lines)
        {
            var parts = line.Split('|');
            var allDigits = parts[0].Trim().Split(' ');
            var digitsToDecode = parts[1].Trim().Split(' ');

            // Position within segments corresponds with a, b, c, d, e, f, g of original 7-segment digit.
            List<char> segments = DetermineSegments(allDigits);
            int value = ComputeValue(digitsToDecode, segments);
            totalValue += value;
        }

        Console.WriteLine($"Part 2: {totalValue}");
    }

    private static int ComputeValue(string[] digitsToDecode, List<char> segments)
    {
        int value = 0;
        var segmentToValueMap = new Dictionary<char, byte>();
        for (int i = 0; i < 7; i++)
        {
            segmentToValueMap[segments[i]] = (byte)(1 << i);
        }

        foreach (string digitSpec in digitsToDecode)
        {
            byte bits = DigitSpecToBits(digitSpec, segmentToValueMap);
            value *= 10;
            value += segmentsToDigitMap[bits];
        }

        return value;
    }

    private static List<char> DetermineSegments(string[] allDigits)
    {
        foreach (string permutation in GeneratePermutations("abcdefg"))
        {
            var segmentToValueMap = new Dictionary<char, byte>();
            for (int i = 0; i < 7; i++)
            {
                segmentToValueMap[permutation[i]] = (byte)(1 << i);
            }

            bool isValidPermutation = true;
            foreach (string digitSpec in allDigits)
            {
                byte bits = DigitSpecToBits(digitSpec, segmentToValueMap);
                if (!validDigits.Contains(bits))
                {
                    isValidPermutation = false;
                    break;
                }
            }

            if (isValidPermutation)
            {
                return permutation.ToList();
            }
        }

        throw new InvalidProgramException("Dit zou niet moeten gebeuren");
    }

    private static byte DigitSpecToBits(string digitSpec, Dictionary<char, byte> segmentToValueMap)
    {
        return (byte)digitSpec
            .Select(ch => (int)segmentToValueMap[ch])
            .Sum();
    }

    public static IEnumerable<string> GeneratePermutations(string str)
    {
        return GeneratePermutations(str.ToCharArray(), str.Length);
    }
    
    private static IEnumerable<string> GeneratePermutations(char[] array, int n)
    {
        if (n == 1)
        {
            yield return new String(array);
        }
        else
        {
            for (int i = 0; i < n - 1; i++)
            {
                foreach (var perm in GeneratePermutations(array, n - 1))
                    yield return perm;

                // if n is even, swap the i-th and the (n-1)-th element
                // if n is odd, swap the first and the (n-1)-th element
                if (n % 2 == 0)
                {
                    Swap(array, i, n - 1);
                }
                else
                {
                    Swap(array, 0, n - 1);
                }
            }
            
            foreach (var perm in GeneratePermutations(array, n - 1))
                yield return perm;
        }
    }
    
    private static void Swap(char[] array, int a, int b)
    {
        char tmp = array[a];
        array[a] = array[b];
        array[b] = tmp;
    }
}