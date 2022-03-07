namespace Vertical.JsonClassGenerator;

public class ScalarValueSchema : SchemaNode
{
    private readonly HashSet<Type> _types = new();
    private readonly HashSet<object?> _values = new();

    /// <inheritdoc />
    public ScalarValueSchema(string path, ContainerSchema? parent) 
        : base(path, parent)
    {
    }

    public IReadOnlyCollection<Type> Types => _types;

    public IReadOnlyCollection<object?> Values => _values;
    
    public int Instances { get; private set; }

    public bool HasNullValues { get; private set; }

    public void OnValueOccurence<T>(T value) where T : notnull
    {
        _types.Add(value.GetType());
        _values.Add(value);
        HasNullValues |= value is Null;
        Instances++;
    }

    /// <inheritdoc />
    public override string ToString() => $"{Path}: [{string.Join(",", _types)})]{(HasNullValues ? "?" : null)}";
}