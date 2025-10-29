using Uppgift;
using Uppgift.Models;
using Uppgift.Config;

public class Program
{
                                    // ==== Review Comments ====
                                    //Osäker på namngivelsen här?
    public static List<FileSystemWatcher> inputWatchers = new();//InputWatchers eller inputWatchers?
    public static bool isReloading = false;                     //Borde det vara IsReloading eftersom den är publik?
    private static readonly object reloadLock = new object();   //...och kanske ReloadLock här eftersom den är static?
                                                                //...eller _reloadLock för att indikera att det är ett privat fält?
                                                                //Du får gärna svara när du rättar inlämningen så jag vet! :)
    public static int Main()
    {
        if (!ConfigValidator.IsValidConfigFile("Inställningar.xml"))
            return 1;

        StartFileMonitor();
        StartConfigWatcher();

        Thread.Sleep(2000);
        Task.Delay(Timeout.Infinite)
            .Wait();
        return 0;
    }
    
    public static void StartConfigWatcher()
    {
        var configDir = Path.GetDirectoryName(Path.GetFullPath("Inställningar.xml"))!;
        var configWatcher = new FileSystemWatcher(configDir)
        {
            Filter = "Inställningar.xml",
            EnableRaisingEvents = true
        };

        configWatcher.Changed += async (sender, e) =>
        {
            await Task.Delay(300); 
            ReloadConfiguration();
        };
    }
    public static void ReloadConfiguration()
    {
        lock (reloadLock)
        {
            if (isReloading)
                return;
            isReloading = true;
        }

        try
        {
            //Logger.Log($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]: Laddar om konfiguration...");
            
            foreach (var watcher in inputWatchers)
            {
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();
            }
            inputWatchers.Clear();
            
            if (!ConfigValidator.IsValidConfigFile("Inställningar.xml"))
            {
                Logger.Log($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]: Ogiltig konfiguration, behåller gamla inställningar");
                return;
            }
            
            StartFileMonitor();

            //Logger.Log($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]: Konfiguration uppdaterad");
        }
        finally
        {
            lock (reloadLock)
            {
                isReloading = false;
            }
        }
    }
    public static void StartFileMonitor()
    {
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
                if (!validExt
                        .ContainsKey(configExt))
                    validExt[configExt] = new List<DirectoryRule>();
                validExt[configExt].Add(directoryConfig);
            }
        }

        var dirGroupedByInput = xmlSettings.Directories
                                                    .GroupBy(d => d.Input)
                                                    .ToList();

        foreach (var inputGroup in dirGroupedByInput)
        {
            var inputWatcher = new FileSystemWatcher(inputGroup.Key)
            {
                EnableRaisingEvents = true
            };

            inputWatcher.Created += async (sender, e) =>
            {
                await Task.Delay(100);
                var ext = Path.GetExtension(e.FullPath)
                    .ToLowerInvariant();
                if (validExt
                    .TryGetValue(ext, out var configs))
                {
                    foreach (var config in configs)
                        await fileMover
                            .HandleFiles(e.FullPath, config);
                }
            };

            inputWatcher.Changed += async (sender, e) =>
            {
                await Task.Delay(100);
                var ext = Path.GetExtension(e.FullPath)
                    .ToLowerInvariant();
                if (validExt
                    .TryGetValue(ext, out var configs))
                {
                    foreach (var config in configs)
                        await fileMover
                            .HandleFiles(e.FullPath, config);
                }
            };

            inputWatchers
                .Add(inputWatcher);
        }
    }
}