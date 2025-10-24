using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Uppgift;

public static class ConfigValidator
{
    public static int ÄrOgiltigKonfiguration(string xmlPath)
    {
        if (!File.Exists(xmlPath))
            return 1;

        XDocument doc;

        try
        {
            doc = XDocument.Load(xmlPath);
        }
        catch
        {
            return 1;
        }

        var settings = doc.Element("Settings");
        if (settings == null)
            return 1;

        var log = settings.Element("Log")?.Value;
        var directory = settings.Element("Directory");
        if (directory == null)
            return 1;

        var name = directory.Element("Name")?.Value;
        var input = directory.Element("Input")?.Value;
        var output = directory.Element("Output")?.Value;
        var types = directory.Elements("Type").ToList();

        if (string.IsNullOrWhiteSpace(log) ||
            string.IsNullOrWhiteSpace(name) ||
            string.IsNullOrWhiteSpace(input) ||
            string.IsNullOrWhiteSpace(output) ||
            !types.Any())
            return 1;

        if (!Directory.Exists(input) || !Directory.Exists(output))
            return 1;

        string? logDir = Path.GetDirectoryName(log);
        if (!string.IsNullOrEmpty(logDir) && !Directory.Exists(logDir))
            return 1;

        return 0;
    }
}