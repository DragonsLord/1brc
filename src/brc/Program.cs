using System.Diagnostics;

static int ParseDecimalAsInt(ReadOnlySpan<char> str)
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

var startTime = Stopwatch.GetTimestamp();

var dataPath = "data/measurements.txt";
using var reader = new StreamReader(dataPath, new FileStreamOptions { BufferSize = 1_000_000 });

var stats = new StationsStats<int>();

while (!reader.EndOfStream)
{
    var line = reader.ReadLine();
    var measure = line!.Split(';');
    var station = measure[0];
    var temperature = ParseDecimalAsInt(measure[1]);

    stats.AddMeasurement(station, temperature);
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
