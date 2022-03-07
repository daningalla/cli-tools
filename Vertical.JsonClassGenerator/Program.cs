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
        using var httpClient = new HttpClient();
        foreach (var (key, value) in options.RequestHeaders)
        {
            httpClient.DefaultRequestHeaders.Add(key, value);
        }
        var response = await httpClient.GetByteArrayAsync(source);
        return new MemoryStream(response);
    }

    return new FileStream(source, FileMode.Open);
}
