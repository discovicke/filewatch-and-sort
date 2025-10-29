using System.Xml.Linq;
using Uppgift.Models;

namespace Uppgift.Config;

public static class SettingsReader
{
    private const string ConfigFileName = "Inställningar.xml";

    public static Settings Load()
    {
        var xmlDoc = XDocument.Load(ConfigFileName);
        var root = xmlDoc.Element("Settings")!;

        var settings = new Settings
        {
            LogPath = Path.GetFullPath(root.Element("Log")?.Value ?? string.Empty)
        };

        foreach (var directory in root.Elements("Directory"))
        {
            var directorySetting = new DirectoryRule
            {
                Name = directory.Element("Name")?.Value ?? string.Empty,
                Input = directory.Element("Input")?.Value ?? string.Empty,
                Output = directory.Element("Output")?.Value ?? string.Empty,
                Types = directory.Elements("Type")
                    .Select(t => t.Value
                        .ToLowerInvariant())
                    .ToList()
            };

            settings.Directories.Add(directorySetting);
        }
        
        Logger.LogPath = Path.GetFullPath(ConfigValidator.NormalizePath(settings.LogPath));

        return settings;
    }
}