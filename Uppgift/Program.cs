using Uppgift.Models;
using Uppgift;

public class Program
{
    public static int Main()
    {
        if (!ConfigValidator.IsValidConfigFile("Inställningar.xml"))
            return 1;

        Settings settings = SettingsReader.Load();
        var dir = settings.Directories[0];
      
        var watcher = new FileSystemWatcher(dir.Input);
        var mover = new FileMover();
        
        watcher.EnableRaisingEvents = true;
        watcher.Created += async (sender, e) =>
        {
            await mover.HandleFiles(e.FullPath, dir);
        };

        Console.WriteLine($"Bevakar {dir.Name}: {dir.Input}");
      
        var logReader = File.ReadAllText(settings.LogPath);
        Console.WriteLine(logReader);

        Task.Delay(Timeout.Infinite).Wait();
        
        return 0;
    }
}