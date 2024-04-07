using System.Diagnostics;

Console.WriteLine("Starting...");
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
    decimal min = item.Min / 10;
    decimal mean = item.Sum / 10M / item.Count;
    decimal max = item.Max / 10;
    Console.WriteLine($"{item.StationName}={min}/{mean:#.0}/{max}");
}

var elapsedTime = Stopwatch.GetElapsedTime(startTime);
Console.WriteLine($"Elapsed: {elapsedTime}");
