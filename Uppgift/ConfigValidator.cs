namespace Uppgift;

using System.IO;
using System.Linq;
using System.Xml.Linq;

public static class ConfigValidator
{
    public static bool IsValidConfigFile(string xmlPath)
    {
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

        var name = directory.Element("Name")?.Value;
        var input = directory.Element("Input")?.Value;
        var output = directory.Element("Output")?.Value;
        var types = directory.Elements("Type").ToList();

        if (string.IsNullOrWhiteSpace(log) ||
            string.IsNullOrWhiteSpace(name) ||
            string.IsNullOrWhiteSpace(input) ||
            string.IsNullOrWhiteSpace(output) ||
            !types.Any())
            return false;

        return true;
    }

    private static bool HasValidDirectories(XElement settings)
    {
        var directory = settings.Element("Directory");
        if (directory == null) return false;

        string input = directory.Element("Input")?.Value ?? "";
        string output = directory.Element("Output")?.Value ?? "";
        string log = settings.Element("Log")?.Value ?? "";

        if (!Directory.Exists(input) || !Directory.Exists(output))
            return false;

        string? logDir = Path.GetDirectoryName(log);
        if (!string.IsNullOrEmpty(logDir) && !Directory.Exists(logDir))
            return false;

        return true;
    }
}