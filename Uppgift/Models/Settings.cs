namespace Uppgift.Models;

public class Settings
{
    public string LogPath { get; set; } = string.Empty;
    public List<DirectoryRule> Directories { get; set; } = new();
}