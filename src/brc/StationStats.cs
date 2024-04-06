using System.Numerics;

internal class StationStats<T>(T initTemperature)
    where T : INumber<T>
{
    public T Min { get; set; } = initTemperature;
    public T Max { get; set; } = initTemperature;
    public T Sum { get; set; } = initTemperature;
    public int Count { get; set; } = 1;
}

internal class StationsStats
{
    public Dictionary<string, StationStats<int>> Stats { get; } = [];

    /* private readonly List<StationStats<int>> _stats = []; */

    public void AddMeasurement(ReadOnlySpan<char> stationMeasurement)
    {
        var splitIdx = stationMeasurement.IndexOf(';');
        var station = stationMeasurement.Slice(0, splitIdx).ToString(); // TODO: avoid ToString call
        var measurement = stationMeasurement.Slice(splitIdx + 1);
        var temperature = ParseDecimalAsInt(measurement);

        if (!Stats.TryGetValue(station, out var value))
        {
            Stats.Add(station, new StationStats<int>(temperature));
        }
        else
        {
            value.Min = value.Min > temperature ? temperature : value.Min;
            value.Max = value.Max < temperature ? temperature : value.Max;
            value.Count += 1;
            value.Sum += temperature;
        }
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

    /* private StationStats<T> FindStationStat(ReadOnlySpan<char> measurement) { */
    /*   measurement.spli */
    /* } */
}
