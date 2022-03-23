using System;
using System.IO;
using System.Linq;

internal class BeatSaberSongExtractor
{
    private static void Main(string[] args)
    {
        string zippedLevelsDirectoryPath;
        string customLevelsDirectoryPath;
        if (args.Length != 2)
        {
            Console.WriteLine($"Invalid arguments supplied. Please use executable like so: {System.Reflection.Assembly.GetCallingAssembly().GetName().Name} <{nameof(zippedLevelsDirectoryPath)}> <{nameof(customLevelsDirectoryPath)}>");
            return;
        }

        zippedLevelsDirectoryPath = args[0];
        customLevelsDirectoryPath = args[1];
        DirectoryInfo customLevelsDirectoryInfo;
        try
        {
            customLevelsDirectoryInfo = new DirectoryInfo(customLevelsDirectoryPath);
            Console.WriteLine($"Loaded directory info for {nameof(customLevelsDirectoryPath)}...");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to get info for {nameof(customLevelsDirectoryPath)} {customLevelsDirectoryPath} : {ex.Message}");
            return;
        }

        ClearDirectory(customLevelsDirectoryInfo);
        UnzipCustomSongDirectories(zippedLevelsDirectoryPath, customLevelsDirectoryPath);
        AlphabetizeDirectoryNames(customLevelsDirectoryInfo);

        Console.WriteLine("Success. Press any key to exit...");
        Console.ReadKey();
    }

    private static void ClearDirectory(DirectoryInfo dir)
    {
        Console.WriteLine($"Removing all directories / files in {dir.Name} directory...");
        foreach (var file in dir.GetFiles())
        {
            file.Delete();
        }

        foreach (var d in dir.GetDirectories())
        {
            d.Delete(true);
        }
    }

    private static void UnzipCustomSongDirectories(string inputPath, string outputDirectoryPath)
    {
        foreach (string zipPath in Directory.GetFiles(inputPath))
        {
            string filename = Path.GetFileNameWithoutExtension(zipPath);
            string path = Path.Combine(outputDirectoryPath, filename);
            if (!Directory.Exists(path))
            {
                Console.WriteLine($"Creating {filename}");
                try
                {
                    System.IO.Compression.ZipFile.ExtractToDirectory(zipPath, path);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to extract {zipPath} : {ex.Message}");
                }
            }
        }
    }

    private static void AlphabetizeDirectoryNames(DirectoryInfo dir)
    {
        int padAmount = dir.GetDirectories().Length;
        int count = 1;
        Console.WriteLine("Reordering directories by level name alphabetically...");
        foreach (var extractInfo in dir.GetDirectories().OrderBy(e => e.Name.Split("(")[1]))
        {
            Directory.Move(extractInfo.FullName, $"{dir.FullName}/{count.ToString().PadLeft(padAmount, '0')} - {extractInfo.Name}");
            count++;
        }

        Console.WriteLine($"{count} levels processed.");
    }
}