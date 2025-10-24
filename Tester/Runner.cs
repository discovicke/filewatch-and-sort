namespace Tester
{
    internal static class Runner
    {
        static readonly object locker = new();
        const string settingsPath = "Inställningar.xml";

        public static int Run(string? settings, int seconds = 0)
        {
            lock (locker)
            {
                if (File.Exists(settingsPath)) File.Delete(settingsPath);
                if (settings != null) File.WriteAllText(settingsPath, settings);

                try
                {
                    if (seconds <= 0)
                    {
                        return Program.Main();
                    }
                    else
                    {
                        int code = 0;
                        var task = Task.Run(() => code = Program.Main());
                        _ = task.Wait(seconds * 1000);
                        return code;
                    }
                }
                catch (Exception e)
                {
                    Assert.Fail("\nProgrammet kraschade med följande fel:\n" + e);
                    return -1;
                }
            }
        }
    }
}
