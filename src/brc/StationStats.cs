using System.Numerics;
using System.Text;

internal class StationStats<T>(string stationName)
    where T : struct, INumber<T>
{
    public string StationName { get; } = stationName;
    public T Min { get; set; }
    public T Max { get; set; }
    public T Sum { get; set; }
    public int Count { get; set; } = 0;
}

internal class StationNameMap
{
    private class NodeComparer : IComparer<Node>
    {
        public static NodeComparer Instance = new();

        public int Compare(Node? x, Node? y)
        {
            return 0;
        }
    }

    internal class Node(string commonPrefix, int index = -1)
    {
        public string prefix = commonPrefix;
        public int index = index;
        public readonly List<Node> children = [];

        public void DebugPrint(string ident, StringBuilder sb)
        {
            _ = sb.Append(prefix);
            if (index > -1)
            {
                sb.Append($"(i{index})");
            }
            else
            {
                sb.Append($"(c{children.Count})");
            }

            int i = 0;
            foreach (var node in children)
            {
                if (i++ == 0)
                {
                    sb.Append(" --> ");
                }
                else
                {
                    sb.AppendLine();
                    sb.Append($"{ident} --> ");
                }
                node.DebugPrint(ident + "    ", sb);
            }
        }
    }

    public readonly List<Node> rootNodes = new();

    public void DebugPrint()
    {
        var sb = new StringBuilder();
        foreach (var node in rootNodes)
        {
            sb.Append('|');
            node.DebugPrint("    ", sb);
            sb.AppendLine();
        }

        Console.Write(sb.ToString());
    }
}

internal class StationsStats
{
    private readonly StationNameMap _keyMap = new();
    public List<StationStats<int>> Stats { get; } = [];

    public void AddMeasurement(ReadOnlySpan<char> stationMeasurement)
    {
        var splitIdx = stationMeasurement.IndexOf(';');
        var stationName = stationMeasurement.Slice(0, splitIdx); // TODO: avoid ToString call
        var measurement = stationMeasurement.Slice(splitIdx + 1);
        var temperature = ParseDecimalAsInt(measurement);

        var stationStats = FindStatsOrAddNew(stationName);

        /* _keyMap.DebugPrint(); */

        stationStats.Min = stationStats.Min > temperature ? temperature : stationStats.Min;
        stationStats.Max = stationStats.Max < temperature ? temperature : stationStats.Max;
        stationStats.Count += 1;
        stationStats.Sum += temperature;
    }

    public void DebugPrint() => _keyMap.DebugPrint();

    // NOTE: possible optimisation by presorting nodes
    private StationStats<int> FindStatsOrAddNew(ReadOnlySpan<char> name)
    {
        var nodes = _keyMap.rootNodes;
        var idx = 0;
        for (int i = 0; i < nodes.Count; i++)
        {
            var node = nodes[i];
            var prefix = node.prefix;
            int j = 0;
            for (; j < prefix.Length && idx + j < name.Length && prefix[j] == name[idx + j]; ++j)
            { }

            // Matched To End (Terminal Node)
            if (idx + j == name.Length)
            {
                // Full match. Node found
                if (j == prefix.Length)
                {
                    return Stats[node.index];
                }

                SplitNode(node, j, Stats.Count);
                var newStats = new StationStats<int>(name.ToString());
                Stats.Add(newStats);
                return newStats;
            }

            // Prefix Fully Matched
            if (j == prefix.Length)
            {
                idx += j;
                nodes = node.children;
                i = -1;
                continue;
            }

            // Prefix Partially Matched
            if (j > 0)
            {
                var newTerminalNode = new StationNameMap.Node(
                    name.Slice(idx + j).ToString(),
                    Stats.Count
                );
                var newStats = new StationStats<int>(name.ToString());
                Stats.Add(newStats);
                SplitNode(node, j);
                node.children.Add(newTerminalNode);
                return newStats;
            }
        }
        var newNode = new StationNameMap.Node(name.Slice(idx).ToString(), Stats.Count);
        var stats = new StationStats<int>(name.ToString());
        Stats.Add(stats);
        nodes.Add(newNode);
        return stats;
    }

    private static void SplitNode(StationNameMap.Node node, int splitAt, int newIndexValue = -1)
    {
        var oldPrefix = node.prefix.AsSpan();
        var newNode = new StationNameMap.Node(oldPrefix.Slice(splitAt).ToString(), node.index);

        node.index = newIndexValue;
        node.prefix = oldPrefix.Slice(0, splitAt).ToString();
        node.children.Add(newNode);
    }

    private static int ParseDecimalAsInt(ReadOnlySpan<char> str)
    {
        int result = (byte)str[^1] - 48;
        var currPos = 10;
        for (int i = str.Length - 3; i > 0; --i)
        {
            result += ((byte)str[i] - 48) * currPos;
            currPos *= 10;
        }
        if (str[0] == 45) // '-'
        {
            result *= -1;
        }
        else
        {
            result += ((byte)str[0] - 48) * currPos;
        }
        return result;
    }
}
