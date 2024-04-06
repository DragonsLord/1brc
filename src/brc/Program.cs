using System.Diagnostics;

var startTime = Stopwatch.GetTimestamp();

var dataPath = "data/measurements.txt";
using var reader = new StreamReader(dataPath, new FileStreamOptions { BufferSize = 1_000_000 });

var stats = new StationsStats();

while (!reader.EndOfStream)
{
    var line = reader.ReadLine();

    stats.AddMeasurement(line);
}

foreach (var item in stats.Stats)
{
    var station = item.Key;
    decimal min = item.Value.Min / 10;
    decimal mean = item.Value.Sum / 10M / item.Value.Count;
    decimal max = item.Value.Max / 10;
    Console.WriteLine($"{station}={min}/{mean:#.0}/{max}");
}

var elapsedTime = Stopwatch.GetElapsedTime(startTime);
Console.WriteLine($"Elapsed: {elapsedTime}");
