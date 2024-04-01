using System.Diagnostics;
using System.Globalization;

var startTime = Stopwatch.GetTimestamp();

var dataPath = "data/measurements.txt";
using var reader = new StreamReader(dataPath, new FileStreamOptions { BufferSize = 1_000_000 });

var stats = new StationsStats();

while (!reader.EndOfStream)
{
    var line = reader.ReadLine();
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

var elapsedTime = Stopwatch.GetElapsedTime(startTime);
Console.WriteLine($"Elapsed: {elapsedTime}");
