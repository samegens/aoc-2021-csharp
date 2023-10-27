class BitStream
{
    private readonly List<bool> _bits = new List<bool>();
    private int _index;

    public void AddBit(bool bit)
    {
        _bits.Add(bit);
    }

    public void AddHexDigit(char hex)
    {
        var bits = Convert.ToString(Convert.ToInt32(hex.ToString(), 16), 2);
        bits = bits.PadLeft(4, '0');
        foreach (var bit in bits)
        {
            AddBit(bit == '1');
        }
    }

    public bool GetBit()
    {
        return _bits[_index++];
    }

    public int GetBits(int nrBits)
    {
        return Enumerable.Range(0, nrBits)
            .ToList()
            .Aggregate(0, (acc, _) => (acc << 1) + (GetBit() ? 1 : 0));
    }

    public void Reset()
    {
        _index = 0;
    }

    public override string ToString()
    {
        return string.Join("", _bits.Select(b => b ? "1" : "0"));
    }
}

class Packet
{
    private readonly List<Packet> _subPackets = new List<Packet>();

    public int Read(BitStream bs)
    {
        int bitsRead = 0;

        Version = bs.GetBits(3);
        bitsRead += 3;

        TypeId = bs.GetBits(3);
        bitsRead += 3;

        if (TypeId == 4)
        {
            bool isNotLast;
            do
            {
                isNotLast = bs.GetBit();
                bitsRead++;

                Literal = (Literal << 4) + bs.GetBits(4);
                bitsRead += 4;
            }
            while (isNotLast);
            return bitsRead;
        }
        else
        {
            int lengthTypeId = bs.GetBits(1);
            bitsRead++;

            if (lengthTypeId == 0)
            {
                Length = bs.GetBits(15);
                bitsRead += 15;

                int bitsInSubPackets = 0;
                while (bitsInSubPackets < Length)
                {
                    var subPacket = new Packet();
                    bitsInSubPackets += subPacket.Read(bs);
                    _subPackets.Add(subPacket);
                }
                if (bitsInSubPackets > Length)
                {
                    throw new Exception("Too many bits read");
                }

                return bitsRead + bitsInSubPackets;
            }
            else
            {
                NrSubPackets = bs.GetBits(11);
                bitsRead += 11;

                for (int i = 0; i < NrSubPackets; i++)
                {
                    var subPacket = new Packet();
                    bitsRead += subPacket.Read(bs);
                    _subPackets.Add(subPacket);
                }

                return bitsRead;
            }
        }
    }

    public int GetSumOfVersions()
    {
        return Version + _subPackets.Sum(p => p.GetSumOfVersions());
    }

    public long Value
    {
        get
        {
            return TypeId switch
            {
                4 => Literal,
                0 => _subPackets.Sum(p => p.Value),
                1 => _subPackets.Aggregate(1L, (acc, p) => acc * p.Value),
                2 => _subPackets.Min(p => p.Value),
                3 => _subPackets.Max(p => p.Value),
                5 => _subPackets[0].Value > _subPackets[1].Value ? 1 : 0,
                6 => _subPackets[0].Value < _subPackets[1].Value ? 1 : 0,
                7 => _subPackets[0].Value == _subPackets[1].Value ? 1 : 0,
                _ => throw new Exception($"Unknown TypeId {TypeId}"),
            };
        }
    }

    public int Version { get; set; }
    public int TypeId { get; set; }
    public int Length { get; set; }
    public int NrSubPackets { get; set; }
    public long Literal { get; set; }
}

internal class Program
{
    private static void Main(string[] args)
    {
        // Parse hex chars into BitStream
        var bs = new BitStream();
        File.ReadAllText("input.txt").ToList()
            .ForEach(ch => bs.AddHexDigit(ch));

        var packet = new Packet();
        packet.Read(bs);
        SolvePart1(packet);
        SolvePart2(packet);
    }

    private static void SolvePart1(Packet packet)
    {
        Console.WriteLine($"Part 1: {packet.GetSumOfVersions()}");
    }

    private static void SolvePart2(Packet packet)
    {
        Console.WriteLine($"Part 2: {packet.Value}");
    }
}
