using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace Vertical.HeaderUtility;



public class Configuration
{
    public string? BasePath { get; set; } = Directory.GetCurrentDirectory();

    public string[] HeaderStops { get; set; } = 
    {
        @"^\s*using",
        @"^\s*namespace",
        @"^\s*(public|private|internal|protected)",
        @"^\s*(class|struct|interface|record|delegate|enum|partial)",
        @"^\s*(readonly|static|sealed)",
        @"^\s*(extern|ref|unsafe)",
        @"^\s*#\w+",
    };

    [JsonPropertyName("include")]
    public string[] Includes { get; set; } = 
    {
        "**/*.cs"
    };

    [JsonPropertyName("exclude")]
    public string[] Excludes { get; set; } =
    {
        "**/bin/",
        "**/obj/"
    };

    public string[] Content { get; set; } = Array.Empty<string>();

    public static async Task<Configuration> FromFileAsync(string source)
    {
        var logger = AppLogger.Default;
        
        logger.LogInformation("Reading configuration from '{path}'", source);

        if (string.IsNullOrWhiteSpace(Path.GetFileName(source)))
        {
            source = Path.Combine(source, "headers.json");
        }
        
        await using var stream = new FileStream(source, FileMode.Open);
        var configuration = await JsonSerializer.DeserializeAsync<Configuration>(stream, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return configuration ?? throw new ApplicationException("Configuration file loaded but no content was deserialized");
    }
}