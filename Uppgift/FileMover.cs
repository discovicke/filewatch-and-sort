using Uppgift.Models;

namespace Uppgift;

public class FileMover
{
    public void HandleFiles(string fullPath, DirectorySetting setting)
    {
        string ext = Path.GetExtension(fullPath).ToLower();
        if (!setting.Types.Contains(ext))
            return;

        string fileName = Path.GetFileName(fullPath);
        string destPath = Path.Combine(setting.Output, fileName);

        const int maxAttempts = 10;
        const int delayMs = 500;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            try
            {
                if (!File.Exists(fullPath))
                {
                    Thread.Sleep(delayMs);
                    continue;
                }

                File.Move(fullPath, destPath, true);

                Logger.Log($"{fileName} flyttades till {setting.Name}");
                return;
            }
            catch (FileNotFoundException)
            {
                return;
            }
            catch (IOException)
            {
                Thread.Sleep(delayMs);
            }
            catch (UnauthorizedAccessException)
            {
                Thread.Sleep(delayMs); ;
            }
        }
    }
}