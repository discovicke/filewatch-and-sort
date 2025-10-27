using Uppgift.Models;
using Uppgift;

public class Program
{
    public static int Main()
    {
        if (!ConfigValidator.IsValidConfigFile("Inställningar.xml"))
            return 1;

        Settings settings = SettingsReader.Load();

        foreach (var dir in settings.Directories)
        {
            Thread.Sleep(1000);
            var watcher = new FileSystemWatcher(dir.Input);
            var mover = new FileMover();

            watcher.EnableRaisingEvents = true;
            watcher.Created += (sender, e) => 
                mover.HandleFiles(e.FullPath, dir);

            Console.WriteLine($"Bevakar {dir.Name}: {dir.Input}");
        }
        
        return 0;
    }
}