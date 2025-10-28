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
      
        Logger.LogPath = Path.GetFullPath(settings.LogPath);

        var fileWatcher = new FileSystemWatcher(directoryConfig.Input)
        { EnableRaisingEvents = true };
        
        var fileMover = new FileMover();
        
        fileWatcher.Created += async (sender, e) =>
        { await fileMover.HandleFiles(e.FullPath, directoryConfig); };
        
        Thread.Sleep(5000);
        
        return 0;
    }
}