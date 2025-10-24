using System.Xml.Linq;
using Uppgift.Models;

namespace Uppgift;

public static class SettingsReader
{
    private const string ConfigFileName = "Inställningar.xml";

    public static Settings Load()
    {
        XDocument doc = XDocument.Load(ConfigFileName);
        XElement settingsEl = doc.Element("Settings")!;

        var settings = new Settings
        {
            LogPath = settingsEl.Element("Log")?.Value ?? string.Empty
        };

        foreach (var dirEl in settingsEl.Elements("Directory"))
        {
            var dir = new DirectorySetting
            {
                Name = dirEl.Element("Name")?.Value ?? string.Empty,
                Input = dirEl.Element("Input")?.Value ?? string.Empty,
                Output = dirEl.Element("Output")?.Value ?? string.Empty,
                Types = dirEl.Elements("Type").Select(t => t.Value.ToLower()).ToList()
            };

            settings.Directories.Add(dir);
        }

        Logger.LogPath = settings.LogPath;

        return settings;
    }
}