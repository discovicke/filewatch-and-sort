using Uppgift;
using Uppgift.Models;
using Uppgift.Config;

public class Program
{
    public static int Main()
    {
        if (!ConfigValidator.IsValidConfigFile("Inställningar.xml"))
            return 1;

        var settings = SettingsReader.Load();
        var directoryConfig = settings.Directories[0];

        using var fileWatcher = new FileSystemWatcher(directoryConfig.Input)
        {
            EnableRaisingEvents = true
        };

        
        var fileMover = new FileMover();
        
        fileWatcher.Created += (sender, e) =>
        {
            _ = Task.Run(async () =>
            {
                await fileMover.HandleFiles(e.FullPath, directoryConfig);
            });
        };
        
        Task.Delay(2000).Wait();
        
        return 0;
    }
}