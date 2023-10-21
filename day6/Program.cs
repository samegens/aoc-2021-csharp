


internal class Program
{
    private static void Main(string[] args)
    {
        var numbers = File.ReadAllText("input.txt").Split(',').Select(int.Parse).ToList();
        SolvePart1(numbers);
        SolvePart2(numbers);
    }

    private static void SolvePart1(List<int> numbers)
    {
        var currentNumbers = new List<int>(numbers);
        for (int turn = 0; turn < 80; turn++)
        {
            var newNumbers = new List<int>();
            var nrNewLanternFish = 0;
            foreach (int number in currentNumbers)
            {
                if (number == 0)
                {
                    nrNewLanternFish++;
                    newNumbers.Add(6);
                }
                else
                {
                    newNumbers.Add(number - 1);
                }
            }

            for (int i = 0; i < nrNewLanternFish; i++)
            {
                newNumbers.Add(8);
            }

            currentNumbers = newNumbers;
        }

        Console.WriteLine($"Part 1: {currentNumbers.Count}");
    }

    private static void SolvePart2(List<int> numbers)
    {
        List<long> countPerTimer = Enumerable.Range(0, 5 + 1)
                             .Select(n => (long)numbers.Count(x => x == n))
                             .ToList();
        countPerTimer.Add(0); // 6
        countPerTimer.Add(0); // 7
        countPerTimer.Add(0); // 8
        for (int turn = 0; turn < 256; turn++)
        {
            var newCountPerTimer = Enumerable.Repeat(0L, 9).ToList();
            newCountPerTimer[8] = countPerTimer[0];
            newCountPerTimer[7] = countPerTimer[8];
            newCountPerTimer[6] = countPerTimer[7] + countPerTimer[0];
            newCountPerTimer[5] = countPerTimer[6];
            newCountPerTimer[4] = countPerTimer[5];
            newCountPerTimer[3] = countPerTimer[4];
            newCountPerTimer[2] = countPerTimer[3];
            newCountPerTimer[1] = countPerTimer[2];
            newCountPerTimer[0] = countPerTimer[1];

            countPerTimer = newCountPerTimer;
        }

        Console.WriteLine($"Part 2: {countPerTimer.Sum()}");
    }

}
