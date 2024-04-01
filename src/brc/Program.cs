using System.Globalization;

var cts = new CancellationTokenSource();
Console.CancelKeyPress += (sender, e) => cts.Cancel();
var token = cts.Token;

var dataPath = "data/measurements.txt";
using var textReader = new StreamReader(dataPath);

var stats = new StationsStats();

while (!textReader.EndOfStream && !token.IsCancellationRequested)
{
    var line = await textReader.ReadLineAsync(token);
    var measure = line!.Split(';');
    var station = measure[0];
    var temperature = decimal.Parse(measure[1].AsSpan(), CultureInfo.InvariantCulture);

    stats.AddMeasurement(station, temperature);
}

foreach (var item in stats.Stats)
{
    var station = item.Key;
    var min = item.Value.Min;
    var mean = item.Value.Sum / item.Value.Count;
    var max = item.Value.Max;
    Console.WriteLine($"{station}={min}/{mean:#.0}/{max}");
}
