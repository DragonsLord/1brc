using System.Diagnostics;
using System.Text;

var rowCount = 1_000_000_000;

Random rand = new();
var startTime = Stopwatch.GetTimestamp();

// Write the string array to a new file named "WriteLines.txt".
using (
    StreamWriter outputFile =
        new(Path.Combine("data", "measurements.txt"), true, Encoding.UTF8, 1048576)
)
{
    for (int i = 0; i < rowCount; i++)
    {
        WeatherStation station = Stations.All[rand.Next(Stations.All.Count)];
        await outputFile.WriteAsync(station.Name);
        await outputFile.WriteAsync($";{station.Measurement(rand):0.0}");
        await outputFile.WriteAsync('\n');
    }
}

var elapsedTime = Stopwatch.GetElapsedTime(startTime);

Console.WriteLine($"Completed: {elapsedTime}");
