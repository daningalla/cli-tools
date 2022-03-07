namespace Vertical.JsonClassGenerator;

public class ClassWriter
{
    private const string SummarySection = "summmary";
    private const string RemarksSection = "remarks";
    
    private readonly Options _options;
    private readonly FormattedTextWriter _writer;

    public ClassWriter(Options options, TextWriter textWriter)
    {
        _options = options;
        _writer = new(options, textWriter);
    }

    public void Write(JsonSchema jsonSchema)
    {
        WriteImports();
        WriteNamespace();
        WriteStructures(jsonSchema);
    }

    private void WriteStructures(JsonSchema jsonSchema)
    {
        var typeSystem = new TypeSystem(_options);
        
        _writer.WriteLine();
        _writer.ResetLineSpacing();

        foreach (var structure in jsonSchema.Structures)
        {
            _writer.WriteSpacingLine();
            WriteStructure(typeSystem, structure);
        }
    }

    private void WriteStructure(TypeSystem typeSystem, StructuralSchema structure)
    {
        _writer.WriteDocumentationSection(SummarySection, 
            "  Represents a structural entity in the source JSON document.",
            $"  Path: {structure.Path}");
        _writer.WriteLine($"public class {typeSystem.GetStructureName(structure)}");
        _writer.WriteLine("{");
        WriteProperties(typeSystem, structure);
        _writer.WriteLine("}");
    }

    private void WriteProperties(TypeSystem typeSystem, StructuralSchema structure)
    {
        _writer.Indent++;
        _writer.ResetLineSpacing();

        foreach (var property in structure.Properties)
        {
            _writer.WriteSpacingLine();
            WriteProperty(typeSystem, property);
        }
        
        _writer.Indent--;
    }

    private void WriteProperty(TypeSystem typeSystem, SchemaNode property)
    {
        _writer.WriteDocumentationSection(SummarySection, $"  Gets or sets the '{property.Name}' value.");
        _writer.WriteDocumentationSection(
            RemarksSection,
            $"  Path: {property.Path}",
            typeSystem.GetSampleValues(property).Select(value => $"  Sample value: '{value}'"));
        _writer.WriteLine($"[JsonPropertyName(\"{property.Name}\")]");
        _writer.Write("public ");
        _writer.Write(typeSystem.GetPropertyTypeName(property));
        _writer.Write(" ");
        _writer.Write(typeSystem.GetPropertyName(property));
        _writer.Write(" { get; set; }");

        var initializer = typeSystem.GetPropertyInitializer(property);

        if (initializer != null)
        {
            _writer.Write($" = {initializer};");
        }

        _writer.WriteLine();
    }

    private void WriteNamespace()
    {
        _writer.WriteLine();
        _writer.WriteLine($"namespace {_options.Namespace};");
    }

    private void WriteImports()
    {
        _writer.WriteLine("using System;");
        _writer.WriteLine("using System.Collections.Generic;");
        _writer.WriteLine("using System.Text.Json.Serialization;");
    }
}