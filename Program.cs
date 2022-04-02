using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

internal class BeatSaberSongExtractor
{
    private static readonly string _userProfileDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    private static readonly string _playerDataFilePath = Path.Combine(_userProfileDirectoryPath, @"LocalLow\Hyperbolic Magnetism\Beat Saber\PlayerData.dat");
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

        SyncLocalPlayerData(_playerDataFilePath);
        ClearDirectory(customLevelsDirectoryInfo);
        UnzipCustomSongDirectories(zippedLevelsDirectoryPath, customLevelsDirectoryPath);
        AlphabetizeDirectoryNames(customLevelsDirectoryInfo);

        Console.WriteLine("Success. Press any key to exit...");
        Console.ReadKey();
    }

    private static void SyncLocalPlayerData(string playerDataFilePath)
    {
        if (!File.Exists(playerDataFilePath))
        {
            Console.WriteLine($"{nameof(playerDataFilePath)} file does not exist at:  {playerDataFilePath}");
            return;
        }

        string fileContents;
        try
        {
            fileContents = File.ReadAllText(playerDataFilePath);
            Console.WriteLine($"Loaded file contents for {nameof(playerDataFilePath)}...");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to get file contents for {nameof(playerDataFilePath)} : {ex.Message}");
            return;
        }

        RootObject playerDataRootObject;
        try
        {
            playerDataRootObject = JsonSerializer.Deserialize<RootObject>(fileContents);
            Console.WriteLine($"Deserialzied data from {nameof(playerDataFilePath)}...");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to deserialize data from {nameof(playerDataFilePath)} : {ex.Message}");
            return;
        }

        var localPlayer = playerDataRootObject.localPlayers[0];

        Console.WriteLine($"Syncing local highscores...");
        // TODO: Get all custom song names and map to localPlayer.levelsStatsData
        //foreach (var levelStatData in localPlayer.levelsStatsData)
        //{

        //}

        Console.WriteLine($"Syncing favorites...");
        var newFavorites = new List<string>();
        foreach (string favoriteLevelId in localPlayer.favoritesLevelIds)
        {
            if (IsReorderedLevelId(favoriteLevelId))
            {
                // TODO: Map these to new level ids
                Console.WriteLine(favoriteLevelId);
            }
            else
            {
                newFavorites.Add(favoriteLevelId);
            }
        }
    }

    private static bool IsReorderedLevelId(string levelId)
    {
        return Regex.IsMatch(levelId, @"^custom_level_(\d){1,4}\s-\s.*");
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
                    System.IO.Compression.ZipFile.ExtractToDirectory(zipPath, path, true);
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
        int padAmount = (int)Math.Floor(Math.Log10(dir.GetDirectories().Length) + 1);
        int count = 0;
        Console.WriteLine("Reordering directories by level name alphabetically...");
        foreach (var extractInfo in dir.GetDirectories().OrderBy(e => e.Name.Split("(")[1]))
        {
            try
            {
                Directory.Move(extractInfo.FullName, $"{dir.FullName}/{count.ToString().PadLeft(padAmount, '0')} - {extractInfo.Name}");
                count++;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to move {extractInfo.Name} : {e.Message}");
            }
        }

        Console.WriteLine($"{count} levels processed.");
    }
}