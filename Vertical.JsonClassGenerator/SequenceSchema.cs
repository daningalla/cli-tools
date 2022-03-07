namespace Vertical.JsonClassGenerator;

public class SequenceSchema : ContainerSchema
{
    /// <inheritdoc />
    public SequenceSchema(string path, ContainerSchema? parent) : base(path, parent)
    {
    }
    
    public SchemaNode? ElementSchema { get; private set; }

    /// <inheritdoc />
    public override void ChildCreated(SchemaNode node) => ElementSchema = node;

    /// <inheritdoc />
    public override string ToString() => $"{Path}: (sequence)";
}