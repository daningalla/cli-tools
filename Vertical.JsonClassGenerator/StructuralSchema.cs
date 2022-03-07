namespace Vertical.JsonClassGenerator;

public class StructuralSchema : ContainerSchema
{
    private readonly Dictionary<string, SchemaNode> _properties = new();
    
    /// <inheritdoc />
    public StructuralSchema(string path, ContainerSchema? parent) : base(path, parent)
    {
    }

    public IReadOnlyCollection<SchemaNode> Properties => _properties.Values;

    /// <inheritdoc />
    public override void ChildCreated(SchemaNode node)
    {
        _properties.Add(JsonPath.GetName(node.Path), node);
    }

    /// <inheritdoc />
    public override string ToString() => $"{Path}: (structural)";
}