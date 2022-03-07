using Vertical.CommandLine.Configuration;
using Vertical.CommandLine.Help;

namespace Vertical.JsonClassGenerator;

public class Options
{
    public string? Source { get; set; }

    public int Indent { get; set; }

    public string Namespace { get; set; } = "YourApplication";

    public string RootClass { get; set; } = "Root";

    public SequenceType SequenceType { get; set; }

    public bool InitializeStrings { get; set; } = true;

    public bool InitializeCollections { get; set; } = true;

    public int SampleValues { get; set; } = 1;
    
    public NamingStyle NamingStyle { get; set; }
    
    public NullReferenceStrategy NullReferenceStrategy { get; set; }

    public Dictionary<string, string> RequestHeaders { get; } = new();

    public static ApplicationConfiguration<Options> Configure(ApplicationConfiguration<Options> configuration)
    {
        configuration
            .HelpOption("help", InteractiveConsoleHelpWriter.Default)
            .Help.UseFile("help.txt")
            .PositionArgument(arg => arg.Map.ToProperty(opt => opt.Source!))
            .Switch("--tabs", arg => arg.Map.Using((opt, _) => opt.Indent = 0))
            .Option<int>("--spaces", arg => arg.Map.Using((opt, count) => opt.Indent = count))
            .Option("--ns|--namespace", arg => arg.Map.ToProperty(opt => opt.Namespace))
            .Option("--root", arg => arg.Map.ToProperty(opt => opt.RootClass))
            .Option<SequenceType>("--collection-type", arg => arg.Map.ToProperty(opt => opt.SequenceType))
            .Switch("--init-strings", arg => arg.Map.ToProperty(opt => opt.InitializeStrings))
            .Switch("--init-collections", arg => arg.Map.ToProperty(opt => opt.InitializeCollections))
            .Option<int>("--sample-values", arg => arg.Map.ToProperty(opt => opt.SampleValues))
            .Option<NamingStyle>("--naming", arg => arg.Map.ToProperty(opt => opt.NamingStyle))
            .Option<NullReferenceStrategy>("--null-references", arg => arg.Map.ToProperty(opt => opt.NullReferenceStrategy))
            .Option("-h|--header|--request-header", arg => arg.Map.Using((opt, value) =>
            {
                var split = value.Split('=');
                opt.RequestHeaders[split[0].Trim()] = split[1].Trim();
            }))
            ;

        return configuration;
    }
}