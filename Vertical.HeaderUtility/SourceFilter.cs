using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.Logging;

namespace Vertical.HeaderUtility;

public class SourceFilter
{
    public static IEnumerable<string> GetSourceFiles(Configuration configuration)
    {
        var logger = AppLogger.Default;
        var matcher = new Matcher();

        foreach (var pattern in configuration.Includes)
        {
            logger.LogDebug("Adding include pattern {pattern}", pattern);
            matcher.AddInclude(pattern);
        }

        foreach (var pattern in configuration.Excludes)
        {
            logger.LogDebug("Adding exclude pattern {pattern}", pattern);
            matcher.AddExclude(pattern);
        }

        var basePath = string.IsNullOrWhiteSpace(configuration.BasePath)
            ? Directory.GetCurrentDirectory()
            : configuration.BasePath;
        
        logger.LogInformation("Validating base path {path}", basePath);

        if (!Directory.Exists(basePath))
        {
            throw new ApplicationException($"Base search path '{basePath}' does not exist");
        }
        
        logger.LogInformation("Matching input files from {sourceDirectory}", configuration.BasePath);
        
        var results = matcher.GetResultsInFullPath(configuration.BasePath);

        return results;
    }
}