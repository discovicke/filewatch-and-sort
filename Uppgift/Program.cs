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
        var directoryConfig = new DirectorySetting
        {
            Name = settings.Directories.First().Name,
            Input = settings.Directories.First().Input,
            Output = settings.Directories.First().Output,
            Types = settings.Directories
                .SelectMany(d => d.Types)
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Select(t => t.ToLowerInvariant())
                .Distinct()
                .ToList()
        };

        var fileMover = new FileMover();
        

        using var fileWatcher = new FileSystemWatcher(directoryConfig.Input)
        {
            EnableRaisingEvents = true,
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite
        };
        
        fileWatcher.Created += (sender, e) =>
        {
            _ = Task.Run(async () =>
            {
                await Task.Delay(200);
                await fileMover.HandleFiles(e.FullPath, directoryConfig);
            });
        };
        
        //_ = Task.Run(async () => await fileMover.ExistingFiles(directoryConfig));

    
        Thread.Sleep(60000);  // 60 sekunder för att hinna med alla 50 filer

        Task.Delay(Timeout.Infinite).Wait();
        
        
        return 0;
    }
}