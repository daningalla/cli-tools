namespace Vertical.JsonClassGenerator;

public class JsonSchema
{
    private readonly IReadOnlyDictionary<string, SchemaNode> _nodes;

    public JsonSchema(IReadOnlyDictionary<string, SchemaNode> nodes)
    {
        _nodes = nodes;

        var values = _nodes.Values.ToList();
        
        Structures = values.Where(n => n is StructuralSchema).Cast<StructuralSchema>().ToArray();
        Sequences = values.Where(n => n is SequenceSchema).Cast<SequenceSchema>().ToArray();
        ScalarValues = values.Where(n => n is ScalarValueSchema).Cast<ScalarValueSchema>().ToArray();
    }
    
    public IReadOnlyCollection<StructuralSchema> Structures { get; }
    
    public IReadOnlyCollection<SequenceSchema> Sequences { get; }
    
    public IReadOnlyCollection<ScalarValueSchema> ScalarValues { get; }
}