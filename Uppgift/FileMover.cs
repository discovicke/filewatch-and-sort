using Uppgift.Models;

namespace Uppgift;

public class FileMover
{
    public async Task HandleFiles(string sourcePath, DirectoryRule setting)
    {
        var ext = Path.GetExtension(sourcePath).ToLowerInvariant();
        if (!setting.Types.Contains(ext))
            return;

        var fileName = Path.GetFileName(sourcePath);
        var destPath = Path.Combine(setting.Output, fileName);

        const int maxAttempts = 10;
        const int delayMs = 50;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            try
            {
                if (!File.Exists(sourcePath))
                {
                    //Console.WriteLine(DateTime.Now + " DEBUG: FILEN FINNS INTE I HANDLE FILES... VÄNTAR");
                    await Task.Delay(delayMs);
                    continue;
                }
                
                using (var stream = File.Open(sourcePath, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    
                }

                File.Move(sourcePath, destPath, true);
                var logMessage = $"{fileName} {setting.Name}";
                var consoleMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]: {fileName} flyttades till {setting.Name}";

                Logger.Log(logMessage);
                Console.WriteLine(consoleMessage);
                return;
            }
            catch (FileNotFoundException)
            {
                //Console.WriteLine(DateTime.Now + " DEBUG: FILEN FINNS INTE");
                return;
            }
            catch (IOException)
            {
                //Console.WriteLine(DateTime.Now + " DEBUG: IOEXCEPTION, FÖRSÖKER IGEN");

                await Task.Delay(delayMs);
            }
            catch (UnauthorizedAccessException)
            {
                //Console.WriteLine(DateTime.Now + " DEBUG: NO ACCESS");

                await Task.Delay(delayMs);
            }
        }
    }
    
    public async Task ExistingFiles(DirectoryRule setting)
    {
        if (!Directory.Exists(setting.Input))
        {
            Console.WriteLine(DateTime.Now + " DEBUG: INPUT-MAPPEN FINNS INTE");
            Logger.Log($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]: Input-mapp finns inte: {setting.Input}");
            return;
        }

        try
        {
            var files = Directory.GetFiles(setting.Input);
            Console.WriteLine($"{DateTime.Now} DEBUG: {files.Length} befintliga filer");
            /*Logger.Log($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]: Kontrollerar {files.Length} befintliga filer")*/;

            foreach (var file in files)
            {
                await HandleFiles(file, setting);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(DateTime.Now + " DEBUG: FEL VID UPPSTARTSBEARBETNING: {ex.Message}");
            Logger.Log($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]: Fel vid uppstartsbearbetning: {ex.Message}");
        }
    }
}