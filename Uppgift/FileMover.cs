using Uppgift.Models;
namespace Uppgift;

public class FileMover
{
    private void HanteraNyFil(string fullPath, DirectorySetting setting)
    {
        string ext = Path.GetExtension(fullPath).ToLower();

        if (!setting.Types.Contains(ext))
            return; // ignorera filer som inte matchar

        string fileName = Path.GetFileName(fullPath);
        string destPath = Path.Combine(setting.Output, fileName);

        File.Move(fullPath, destPath, true);
        Logger.Log($"{fileName} flyttades till {setting.Name}");
    }
}