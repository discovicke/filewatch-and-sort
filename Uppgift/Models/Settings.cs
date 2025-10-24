namespace Uppgift.Models;

public class Settings
{
    public string LogPath { get; set; } = string.Empty;
    public List<DirectorySetting> Directories { get; set; } = new();
}