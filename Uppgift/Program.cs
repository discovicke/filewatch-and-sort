using Uppgift;
using Uppgift.Models;
using Uppgift.Config;

public class Program
{

    public static int Main()
    {
        if (!ConfigValidator.IsValidConfigFile("Inställningar.xml"))
            return 1;

        var xmlSettings = SettingsReader.Load();
        var fileMover = new FileMover();

        var validExt = new Dictionary<string, List<DirectoryRule>>();
        foreach (var directoryConfig in xmlSettings.Directories)
        {
            fileMover
                .ExistingFiles(directoryConfig)
                .Wait();

            foreach (var configExt in directoryConfig.Types)
            {
                if (!validExt.ContainsKey(configExt))
                    validExt[configExt] = new List<DirectoryRule>();
                validExt[configExt].Add(directoryConfig);
            }
        }

        var dirGroupedByInput = xmlSettings.Directories
            .GroupBy(d => d.Input)
            .ToList();

        var inputWatchers = new List<FileSystemWatcher>();

        foreach (var inputGroup in dirGroupedByInput)
        {
            var inputWatcher = new FileSystemWatcher(inputGroup.Key)
            {
                EnableRaisingEvents = true,
            };

            inputWatcher.Created += async (sender, e) =>
            {
                await Task.Delay(100);
                var fileExtentions = Path.GetExtension(e.FullPath)
                    .ToLowerInvariant();
                if (validExt
                    .TryGetValue(fileExtentions, out var matchingDirectories))
                {
                    foreach (var config in matchingDirectories)
                    {
                        await fileMover
                            .HandleFiles(e.FullPath, config);
                    }
                }
            };

            inputWatcher.Changed += async (sender, e) =>
            {
                await Task.Delay(100);
                var fileExtentions = Path.GetExtension(e.FullPath)
                    .ToLowerInvariant();
                if (validExt
                    .TryGetValue(fileExtentions, out var matchingDirectories))
                {
                    foreach (var rule in matchingDirectories)
                    {
                        await fileMover
                            .HandleFiles(e.FullPath, rule);
                    }
                }
            };
            
            inputWatchers
                .Add(inputWatcher);
        }

        Thread.Sleep(2000);
        Task.Delay(Timeout.Infinite)
            .Wait();
        return 0;
    }
}