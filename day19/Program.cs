using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;

namespace Day19;

public class Point3d
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }

    public Point3d(int x, int y, int z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public Point3d Move(int dX, int dY, int dZ)
    {
        return new Point3d(X + dX, Y + dY, Z + dZ);
    }

    public override string ToString()
    {
        return $"({X},{Y},{Z})";
    }

    public override bool Equals(object? obj)
    {
        if (obj is Point3d otherPoint)
        {
            return X == otherPoint.X && Y == otherPoint.Y;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Z);
    }

    internal void Print()
    {
        Console.WriteLine($"{X},{Y},{Z}");
    }

    internal Vector<float> ToVector()
    {
        return Vector<float>.Build.DenseOfArray(new float[] { X, Y, Z });
    }
}

class Scanner
{
    public int Id { get; set; }
    public Point3d Position { get; set; }
    public List<Vector<float>> Beacons { get; set; }
    public bool IsSolved { get; set; }

    public Scanner(int id)
    {
        Id = id;
        Position = new Point3d(0, 0, 0);
        Beacons = new List<Vector<float>>();
    }

    public void AddBeacon(Vector<float> beacon)
    {
        Beacons.Add(beacon);
    }

    public void Print()
    {
        Console.WriteLine($"Scanner {Id} at {Position}");
        foreach (var beacon in Beacons)
        {
            Console.WriteLine($"  {beacon[0]},{beacon[1]},{beacon[2]}");
        }
    }

    internal Tuple<bool, Matrix<float>?, Vector<float>?> HasOverlapWith(Scanner scanner)
    {
        foreach (Matrix<float> rot in GenerateUniqueOrientations())
        {
            var rotatedBeacons = Beacons.Select(beacon => rot * beacon).ToList();
            // Iterate over all rotated beacons
            //   Iterate over Beacons of other scanner
            //     Compute vector of rotated beacon to other beacon
            //     Check if this results in at least 12 beacons matching up.
            for (int i = 0; i < rotatedBeacons.Count; i++)
            {
                for (int j = 0; j < scanner.Beacons.Count; j++)
                {
                    var vector = rotatedBeacons[i] - scanner.Beacons[j];
                    int nrMatchingBeacons = rotatedBeacons
                        .Select(rotatedBeacon => rotatedBeacon - vector)
                        .Intersect(scanner.Beacons)
                        .Count();
                    if (nrMatchingBeacons >= 12)
                    {
                        return new Tuple<bool, Matrix<float>?, Vector<float>?>(true, rot, vector);
                    }
                }
            }
        }
        return new Tuple<bool, Matrix<float>?, Vector<float>?>(false, null, null);
    }

    public static IEnumerable<Matrix<float>> GenerateUniqueOrientations()
    {
        var matrixBuilder = Matrix<float>.Build;

        var rotationX = matrixBuilder.DenseOfArray(new float[,]
        {
            {1, 0, 0},
            {0, 0, -1},
            {0, 1, 0}
        });

        var rotationY = matrixBuilder.DenseOfArray(new float[,]
        {
            {0, 0, 1},
            {0, 1, 0},
            {-1, 0, 0}
        });

        var rotationZ = matrixBuilder.DenseOfArray(new float[,]
        {
            {0, -1, 0},
            {1, 0, 0},
            {0, 0, 1}
        });

        var orientations = new List<Matrix<float>>();
        var primaryRotations = new List<Matrix<float>>
        {
            matrixBuilder.DenseIdentity(3),
            rotationX,
            rotationX * rotationX,
            rotationX * rotationX * rotationX
        };

        foreach (var primaryRotation in primaryRotations)
        {
            orientations.Add(primaryRotation);

            for (int i = 1; i <= 3; i++)
            {
                orientations.Add(primaryRotation * MatrixPower(rotationY, i));
                orientations.Add(primaryRotation * MatrixPower(rotationZ, i));
            }
        }

        return orientations.Distinct(new MatrixEqualityComparer());
    }

    private static Matrix<float> MatrixPower(Matrix<float> matrix, int power)
    {
        var result = Matrix<float>.Build.DenseIdentity(3);
        for (int i = 0; i < power; i++)
        {
            result = result * matrix;
        }
        return result;
    }

    private class MatrixEqualityComparer : IEqualityComparer<Matrix<float>>
    {
        public bool Equals(Matrix<float>? x, Matrix<float>? y)
        {
            if (x == null || y == null)
            {
                return false;
            }

            return x.Equals(y);
        }

        public int GetHashCode(Matrix<float> obj)
        {
            return obj.ToRowMajorArray().Aggregate(0, (acc, v) => acc ^ v.GetHashCode());
        }
    }

    internal void Transform(Matrix<float> rotation, Vector<float> translation)
    {
        Position = new Point3d(
            (int)Math.Round(translation[0]),
            (int)Math.Round(translation[1]),
            (int)Math.Round(translation[2])
        );
        Beacons = Beacons.Select(beacon => rotation * beacon - translation).ToList();
        IsSolved = true;
    }
}

internal class Program
{
    private static void Main(string[] args)
    {
        List<Scanner> scanners = ParseScanners();

        SolvePart1(scanners);
        // Note: the scanners have now been transformed, we don't need to do that again.
        SolvePart2(scanners);
    }

    private static void SolvePart1(List<Scanner> scanners)
    {
        // When we checked one scanner against another and there is no match,
        // we don't have to repeat that check.
        HashSet<Tuple<int, int>> mismatchedScanners = new HashSet<Tuple<int, int>>();

        do
        {
            for (int i = 1; i < scanners.Count; i++)
            {
                if (scanners[i].IsSolved)
                {
                    continue;
                }

                for (int j = 0; j < scanners.Count; j++)
                {
                    if (i == j || // Don't check a scanner against itself.
                        !scanners[j].IsSolved || // Don't check against unsolved scanners.
                        mismatchedScanners.Contains(new Tuple<int, int>(i, j))) // Don't check against scanners we know don't match.
                    {
                        continue;
                    }
                    Tuple<bool, Matrix<float>?, Vector<float>?> hasMatch = scanners[i].HasOverlapWith(scanners[j]);
                    if (hasMatch.Item1)
                    {
                        scanners[i].Transform(hasMatch.Item2!, hasMatch.Item3!);
                        break;
                    }
                    else
                    {
                        mismatchedScanners.Add(new Tuple<int, int>(i, j));
                    }
                }
            }
        } while (scanners.Any(scanner => !scanner.IsSolved));

        HashSet<Vector<float>> allBeacons = new HashSet<Vector<float>>();
        scanners.ForEach(scanner => allBeacons.UnionWith(scanner.Beacons));
        Console.WriteLine($"Part 1: {allBeacons.Count}");
    }

    private static void SolvePart2(List<Scanner> scanners)
    {
        // Find the largest manhattan distance between any two scanners.
        int maxDistance = 0;
        for (int i = 0; i < scanners.Count; i++)
        {
            for (int j = 0; j < scanners.Count; j++)
            {
                int manhattanDistance = Math.Abs(scanners[i].Position.X - scanners[j].Position.X) +
                    Math.Abs(scanners[i].Position.Y - scanners[j].Position.Y) +
                    Math.Abs(scanners[i].Position.Z - scanners[j].Position.Z);
                maxDistance = Math.Max(maxDistance, manhattanDistance);
            }
        }
        Console.WriteLine($"Part 2: {maxDistance}");
    }

    private static List<Scanner> ParseScanners()
    {
        return File.ReadLines("input.txt")
            .Aggregate(new List<Scanner>(), (scanners, line) =>
            {
                if (line.StartsWith("---"))
                {
                    int scannerId = int.Parse(line.Split(" ")[2]);
                    scanners.Add(new Scanner(scannerId));
                    if (scannerId == 0)
                    {
                        scanners.Last().IsSolved = true;
                    }
                }
                else if (line.Trim().Length > 0)
                {
                    var beacon = ParsePoint(line);
                    scanners.Last().AddBeacon(beacon);
                }
                return scanners;
            });
    }

    private static Vector<float> ParsePoint(string line)
    {
        var parts = line.Split(",");
        int x = int.Parse(parts[0]);
        int y = int.Parse(parts[1]);
        int z = int.Parse(parts[2]);
        return Vector<float>.Build.DenseOfArray(new float[] { x, y, z });
    }
}