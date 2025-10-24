namespace Uppgift.Models;

public class DirectorySetting
{
    public string Name { get; set; } = string.Empty;
    public string Input { get; set; } = string.Empty;
    public string Output { get; set; } = string.Empty;
    public List<string> Types { get; set; } = new();
}