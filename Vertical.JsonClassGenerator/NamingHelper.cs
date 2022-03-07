using System.Text.RegularExpressions;
using Humanizer;

namespace Vertical.JsonClassGenerator;

public static class NamingHelper
{
    public static string MakeProperName(string name) => Regex.Replace(name, "^[^a-zA-Z0-9_]", "").Pascalize();

}