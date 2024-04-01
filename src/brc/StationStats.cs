internal class StationStats(decimal initTemperature)
{
    public decimal Min { get; set; } = initTemperature;
    public decimal Max { get; set; } = initTemperature;
    public decimal Sum { get; set; } = initTemperature;
    public int Count { get; set; } = 1;
}

internal class StationsStats
{
    public Dictionary<string, StationStats> Stats { get; } = [];

    public void AddMeasurement(string station, decimal temperature)
    {
        if (!Stats.TryGetValue(station, out var value))
        {
            Stats.Add(station, new StationStats(temperature));
        }
        else
        {
            value.Min = Math.Min(value.Min, temperature);
            value.Max = Math.Max(value.Max, temperature);
            value.Count += 1;
            value.Sum += temperature;
        }
    }
}
