
using System.Net.Sockets;

namespace Programmet;

[TestClass]
public sealed class Kör
{
    private string testDirectory = "";
    private string inputDirectory = "";
    private string outputDirectory = "";
    private string[] extensions = [];
    private string[] includedFiles = [];
    private string[] excludedFiles = [];
    private string logFilePath = "";
    private string groupName = "";

    [TestInitialize]
    public void Setup()
    {
        testDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(testDirectory);

        inputDirectory = Path.Combine(testDirectory, "Input");
        Directory.CreateDirectory(inputDirectory);

        outputDirectory = Path.Combine(testDirectory, "Output");
        Directory.CreateDirectory(outputDirectory);

        if (File.Exists("Inställningar.xml"))
            File.Delete("Inställningar.xml");

        var random = new Random();
        logFilePath = allLogFiles[random.Next(0, allLogFiles.Length)];
        groupName = groupNames[random.Next(0, groupNames.Length)];

        var allExtensions = allFileNames.Select(x => Path.GetExtension(x)).Distinct().OrderBy(x => random.Next(1, 1000)).ToArray();
        extensions = allExtensions.Take(allExtensions.Length / 2).ToArray();

        includedFiles = allFileNames
            .Where(fileName => extensions.Any(ext => Path.GetExtension(fileName) == ext))
            .ToArray();
        excludedFiles = allFileNames.Except(includedFiles).ToArray();

        File.WriteAllText("Inställningar.xml",
            "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n" +
            $"<Settings>\r\n\t<Log>{logFilePath}</Log>\r\n\r\n" +
            "\t<Directory>\r\n" +
            $"\t\t<Name>{groupName}</Name>\r\n" +
            $"\t\t<Input>{inputDirectory}</Input>\r\n" +
            $"\t\t<Output>{outputDirectory}</Output>\r\n" +
            string.Join("\r\n", extensions.Select(x => $"\t\t<Type>{x}</Type>")) +
            "\t</Directory>\r\n" +
            "</Settings>");
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (Directory.Exists(testDirectory))
            Directory.Delete(testDirectory, true);
    }


    [TestMethod("Och flyttar de utpekade filerna")]
    public async Task ProgrammetFlyttarFiler()
    {
        var cancellation = new CancellationTokenSource();
        try
        {
            var main = RunUntilCancelled(cancellation.Token);

            await Task.Delay(300);


            for (int fileNumber = 0; fileNumber < 10; fileNumber++)
            {
                var fileName = GetRandomFileName();
                var inputPath = Path.Combine(inputDirectory, fileName);
                var outputPath = Path.Combine(outputDirectory, fileName);

                var textContent = GetRandomText();
                File.WriteAllText(inputPath, textContent);
                await Task.Delay(200);

                for (int retries = 0; retries < 10; retries++)
                {
                    if (File.Exists(outputPath)) break;
                    await Task.Delay(100 + (retries * 300));
                }
                if (!File.Exists(outputPath)) Assert.Fail(
                    $"\nFilen flyttade inte på sig\n" +
                    $"Fil som borde ha flyttats: {inputPath}\n" +
                    $"Där den borde ha dykt upp: {outputPath}"
                );

                var text = File.ReadAllText(outputPath);
                if (text != textContent)
                    Assert.Fail(
                    $"\nFilen flyttade inte korrekt\n" +
                    $"En fil med rätt namn dök upp: {outputPath}\n" +
                    $"Dens innehåll blev dock förändrat, borde ha varit: \"{textContent}\""
                );

                if (File.Exists(inputPath)) Assert.Fail(
                    $"\nFilen kopierades, originalet togs inte bort\n" +
                    $"Fil som borde ha tagits bort: {inputPath}"
                );

                // Cleanup for next test
                File.Delete(outputPath);
            }
        }
        finally
        {
            cancellation.Cancel();
        }
    }


    [TestMethod("Och loggar alla flyttade filer")]
    public async Task ProgrammetLoggar()
    {
        // Clear log if it exists
        File.WriteAllText(logFilePath, "");

        var expectedLogg = new List<string>();
        var cancellation = new CancellationTokenSource();
        try
        {
            var main = RunUntilCancelled(cancellation.Token);

            await Task.Delay(300);
            for (int i = 0; i < includedFiles.Length; i++)
            {
                var fileName = includedFiles[i];
                var inputPath = Path.Combine(inputDirectory, fileName);

                var textContent = GetRandomText();
                File.WriteAllText(inputPath, textContent);
                await Task.Delay(200);

                expectedLogg.Add(fileName);
            }

            await Task.Delay(1000);
            if (!File.Exists(logFilePath)) Assert.Fail(
                "\nDet skapades ingen logfil\n" +
                $"\"{logFilePath}\" borde ha skapats eftersom det anges i konfigurationsfilen");

            var logLines = File.ReadAllLines(logFilePath)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToArray();

            // Missing file names
            var missingLines = expectedLogg
                .Distinct()
                .Where(expected => !logLines.Any(actual => actual.Contains(expected)))
                .ToArray();

            if (missingLines.Length > 0) Assert.Fail(
                "\nLoggfilen innehöll inte alla rader som förväntades\n" +
                $"Det saknades {missingLines.Length} {(missingLines.Length == 1 ? "rad" : "rader")} som som innehåller följande:\n" +
                string.Join("\n", missingLines.Select(x => $"  {x}"))
            );

            // Missing group names
            missingLines = logLines
                .Where(actual => !actual.Contains(groupName))
                .ToArray();

            if (missingLines.Length > 0) Assert.Fail(
                "\nLoggfilen innehöll inte alla rader som förväntades\n" +
                $"Gruppnamnet saknades i {missingLines.Length} {(missingLines.Length == 1 ? "rad" : "rader")}\n" +
                $"\"{groupName}\" borde också ha skrivits ut bredvid filnamnet, eftersom detta är namnet i konfigurationsfilen"
            );
        }
        finally
        {
            cancellation.Cancel();
        }
    }

    [TestMethod("Och ignorerar de andra filerna")]
    public async Task ProgrammetInteFlyttarFiler()
    {
        var cancellation = new CancellationTokenSource();
        try
        {
            var main = RunUntilCancelled(cancellation.Token);

            await Task.Delay(300);

            var files = new List<string>();
            var contents = new List<string>();
            foreach (var file in excludedFiles)
            {
                var path = Path.Combine(inputDirectory, file);
                files.Add(path);
                var content = GetRandomText();
                contents.Add(content);
                File.WriteAllText(path, content);
                await Task.Delay(50);
            }

            // Wait 10 seconds
            await Task.Delay(10000);

            for (int i = 0; i < files.Count; i++)
            {
                var output = Path.Combine(outputDirectory, excludedFiles[i]);
                var extension = Path.GetExtension(excludedFiles[i])!;
                if (File.Exists(output)) Assert.Fail(
                    $"\nFilen flyttades\n" +
                    $"Filnamn: {excludedFiles[i]}\n" +
                    $"Eftersom denna har filändelse \"{extension}\" så borde den ha skippats av programmet\n" +
                    $"Konfigurationsfilen anger följande filändelser:\n" +
                    string.Join("\n", extensions.Select(x => $"  {x}"))
                );

                if (!File.Exists(files[i])) Assert.Fail(
                    $"\nFilen togs bort\n" +
                    $"Filnamn: {excludedFiles[i]}\n" +
                    $"Eftersom denna har filändelse \"{extension}\" så borde den ha skippats av programmet\n" +
                    $"Konfigurationsfilen anger följande filändelser:\n" +
                    string.Join("\n", extensions.Select(x => $"  {x}"))
                );
            }
        }
        finally
        {
            cancellation.Cancel();
        }
    }


    public async Task RunUntilCancelled(CancellationToken cancellationToken)
    {
        var task = Task.Run(() =>
        {

            var code = Program.Main();
            Assert.Fail("\nProgrammet borde inte ha avslutats");
        }, cancellationToken);

        try
        {
            await Task.WhenAny(task, Task.Delay(Timeout.Infinite, cancellationToken));
        }
        catch (TaskCanceledException)
        {
            // Expected if cancellationFToken was cancelled
        }
        catch (Exception e)
        {
            Assert.Fail($"\nProgrammet kraschade med följande fel:\n{e}");
        }



    }
    static string GetRandomText()
    {
        string[] options =
        {
            "The purple toaster refuses to negotiate with the spoon.",
            "Meanwhile, six ducks in overalls discuss tax reform.",
            "Please press any key to continue, but not THAT key.  ",
            "Error 418: I’m a teapot and I don’t do coffee.",
            "Uploading cat memes to the mainframe... 83 % complete.",
            "Do not trust the fridge after midnight. It hums secrets.",
            "The sky forgot its password.",
            "Rain logs out slowly.",
            "Even the wind needs a firmware update.",
            "I dream in pixels, and sometimes in bread crumbs.",
            "At Synergex™, we optimize paradigm-driven innovation to achieve scalable unicorn alignment.",
            "Our mission: disrupt sandwiches through cloud-based mayonnaise solutions.",
            "KPIs include vibes, synergy, and occasional jazz hands.",
            "Version 2.0: now with 37 % more buzzwords per byte.",
        };
        var random = new Random();
        return options[random.Next(0, options.Length)];
    }


    string GetRandomFileName()
    {
        var random = new Random();
        return includedFiles[random.Next(0, includedFiles.Length)];
    }

    static readonly string[] allLogFiles =
    {
        "system_boot_sequence.log",
        "error_report_42.log",
        "quantum_server_trace.log",
        "debugging_the_matrix.log",
        "unexpected_frog_activity.out",
        "coffee_machine_status.log",
        "ai_overlord_commands.log",
        "time_travel_patch_notes.log",
        "mildly_confused_penguin.trace",
        "kernel_panic_and_chill.log",
        "network_latency_adventures.log",
        "404_brain_not_found.log",
        "server_room_dramatics.out",
        "upload_failures_debug.log",
        "mystery_meat_analysis.log",
    };

    static readonly string[] groupNames =
    {
        "Viktiga grejer",
        "Top Secret",
        "Meme Arkivet",
        "Kaos",
        "Förvirring",
        "Husdjur",
        "Experimentella Projekt",
        "Magiska Textfiler",
        "Skräp som inte borde finnas",
        "Nattliga Äventyr",
        "Buggar",
    };

    static readonly string[] allFileNames =
        {
            "meeting_notes_final_FINAL_really.docx",
            "todo_maybe_later.txt",
            "budget_of_doom.xlsx",
            "notes_from_the_void.docx",
            "who_even_reads_these.txt",
            "very_important_document_definitely.pdf",
            "final_version_v32_revised.docx",
            "coffee_stains_and_hope.docx",
            "project_mystery_meat.txt",
            "this_file_will_self_destruct.docx",
            "log_of_confusion_002.txt",
            "error_but_make_it_artistic.md",
            "AI_was_here.docx",
            "system32_dont_delete_me.txt",
            "debugging_for_dummies.docx",
            "quantum_spaghetti_notes.txt",
            "user_input_unexpected_but_valid.txt",
            "runtime_meltdown.docx",
            "keyboard_smash_results.txt",
            "404_brain_not_found.docx",
            "frogs_in_space_the_thesis.pdf",
            "time_travel_expense_report.xlsx",
            "why_is_the_sun_loud.txt",
            "haiku_about_databases.docx",
            "emergency_planet_backup.docx",
            "reasons_the_toaster_is_plotting.txt",
            "how_to_pet_a_cloud.pdf",
            "unofficial_official_manifesto.docx",
            "the_secret_life_of_spreadsheets.xlsx",
            "existential_meeting_minutes.txt",
            "angry_potato_in_hd.png",
            "cat_screaming_at_salad_final_final.jpg",
            "banana_with_confidence.jpeg",
            "dramatic_duck_staring_into_distance.png",
            "enchanted_toaster_artwork_v2.webp",
            "mildly_confused_penguin.gif",
            "unidentified_flying_donut.bmp",
            "portrait_of_a_spreadsheet.tiff",
            "moon_with_wifi_signal.jpg",
            "serious_frog_headshot.png",
            "debug_screenshot_of_doom.png",
            "error404_but_make_it_aesthetic.jpeg",
            "ai_generated_pizza_abomination.webp",
            "bug_report_visual_v5.png",
            "system32_wallpaper.png",
            "quantum_cat_in_a_box.gif",
            "upload_me_not_final_final.png",
            "network_latency_diagram_final.jpg",
            "data_visualization_but_pretty.tiff",
            "low_resolution_pride.png",
            "team_photo_but_half_are_ai.jpg",
            "synergy_diagram_revised_final_final.png",
            "circle_back_chart_2.0.jpeg",
            "innovation_pipeline_concept_art.png",
            "pivot_strategy_visualization_v7.jpg",
            "growth_curve_with_confusion.png",
            "meeting_room_moodboard.webp",
            "brand_identity_but_confused.png",
            "budget_slide_screaming_inside.png",
            "vision_statement_poster_vfinal.jpg",
            "ambient_toaster_hum_loop.wav",
            "duck_in_a_tunnel_reverb_mix.mp3",
            "mysterious_pigeon_intro.ogg",
            "keyboard_solo_in_caffeine_minor.flac",
            "rain_on_windows_xp.wav",
            "soft_jazz_for_debugging.mp3",
            "lofi_for_late_deadlines.mp3",
            "echoes_of_the_meeting_room.wav",
            "dramatic_clicking_soundtrack.wav",
            "theme_from_the_void.mp3",
            "angry_duck_remix.mp3",
            "haunted_printer_noise.wav",
            "alien_cowbell_solo.wav",
            "elevator_music_for_spaceships.mp3",
            "guitar_solo_by_a_cat.flac",
            "muffin_song_in_D_minor.wav",
            "unnecessary_dramatic_intro.mp3",
            "melancholy_frog_ballad.wav",
            "background_noise_of_despair.ogg",
            "sneeze_remix_deluxe_edition.mp3",
        };
}
