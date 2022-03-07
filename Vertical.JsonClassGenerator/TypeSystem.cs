namespace Vertical.JsonClassGenerator;

public class TypeSystem
{
    private enum SchemaContext
    {
        StructureMember,
        SequenceElement
    }
    
    private readonly Options _options;
    private readonly StructureCollection _structures;
    private readonly HashSet<string> _generatedPropertyNames = new();

    public TypeSystem(Options options)
    {
        _options = options;
        _structures = new(options);
    }

    public string GetStructureName(StructuralSchema structure) => _structures.GetStructureName(structure);

    public IEnumerable<string> GetSampleValues(SchemaNode schemaNode)
    {
        if (_options.SampleValues == 0 || schemaNode is not ScalarValueSchema scalar)
        {
            return Array.Empty<string>();
        }

        return scalar
            .Values
            .Take(_options.SampleValues)
            .Select(value => value?.ToString() ?? "{null}");
    }
    
    public string GetPropertyName(SchemaNode propertySchema)
    {
        var baseName = NamingHelper.MakeProperName(propertySchema.Name);
        var generatedName = baseName;
        var iteration = 1;

        while (!_generatedPropertyNames.Add($"{propertySchema.Parent!.Path}/{generatedName}"))
        {
            iteration++;
            generatedName = $"{baseName}{iteration}";
        }

        return generatedName;
    }

    public string GetPropertyTypeName(SchemaNode propertySchema)
    {
        switch (propertySchema)
        {
            case ScalarValueSchema scalar:
                return GetScalarValueTypeName(scalar, SchemaContext.StructureMember);
            
            case StructuralSchema structure:
                return GetStructureTypeName(structure, SchemaContext.StructureMember);
            
            case SequenceSchema sequence:
                return GetSequenceTypeName(sequence, SchemaContext.StructureMember);
        }

        throw new InvalidOperationException();
    }

    public string? GetPropertyInitializer(SchemaNode propertySchema)
    {
        switch (propertySchema)
        {
            case ScalarValueSchema scalar when ResolveScalarValueType(scalar) == typeof(string) && _options.InitializeStrings:
                return "string.Empty";
            
            case SequenceSchema sequence when _options.InitializeCollections:
                var elementType = GetSequenceElementTypeName(sequence, SchemaContext.StructureMember);
                return _options.SequenceType switch
                {
                    SequenceType.List => $"new List<{elementType}>()",
                    _ => $"Array.Empty<{elementType}>()"
                };  
        }

        return null;
    }

    private string GetSequenceTypeName(SequenceSchema sequence, SchemaContext context)
    {
        var elementType = sequence.ElementSchema switch
        {
            ScalarValueSchema scalar => GetScalarValueTypeName(scalar, SchemaContext.SequenceElement),
            StructuralSchema structure => GetStructureTypeName(structure, SchemaContext.SequenceElement),
            SequenceSchema elementSequence => GetSequenceTypeName(elementSequence, SchemaContext.SequenceElement),
            _ => throw new InvalidCastException()
        };

        var nullNotation = _options.NullReferenceStrategy != NullReferenceStrategy.Never
                           && context == SchemaContext.StructureMember
            ? "?"
            : "";

        return _options.SequenceType switch
        {
            SequenceType.List => $"List<{elementType}>{nullNotation}",
            SequenceType.ReadOnlyCollection => $"IReadOnlyCollection<{elementType}>{nullNotation}",
            SequenceType.ReadOnlyList => $"IReadOnlyList<{elementType}>{nullNotation}",
            _ => $"{elementType}[]{nullNotation}"
        };
    }
    
    private string GetSequenceElementTypeName(SequenceSchema sequence, SchemaContext context)
    {
        var elementType = sequence.ElementSchema switch
        {
            ScalarValueSchema scalar => GetScalarValueTypeName(scalar, SchemaContext.SequenceElement),
            StructuralSchema structure => GetStructureTypeName(structure, SchemaContext.SequenceElement),
            SequenceSchema elementSequence => GetSequenceTypeName(elementSequence, SchemaContext.SequenceElement),
            _ => throw new InvalidCastException()
        };

        return elementType;
    }

    private string GetStructureTypeName(StructuralSchema structure, SchemaContext context)
    {
        var typeName = _structures.GetStructureName(structure);

        var countAsNull = _options.NullReferenceStrategy != NullReferenceStrategy.Never
                          && context == SchemaContext.StructureMember;
            
        var nullNotation = countAsNull ? "?" : "";

        return $"{typeName}{nullNotation}";
    }

    private string GetScalarValueTypeName(ScalarValueSchema scalar, SchemaContext context)
    {
        var displayName = TypeHelpers.GetDisplayName(ResolveScalarValueType(scalar));
        var nullNotation = GetNullNotation(scalar, context);

        return $"{displayName}{nullNotation}";
    }

    private static Type ResolveScalarValueType(ScalarValueSchema scalar)
    {
        if (scalar.Types.Count == 1)
        {
            return scalar.Types.First();
        }
        
        var type = scalar.Types.First();

        foreach (var otherType in scalar.Types.Skip(1))
        {
            var higherType = TypeHelpers.GetHighestPrecisionType(type, otherType);
            if (higherType == null)
            {
                throw Exceptions.ScalarTypeConflict(scalar);
            }

            type = otherType;
        }

        return type;
    }

    private string GetNullNotation(SchemaNode schemaNode, SchemaContext context)
    {
        var underlyingSchema = schemaNode switch
        {
            SequenceSchema sequence => sequence.ElementSchema,
            _ => schemaNode
        };
        
        switch (_options.NullReferenceStrategy)
        {
            case NullReferenceStrategy.Comprehensive when context == SchemaContext.StructureMember:
                return underlyingSchema switch
                {
                    ScalarValueSchema scalar => scalar.Types.Any(t => !t.IsValueType) || scalar.HasNullValues ? "?" : "",
                    _ => "?"
                };
            
            case NullReferenceStrategy.Subjective when context == SchemaContext.StructureMember:
                return underlyingSchema switch
                {
                    ScalarValueSchema scalar => scalar.HasNullValues ? "?" : "",
                    SequenceSchema sequence => sequence.ElementSchema == null ? "?" : "",
                    _ => ""
                };
            
            default:
                return "";
        }
    }
}