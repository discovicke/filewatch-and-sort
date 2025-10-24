namespace Uppgift.Models;

public class DirectorySetting
{
    public string Name { get; set; }
    public string Input { get; set; }
    public string Output { get; set; }
    public List<string> Types { get; set; } = new();
}