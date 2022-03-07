using System.Text.Json;

namespace Vertical.JsonClassGenerator;

public static class Exceptions
{
    public static Exception NoDocument()
    {
        return new ApplicationException("No JSON document was read from the input source");
    }

    public static Exception NoEndObject(string path, in Utf8JsonReader jsonReader)
    {
        return new ApplicationException($"Object at '{path}' not terminated ({DescribeReader(jsonReader)})");
    }

    public static Exception NoPropertyName(string path, Utf8JsonReader jsonReader)
    {
        return new ApplicationException($"Read null string for PropertyName token at '{path}' ({DescribeReader(jsonReader)})");
    }

    private static string DescribeReader(in Utf8JsonReader reader)
    {
        return $"position={reader.Position}";
    }

    public static Exception NodeTypeConflict(string path, Type expectedType, Type actualType)
    {
        return new InvalidOperationException($"Expected node at path '{path}' to be {expectedType}, but was {actualType}");
    }

    public static Exception ScalarTypeConflict(ScalarValueSchema scalar)
    {
        var message = $"Could not resolve to a common type for property '{scalar.Path}' among the " +
                      $"following types: [{string.Join(",", scalar.Types)}]";
        return new ApplicationException(message);
    }
}