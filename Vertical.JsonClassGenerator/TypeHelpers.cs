namespace Vertical.JsonClassGenerator;

public static class TypeHelpers
{
    private static readonly Dictionary<Type, string> TypeDisplayNames = new()
    {
        [typeof(string)] = "string",
        [typeof(bool)] = "bool",
        [typeof(int)] = "int",
        [typeof(long)] = "long",
        [typeof(float)] = "float",
        [typeof(double)] = "double",
        [typeof(decimal)] = "decimal",
        [typeof(object)] = "object",
        [typeof(Null)] = "object"
    };

    private static readonly HashSet<(Type, Type)> CompatibleTypePairs = new()
    {
        // Identifies
        (typeof(bool), typeof(bool)),
        (typeof(int), typeof(int)),
        (typeof(long), typeof(long)),
        (typeof(float), typeof(float)),
        (typeof(double), typeof(double)),
        (typeof(decimal), typeof(decimal)),
        (typeof(object), typeof(object)),
        (typeof(Null), typeof(Null)),
        (typeof(string), typeof(string)),
        
        // Precision downgrades
        (typeof(long), typeof(int)),
        (typeof(double), typeof(float)),
        (typeof(decimal), typeof(float)),
        (typeof(decimal), typeof(double))
    };

    private static readonly HashSet<(Type, Type)> UpgradeableTypePairs = new()
    {
        // Anything from null
        (typeof(Null), typeof(bool)),
        (typeof(Null), typeof(int)),
        (typeof(Null), typeof(long)),
        (typeof(Null), typeof(float)),
        (typeof(Null), typeof(double)),
        (typeof(Null), typeof(decimal)),
        (typeof(Null), typeof(object)),
        (typeof(Null), typeof(string)),
        
        // Precision upgrades
        (typeof(int), typeof(long)),
        (typeof(int), typeof(float)),
        (typeof(int), typeof(double)),
        (typeof(int), typeof(decimal)),
        (typeof(long), typeof(decimal)),
        (typeof(float), typeof(double)),
        (typeof(float), typeof(decimal)),
        (typeof(double), typeof(decimal))
    };

    public static string GetDisplayName(Type type) => TypeDisplayNames[type];

    public static Type? GetHighestPrecisionType(Type a, Type b)
    {
        if (CompatibleTypePairs.Contains((a, b)))
        {
            return a;
        }

        if (UpgradeableTypePairs.Contains((a, b)))
        {
            return b;
        }

        return null;
    }
}