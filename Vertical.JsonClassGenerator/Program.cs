using System.Net;
using System.Text.RegularExpressions;
using Vertical.CommandLine;
using Vertical.CommandLine.Configuration;
using Vertical.JsonClassGenerator;

var configuration = new ApplicationConfiguration<Options>();

Options
    .Configure(configuration)
    .OnExecuteAsync(ExecuteAsync);

await CommandLineApplication.RunAsync(configuration, args);

static async Task ExecuteAsync(Options options, CancellationToken cancellationToken)
{
    try
    {
        await using var jsonReader = new JsonReader(await GetInputStreamAsync(options));
        var schemaBuilder = new JsonSchemaBuilder();
        await jsonReader.ReadAsync(schemaBuilder);

        var schema = schemaBuilder.Build();
        var classWriter = new ClassWriter(options, Console.Out);
        
        classWriter.Write(schema);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}

static async Task<Stream> GetInputStreamAsync(Options options)
{
    var source = options.Source;
    
    if (string.IsNullOrWhiteSpace(source))
    {
        throw new ArgumentException("Source cannot be null");
    }

    if (Regex.IsMatch(source, "^https?://"))
    {
        using var httpClient = new HttpClient(new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.All
        });
        foreach (var (key, value) in options.RequestHeaders)
        {
            httpClient.DefaultRequestHeaders.Add(key, value);
        }
        httpClient.DefaultRequestHeaders.Add("Accept", "*/*");
        httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
        httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");

        var httpResponse = await httpClient.GetAsync(source);
        var memoryStream = new MemoryStream();
        await httpResponse.Content.CopyToAsync(memoryStream);
        memoryStream.Position = 0;
        return memoryStream;
    }

    return new FileStream(source, FileMode.Open);
}
