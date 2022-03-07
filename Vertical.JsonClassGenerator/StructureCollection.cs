namespace Vertical.JsonClassGenerator;

public class StructureCollection : Dictionary<string, (SchemaNode Schema, string ClassName)>
{
    private readonly Options _options;
    private readonly HashSet<string> _generatedClassNames = new();

    public StructureCollection(Options options)
    {
        _options = options;
    }
    
    public string GetStructureName(StructuralSchema structure) => GetContainerName(structure);
    
    private string GetContainerName(ContainerSchema container)
    {
        return TryGetValue(container.Path, out var entry)
            ? entry.ClassName
            : GenerateContainerName(container);
    }

    private string GenerateContainerName(ContainerSchema schema)
    {
        ICollection<string> components = new List<string>(5);

        if (schema.IsRoot)
        {
            return _options.RootClass;
        }

        for (var current = schema; current != null; current = current.Parent)
        {
            switch (current)
            {
                case { Parent: StructuralSchema }:
                    components.Add(NamingHelper.MakeProperName(current.Name));
                    break;
                
                case { Parent: SequenceSchema }:
                    components.Add("Item");
                    break;
                
                case { IsRoot: true }:
                    components.Add(_options.RootClass);
                    break;
                
                default:
                    components.Add(NamingHelper.MakeProperName(current.Name));
                    break;
            }
        }

        return RegisterStructureName(schema, string.Join("", components.Reverse()));
    }
    
    private string RegisterStructureName(ContainerSchema schema, string name)
    {
        var generatedName = name;

        while (!_generatedClassNames.Add(generatedName))
        {
            generatedName = $"{name}{Guid.NewGuid().ToString("N")[..5]}";
        }
        
        Add(schema.Path, (schema, generatedName));

        return generatedName;
    }
}