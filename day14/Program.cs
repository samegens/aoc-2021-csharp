
using System.Text;

class Rule
{
    public Rule(string searchPair, char charToInsert)
    {
        SearchPair = searchPair;
        CharToInsert = charToInsert;
    }

    public string SearchPair { get; set; }

    public char CharToInsert { get; set; }

    public void Print()
    {
        Console.WriteLine($"{SearchPair}->{CharToInsert}");
    }
}

internal class Program
{
    private static void Main(string[] args)
    {
        var lines = File.ReadAllLines("input.txt");
        var polymer = lines.First();
        var rules = new List<Rule>();
        lines.Skip(2)
            .ToList()
            .ForEach(l => rules.Add(new Rule(l.Split("->")[0].Trim(), l.Last())));

        Console.WriteLine($"Part 1: {Solve(polymer, rules, 10)}");
        Console.WriteLine($"Part 2: {Solve(polymer, rules, 40)}");
    }

    private static long Solve(string polymer, List<Rule> rules, int nrIterations)
    {
        Dictionary<string, long> pairCounts = Enumerable.Range(0, polymer.Length - 1)
            .Select(i => polymer.Substring(i, 2))
            .GroupBy(p => p)
            .ToDictionary(grp => grp.Key, grp => (long)grp.Count());

        for (int i = 0; i < nrIterations; i++)
        {
            pairCounts = ExecuteStep(pairCounts, rules);
        }

        Dictionary<char, long> charCounts = PairCountsToCharCounts(polymer, pairCounts);
        long highest = charCounts.Select(kv => kv.Value).Max();
        long lowest = charCounts.Select(kv => kv.Value).Min();
        return highest - lowest;
    }

    private static Dictionary<string, long> ExecuteStep(Dictionary<string, long> pairCounts, List<Rule> rules)
    {
        var newPairCounts = new Dictionary<string, long>();
        foreach (KeyValuePair<string, long> pairCount in pairCounts)
        {
            Rule rule = rules.Single(r => r.SearchPair == pairCount.Key);

            string newPair1 = $"{rule.SearchPair[0]}{rule.CharToInsert}";
            if (!newPairCounts.ContainsKey(newPair1))
            {
                newPairCounts[newPair1] = 0;
            }
            newPairCounts[newPair1] = newPairCounts[newPair1] + pairCount.Value;

            string newPair2 = $"{rule.CharToInsert}{rule.SearchPair[1]}";
            if (!newPairCounts.ContainsKey(newPair2))
            {
                newPairCounts[newPair2] = 0;
            }
            newPairCounts[newPair2] = newPairCounts[newPair2] + pairCount.Value;
        }
        return newPairCounts;
    }

    private static Dictionary<char, long> PairCountsToCharCounts(string polymer, Dictionary<string, long> pairCounts)
    {
        var charCounts = new Dictionary<char, long>();
        foreach (var pairCount in pairCounts)
        {
            char ch1 = pairCount.Key[0];
            if (!charCounts.ContainsKey(ch1))
            {
                charCounts[ch1] = 0;
            }
            charCounts[ch1] = charCounts[ch1] + pairCount.Value;

            char ch2 = pairCount.Key[1];
            if (!charCounts.ContainsKey(ch2))
            {
                charCounts[ch2] = 0;
            }
            charCounts[ch2] = charCounts[ch2] + pairCount.Value;
        }

        // All chars have been counted double, except the first and last char in the polymer.
        charCounts[polymer.First()] = charCounts[polymer.First()] + 1;
        charCounts[polymer.Last()] = charCounts[polymer.Last()] + 1;
        foreach (var charCount in charCounts)
        {
            charCounts[charCount.Key] = charCounts[charCount.Key] / 2;
        }

        return charCounts;
    }
}