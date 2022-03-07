using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace Vertical.HeaderUtility;

public class FileProcessor
{
    public static async Task ProcessAsync(
        IEnumerable<string> files, 
        Configuration configuration)
    {
        var processingTasks = files
            .Select(file => ProcessFileAsync(file, configuration))
            .ToList();

        await Task.WhenAll(processingTasks);
    }

    private static async Task ProcessFileAsync(
        string file,
        Configuration configuration)
    {
        var source = await File.ReadAllLinesAsync(file);
        var configuredHeader = configuration.Content;
        var sourceHeader = source
            .TakeWhile(str => !configuration.HeaderStops.Any(pattern => Regex.IsMatch(str, pattern)))
            .ToArray();

        if (configuredHeader.SequenceEqual(sourceHeader))
        {
            return;
        }

        var writer = new StreamWriter(new FileStream(file, FileMode.Create));

        foreach (var header in configuredHeader)
        {
            await writer.WriteLineAsync(header);
        }

        foreach (var line in source.Skip(sourceHeader.Length))
        {
            await writer.WriteLineAsync(line);
        }

        await writer.FlushAsync();
        
        AppLogger.Default.LogInformation("Wrote {Path}", file);
    }
}