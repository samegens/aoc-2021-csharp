

internal class Program
{
    private static void Main(string[] args)
    {
        var numbers = File.ReadAllText("input.txt").Split(',').Select(int.Parse).ToList();
        // SolvePart1(numbers);
        SolvePart2(numbers);
    }

    private static void SolvePart1(List<int> numbers)
    {
        float avg = (float)numbers.Sum() / (float)numbers.Count;
        Console.WriteLine($"Part 1: {avg}");

        for (int pos = 0; pos < numbers.Count; pos++)
        {
            Console.WriteLine($"pos {pos} total fuel = {GetTotalFuel(numbers, pos)}");
        }
        int answer = Enumerable.Range(0, numbers.Count).Select(x => GetTotalFuel(numbers, x)).Min();
        Console.WriteLine($"Part 1: {answer}");
    }

    private static int GetTotalFuel(List<int> numbers, int pos)
    {
        return numbers.Select(x => Math.Abs(pos - x)).Sum();
    }

    private static void SolvePart2(List<int> numbers)
    {
        List<int> fuelPerDistance = Enumerable.Range(0, numbers.Max() + 1).Select(x => Enumerable.Range(0, x+1).Sum()).ToList();
        // fuelPerDistance.ForEach(x => Console.WriteLine(x));
        int answer = Enumerable.Range(0, numbers.Count).Select(pos => GetTotalFuelPart2(numbers, pos, fuelPerDistance)).Min();
        Console.WriteLine($"Part 2: {answer}");
    }

    private static int GetTotalFuelPart2(List<int> numbers, int pos, List<int> fuelPerDistance)
    {
        return numbers.Select(x => fuelPerDistance[Math.Abs(pos - x)]).Sum();
    }
}
