namespace Vertical.JsonClassGenerator;

public class JsonSchemaBuilder : IJsonCallback<Stack<ContainerSchema>>
{
    private readonly Dictionary<string, SchemaNode> _nodes = new();
    
    /// <inheritdoc />
    public Stack<ContainerSchema> CreateState() => new();

    /// <inheritdoc />
    public void VisitObject(string path, Stack<ContainerSchema> state)
    {
        AddOrGetNode(path, state, (id, parent) => new StructuralSchema(id, parent));
    }

    /// <inheritdoc />
    public void VisitArray(string path, Stack<ContainerSchema> state)
    {
        AddOrGetNode(path, state, (id, parent) => new SequenceSchema(id, parent));
    }

    /// <inheritdoc />
    public void VisitValue<T>(string path, T value, Stack<ContainerSchema> state) where T : notnull
    {
        var scalarValue = AddOrGetNode(path, state, (id, parent) => new ScalarValueSchema(id, parent));
        
        scalarValue.OnValueOccurence(value);
    }

    /// <inheritdoc />
    public void VisitNull(string path, Stack<ContainerSchema> state)
    {
        VisitValue(path, Null.Value, state);
    }

    /// <inheritdoc />
    public void LeaveObject(string path, Stack<ContainerSchema> state) => LeaveContainer<StructuralSchema>(path, state);

    /// <inheritdoc />
    public void LeaveArray(string path, Stack<ContainerSchema> state) => LeaveContainer<SequenceSchema>(path, state);

    public JsonSchema Build() => new JsonSchema(_nodes);

    private static void LeaveContainer<T>(string path, Stack<ContainerSchema> state) where T : ContainerSchema
    {
        var topNode = state.Pop();
        
        if (topNode is not T)
        {
            throw Exceptions.NodeTypeConflict(path, typeof(T), topNode.GetType());
        }
    }

    private T AddOrGetNode<T>(
        string path,
        Stack<ContainerSchema> state,
        Func<string, ContainerSchema?, T> factory) 
        where T : SchemaNode
    {
        SchemaNode? node = default;

        try
        {
            if (_nodes.TryGetValue(path, out node))
            {
                if (node is not T t)
                {
                    throw Exceptions.NodeTypeConflict(path, typeof(T), node.GetType());
                }
                return t;
            }

            var parent = state.Count > 0 ? state.Peek() : null;
            
            _nodes.Add(path, node = factory(path, parent));
            
            parent?.ChildCreated(node);

            return (T)node;
        }
        finally
        {
            if (node is ContainerSchema container)
            {
                state.Push(container);
            }   
        }
    }
}