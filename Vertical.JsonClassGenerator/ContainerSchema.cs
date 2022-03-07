namespace Vertical.JsonClassGenerator;

public abstract class ContainerSchema : SchemaNode
{
    /// <inheritdoc />
    protected ContainerSchema(string path, ContainerSchema? parent) : base(path, parent)
    {
    }

    public virtual void ChildCreated(SchemaNode node)
    {
    }

    public bool IsRoot => Parent == null;
}