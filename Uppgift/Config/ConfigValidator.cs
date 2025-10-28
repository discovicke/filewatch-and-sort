namespace Uppgift.Config;

using System.IO;
using System.Linq;
using System.Xml.Linq;

public static class ConfigValidator
{
    public static bool IsValidConfigFile(string xmlPath)
    {
        if (!File.Exists(xmlPath)) return false;

        var doc = XDocument.Load(xmlPath);
        if (doc == null) 
            return false;

        var settingsElement = doc.Element("Settings");
        if (settingsElement == null) 
            return false;

        return HasValidContent(settingsElement);
    }

    private static bool HasValidContent(XElement settings)
    {
        string logPath = settings.Element("Log")?.Value ?? string.Empty;
        logPath = NormalizePath(logPath);
        if (string.IsNullOrWhiteSpace(logPath))
            return false;
        var logDir = Path.GetDirectoryName(Path.GetFullPath(logPath));
        if (string.IsNullOrEmpty(logDir) || !Directory.Exists(logDir))
            return false;
        
        var dir = settings.Element("Directory");
        if (dir == null) 
            return false;

        var name = dir.Element("Name")?.Value ?? string.Empty;
        var input = NormalizePath(dir.Element("Input")?.Value ?? string.Empty);
        var output = NormalizePath(dir.Element("Output")?.Value ?? string.Empty);
        var types = dir
            .Elements("Type")
            .Select(t => t.Value)
            .ToList();

        if (string.IsNullOrWhiteSpace(name) 
            || string.IsNullOrWhiteSpace(input) 
            || string.IsNullOrWhiteSpace(output) 
            || !types.Any())
            return false;

        return Directory.Exists(input) && Directory.Exists(output);
    }

    public static string NormalizePath(string path)
        => path
            .Replace('\\', Path.DirectorySeparatorChar)
            .Replace('/', Path.DirectorySeparatorChar);
}