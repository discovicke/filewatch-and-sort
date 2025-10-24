using System.Diagnostics;

namespace Compilern
{
    [TestClass]
    public sealed class Körs
    {
        [TestMethod("Utan fel eller varningar")]
        public void Run()
        {
            var projectPath = @"..\..\..\..\Uppgift\Uppgift.csproj";

            var processStartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"build \"{projectPath}\" --no-incremental -warnaserror",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = processStartInfo };
            process.Start();

            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            var errors = output
                .Split('\n')
                .Where(x => x.Contains(": warning CS") || x.Contains(": error CS"))
                .Select(x =>
                {
                    var safe = x.Replace('\\', '/');
                    var start = safe.IndexOf("/Uppgift/");
                    if (start < 0) return x;

                    var trimmed = x.Substring(start + 1);

                    var pathEnds = trimmed.IndexOf(':');
                    var path = trimmed.Substring(0, pathEnds);
                    path = path.Replace('(', ':').Replace(',', ':').TrimEnd(')');

                    trimmed = trimmed.Substring(pathEnds + 1);
                    trimmed = trimmed.Substring(trimmed.IndexOf("CS"));

                    var warningEnds = trimmed.IndexOf(':');
                    var code = trimmed.Substring(0, warningEnds);
                    trimmed = trimmed.Substring(warningEnds + 1).TrimStart();

                    return $" • {path} (felkod {code}): {trimmed}";
                })
                .Distinct()
                .ToArray();


            if (errors.Length != 0)
            {
                Assert.Fail("Programmet kompileras med följande fel:\n" + string.Join("\n", errors));
            }
        }
    }
}