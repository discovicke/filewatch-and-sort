using Uppgift.Models;

namespace Uppgift;

public class FileMover
{
    public async Task HandleFiles(string sourcePath, DirectorySetting setting)
    {
        var ext = Path.GetExtension(sourcePath).ToLowerInvariant();
        if (!setting.Types.Contains(ext))
            return;

        var fileName = Path.GetFileName(sourcePath);
        var destPath = Path.Combine(setting.Output, fileName);

        const int maxAttempts = 10;
        const int delayMs = 400;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            try
            {
                if (!File.Exists(sourcePath))
                {
                    await Task.Delay(delayMs);
                    continue;
                }
                
                using (var stream = File.Open(sourcePath, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    
                }

                File.Move(sourcePath, destPath, true);
                Logger.Log($"[{DateTime.Now}]: {fileName} flyttades till {setting.Name}");
                Console.WriteLine($"[{DateTime.Now}]: {fileName} flyttades till {setting.Name}");
                return;
            }
            catch (FileNotFoundException)
            {
                return;
            }
            catch (IOException)
            {
                await Task.Delay(delayMs);
            }
            catch (UnauthorizedAccessException)
            {
                await Task.Delay(delayMs);
            }
        }
    }
}
