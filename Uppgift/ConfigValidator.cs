namespace Uppgift;

using System.IO;
using System.Linq;
using System.Xml.Linq;

public static class ConfigValidator
{
    public static bool IsValidConfigFile(string xmlPath)
    {
        System.IO.Path.GetFullPath(xmlPath);
        if (!FilFinns(xmlPath))
            return false;

        XDocument? doc = LoadXml(xmlPath);
        if (doc == null)
            return false;

        var settings = doc.Element("Settings");
        if (settings == null)
            return false;

        if (!HasValidContent(settings))
            return false;

        if (!HasValidDirectories(settings))
            return false;

        return true;
    }

    private static bool FilFinns(string path)
    {
        return File.Exists(path);
    }

    private static XDocument? LoadXml(string path)
    {
        try
        {
            return XDocument.Load(path);
        }
        catch
        {
            return null;
        }
    }

    private static bool HasValidContent(XElement settings)
    {
        var log = settings.Element("Log")?.Value;
        var directory = settings.Element("Directory");
        if (directory == null)
            return false;

        var name = directory.Element("Name")?.Value ?? string.Empty;
        var input = directory.Element("Input")?.Value ?? string.Empty;
        var output = directory.Element("Output")?.Value ?? string.Empty;
        var types = directory.Elements("Type").ToList();

        if (string.IsNullOrWhiteSpace(log) ||
            string.IsNullOrWhiteSpace(name) ||
            string.IsNullOrWhiteSpace(input) ||
            string.IsNullOrWhiteSpace(output) ||
            !types.Any())
            return false;
        
        if (!Directory.Exists(input) || !Directory.Exists(output))
            return false;

        return true;
    }

    private static bool HasValidDirectories(XElement settings)
    {
        var directoryElement = settings.Element("Directory");
        if (directoryElement == null)
            return false;

        string logFile = settings.Element("Log")?.Value ?? "log.txt";
        // Normaliserar filsökvägen eftersom Steve Jobs gjorde ett designerfel för 20 år sen :)
        logFile = logFile.Replace('\\', Path.DirectorySeparatorChar)
            .Replace('/', Path.DirectorySeparatorChar);
        string? logDirectory = Path.GetDirectoryName(logFile);
        
        if (!string.IsNullOrEmpty(logDirectory) && !System.IO.Directory.Exists(logDirectory))
            return false;
        
        if (!File.Exists(logFile))
            return false;

        return true;
    }

}