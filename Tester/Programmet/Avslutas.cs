using Tester;

namespace Programmet;

[TestClass]
public sealed class Avslutas
{
    [TestMethod("Om konfigurationsfilen inte finns")]
    public void OmInteFinns()
    {
        CreateDirs();
        var code = Runner.Run(null);
        if (code != 1) Assert.Fail("\nProgrammet borde avslutas med kod 1");
        RemoveDirs();
    }

    [TestMethod("Om <Settings> inte finns i konfigurationsfilen")]
    public void OmSettingsInteFinns()
    {
        CreateDirs();
        var settings = GenerateXml(XMLData.Settings, ".jpg", ".png");
        var code = Runner.Run(settings, 10);
        if (code != 1) Assert.Fail("\nProgrammet borde avslutas med kod 1");
        RemoveDirs();
    }

    [TestMethod("Om <Log> inte finns i konfigurationsfilen")]
    public void OmLogInteFinns()
    {
        CreateDirs();
        var settings = GenerateXml(XMLData.Log, ".jpg", ".png");
        var code = Runner.Run(settings, 10);
        if (code != 1) Assert.Fail("\nProgrammet borde avslutas med kod 1");
        RemoveDirs();
    }

    [TestMethod("Om <Directory> inte finns i konfigurationsfilen")]
    public void OmDirectoryInteFinns()
    {
        CreateDirs();
        var settings = GenerateXml(XMLData.Directory, ".jpg", ".png");
        var code = Runner.Run(settings, 10);
        if (code != 1) Assert.Fail("\nProgrammet borde avslutas med kod 1");
        RemoveDirs();
    }

    [TestMethod("Om <Name> inte finns i konfigurationsfilen")]
    public void OmNameInteFinns()
    {
        CreateDirs();
        var settings = GenerateXml(XMLData.Name, ".jpg", ".png");
        var code = Runner.Run(settings, 10);
        if (code != 1) Assert.Fail("\nProgrammet borde avslutas med kod 1");
        RemoveDirs();
    }

    [TestMethod("Om <Input> inte finns i konfigurationsfilen")]
    public void OmInputInteFinns()
    {
        CreateDirs();
        var settings = GenerateXml(XMLData.Input, ".jpg", ".png");
        var code = Runner.Run(settings, 10);
        if (code != 1) Assert.Fail("\nProgrammet borde avslutas med kod 1");
        RemoveDirs();
    }

    [TestMethod("Om <Output> inte finns i konfigurationsfilen")]
    public void OmOutputInteFinns()
    {
        CreateDirs();
        var settings = GenerateXml(XMLData.Output, ".jpg", ".png");
        var code = Runner.Run(settings, 10);
        if (code != 1) Assert.Fail("\nProgrammet borde avslutas med kod 1");
        RemoveDirs();
    }

    [TestMethod("Om <Type> inte finns i konfigurationsfilen")]
    public void OmTypeInteFinns()
    {
        CreateDirs();
        var settings = GenerateXml(XMLData.Type);
        var code = Runner.Run(settings, 10);
        if (code != 1) Assert.Fail("\nProgrammet borde avslutas med kod 1");
        RemoveDirs();
    }

    [TestMethod("Om <Input> inte är en giltig sökväg")]
    public void OmInputÄrOgiltig()
    {
        CreateDirs();
        foreach (var file in randomDirectoryPaths)
        {
            var settings = GenerateXml(XMLData.None, ".jpg", ".png").Replace(">Downloads<", $">{file}<");
            var code = Runner.Run(settings, 10);
            if (code != 1) Assert.Fail("\nProgrammet borde avslutas med kod 1");
        }
        RemoveDirs();
    }

    [TestMethod("Om <Output> inte är en giltig sökväg")]
    public void OmOutputÄrOgiltig()
    {
        CreateDirs();
        foreach (var file in randomDirectoryPaths)
        {
            var settings = GenerateXml(XMLData.None, ".jpg", ".png").Replace(">Downloads/Bilder<", $">{file}<");
            var code = Runner.Run(settings, 10);
            if (code != 1) Assert.Fail("\nProgrammet borde avslutas med kod 1");
        }
        RemoveDirs();
    }

    [TestMethod("Om <Log> inte pekar på en giltig mapp")]
    public void OmLogÄrOgiltig()
    {
        CreateDirs();
        foreach (var file in randomFilePaths)
        {
            var settings = GenerateXml(XMLData.None, ".jpg", ".png").Replace(">log.txt<", $">{file}<");
            var code = Runner.Run(settings, 10);
            if (code != 1) Assert.Fail($"\nProgrammet borde avslutas med kod 1 för logfil \"{file}\"");
        }
        RemoveDirs();
    }


    static void CreateDirs()
    {
        if (!Directory.Exists("Downloads"))
            Directory.CreateDirectory("Downloads");

        if (!Directory.Exists("Downloads/Bilder"))
            Directory.CreateDirectory("Downloads/Bilder");
    }

    static void RemoveDirs()
    {
        if (Directory.Exists("Downloads"))
            Directory.Delete("Downloads", true);
    }


    static string GenerateXml(XMLData exceptFlags, params string[] types)
    {
        var flags = XMLData.All & ~exceptFlags;
        var writer = new StringWriter();
        writer.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
        if (flags.HasFlag(XMLData.Settings)) writer.WriteLine("<Settings>");
        else writer.WriteLine("<InteRättElement>");

        if (flags.HasFlag(XMLData.Log)) writer.WriteLine("\t<Log>log.txt</Log>");

        if (flags.HasFlag(XMLData.Directory)) writer.WriteLine("\t<Directory>");
        else writer.WriteLine("\t<InteRättElement>");

        if (flags.HasFlag(XMLData.Name)) writer.WriteLine("\t\t<Name>Bilder</Name>");
        if (flags.HasFlag(XMLData.Input)) writer.WriteLine("\t\t<Input>Downloads</Input>");
        if (flags.HasFlag(XMLData.Output)) writer.WriteLine("\t\t<Output>Downloads/Bilder</Output>");
        if (flags.HasFlag(XMLData.Type))
        {
            foreach (var type in types)
            {
                writer.WriteLine($"\t\t<Type>{type}</Type>");
            }
        }
        if (flags.HasFlag(XMLData.Directory)) writer.WriteLine("\t</Directory>");
        else writer.WriteLine("\t</InteRättElement>");

        if (flags.HasFlag(XMLData.Settings)) writer.WriteLine("</Settings>");
        else writer.WriteLine("</InteRättElement>");
        return writer.ToString();
    }

    [Flags]
    enum XMLData
    {
        None = 0,
        Settings = 1,
        Log = 2,
        Directory = 4,
        Name = 8,
        Input = 16,
        Output = 32,
        Type = 64,
        All = Settings | Log | Directory | Name | Input | Output | Type,
    }


    static readonly string[] randomPaths = {
        "./funny-folder/banana_suit/party.txt",
        "../../../../whoopsie_daisy/too_far/nowhere",
        "tmp\\stinky\\socks\\README.md",
        "build/output\\CON\\not-a-device",
        "./.hidden_but_proud/.DS_Store-ish",
        "cache/ಠ_ಠ/weird_icon.png",
        "local/bin/oops/execute.sh",
        "logs\\null\\0placeholder.log",
        "snapshots/space at end /oops.txt",
        "temp/trailing.dot./file",
        "./weird:colon:name/file.txt",
        "scripts\\tab\\tname\\run.bat",
        "samples/emoji-🚀-launch/notes.md",
        "./almost-normal/.. /evil.txt",
        "user/COM1/report.pdf",
        "assets\\AUX\\image.png",
        "docs/what?nope/README",
        "src\\this|that\\main.c",
        "./relative/with\\0nul",
        "../mystery/line\\nbreak.txt",
        "examples/CR_carriage\\rfile",
        "./mix/separators\\and/slashes.txt",
        "modules/PRN.txt/log",
        "./colon-leading:thing/config.yml",
        "notes\\trailing_space \\doc.txt",
        "./dot_at_end././file",
        "../..\\..\\deep\\dive.txt",
        "tmp/quote\"mark/quotes.txt",
        "weird_names/pipe|pipe|pipe/data.json",
        "./.weird_resourcefork/._resourcefork",
        "local/ICON\\r/iconfile",
        "./tiny\\x01control/bug.txt",
        "misc/ttylol/OMG!.txt",
        "music\\song?.mp3",
        "./url_like/http://not-a-url",
        "package\\name\\with*star\\pkg.json",
        "./emoji_combo/🤖+🦄/readme.md",
        "temp/..\\..\\escape\\huh",
        "sandbox/spacey␠name/file",
        "backup/leadingcolon:whoami.bak",
        "./file_with_newline_\\n_in_name",
        "./trailing-dot.\\also-weird",
        "proj/..//..//normalize_me",
        "./relative\\mixed/separators\\odd.txt",
        "stuff/NULL\\0BYTE/test.bin",
        "data/..\\CON\\nested.txt",
        "./parent_traverse/../../../outside",
        "examples/semicolon;evil/script.sh",
        "./name_with_tab\\tand_more.txt",
        "./reserved/LPT1/stuff.dat",
        "test/emoji-🐙-octopus/notes",
        "./weird_unicode/𝌆_glitch/file.md",
        "tmp/space-leading /file.txt",
        "./multi..dots.../weird",
        "folder\\backslash\\only\\file.txt",
        "./almost/url_like\\file:///notfile"
    };
    static readonly string[] randomDirectoryPaths = randomPaths
        .Where(x => string.IsNullOrEmpty(Path.GetExtension(x)))
        .ToArray();
    static readonly string[] randomFilePaths = randomPaths
        .Where(x => !string.IsNullOrEmpty(Path.GetExtension(x)))
        .ToArray();
}
