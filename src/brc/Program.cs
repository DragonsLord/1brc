using System.Globalization;

var cts = new CancellationTokenSource();
Console.CancelKeyPress += (sender, e) => cts.Cancel();
var token = cts.Token;

var dataPath = "data/measurements.txt";
using var textReader = new StreamReader(dataPath);

var result = new Dictionary<string, (decimal Min, decimal Max, int Count, decimal Sum)>();

while (!textReader.EndOfStream && !token.IsCancellationRequested)
{
    var line = await textReader.ReadLineAsync(token);
    var measure = line!.Split(';');
    var station = measure[0];
    var temperature = decimal.Parse(measure[1].AsSpan(), CultureInfo.InvariantCulture);

    if (!result.TryGetValue(station, out var value))
    {
        result.Add(station, (temperature, temperature, 1, temperature));
    }
    else
    {
        value.Min = Math.Min(value.Min, temperature);
        value.Max = Math.Max(value.Max, temperature);
        value.Count += 1;
        value.Sum += temperature;
        // TODO: use ref type instead of value type tuple?
        result[station] = value;
    }
}

foreach (var item in result)
{
    var station = item.Key;
    var min = item.Value.Min;
    var mean = item.Value.Sum / item.Value.Count;
    var max = item.Value.Max;
    Console.WriteLine($"{station}={min}/{mean}/{max}");
}
