namespace Vertical.JsonClassGenerator;

public static class JsonPath
{
    public const char Separator = '/';
    public const string Root = "/";
    public const string ElementName = "$(item)";

    public static string Combine(string root, string name) => root == Root
        ? $"/{name}"
        : $"{root}{Separator}{name}";

    public static string CombineElementName(string root) => Combine(root, ElementName);

    public static bool IsRootPath(string path) => path == Root;

    public static string GetName(string nodePath) => Path.GetFileName(nodePath);
}