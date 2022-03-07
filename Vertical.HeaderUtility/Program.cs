using Microsoft.Extensions.Logging;
using Vertical.HeaderUtility;

var logger = AppLogger.Initialize();

logger.LogInformation("Vertical.HeaderUtility v1.0");

var source = args.Length == 1 
    ? args[0] 
    : Path.Combine(Directory.GetCurrentDirectory(), "header.json");

try
{
    var configuration = await Configuration.FromFileAsync(source);
    var files = SourceFilter.GetSourceFiles(configuration);

    await FileProcessor.ProcessAsync(files, configuration);
}
catch (ApplicationException ex)
{
    logger.LogError(ex.Message);
}
catch (Exception ex)
{
    logger.LogError(ex, "Unhandled error occurred");
}




