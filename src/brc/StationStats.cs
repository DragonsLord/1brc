using System.Numerics;

internal class StationStats<T>(T initTemperature)
    where T : INumber<T>
{
    public T Min { get; set; } = initTemperature;
    public T Max { get; set; } = initTemperature;
    public T Sum { get; set; } = initTemperature;
    public int Count { get; set; } = 1;
}

internal class StationsStats<T>
    where T : INumber<T>
{
    public Dictionary<string, StationStats<T>> Stats { get; } = [];

    public void AddMeasurement(string station, T temperature)
    {
        if (!Stats.TryGetValue(station, out var value))
        {
            Stats.Add(station, new StationStats<T>(temperature));
        }
        else
        {
            value.Min = value.Min > temperature ? temperature : value.Min;
            value.Max = value.Max < temperature ? temperature : value.Max;
            value.Count += 1;
            value.Sum += temperature;
        }
    }
}
