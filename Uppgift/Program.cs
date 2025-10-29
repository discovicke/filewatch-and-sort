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

        fileMover.ExistingFiles(directoryConfig).Wait();

        using var fileWatcher = new FileSystemWatcher(directoryConfig.Input)
        {
            EnableRaisingEvents = true,
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
            InternalBufferSize = 128 * 1024
        };

        fileWatcher.Created += async (sender, e) =>
        {
            await Task.Delay(100);
            await fileMover.HandleFiles(e.FullPath, directoryConfig);
        };

        fileWatcher.Changed += async (sender, e) =>
        {
            await Task.Delay(100);
            await fileMover.HandleFiles(e.FullPath, directoryConfig);
        };
        
        Thread.Sleep(2000);

        Task.Delay(Timeout.Infinite).Wait();
        return 0;
    }
}