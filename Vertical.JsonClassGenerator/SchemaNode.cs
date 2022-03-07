namespace Vertical.JsonClassGenerator;

public abstract class SchemaNode
{
    protected SchemaNode(
        string path,
        ContainerSchema? parent)
    {
        Path = path;
        Parent = parent;
        Name = JsonPath.GetName(path);
    }

    public string Path { get; }

    public ContainerSchema? Parent { get; }
    
    public string Name { get; }

    /// <inheritdoc />
    public override string ToString() => Path;
}