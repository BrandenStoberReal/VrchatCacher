// See https://aka.ms/new-console-template for more information
using Microsoft.VisualBasic.FileIO;
using System.Diagnostics;
using System.IO;
using System.Xml;

int sleepTime = 30; // 30 seconds
static void CopyFilesRecursively(string sourcePath, string targetPath)
{
    //Now Create all of the directories
    foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", System.IO.SearchOption.AllDirectories))
    {
        Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
    }

    //Copy all the files & Replaces any files with the same name
    foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", System.IO.SearchOption.AllDirectories))
    {
        if (!newPath.Contains("_lock"))
        {
            File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
        }
    }
}

while (true)
{
    string cache = @"%AppData%\..\LocalLow\VRChat\VRChat\Cache-WindowsPlayer";
    cache = Path.GetFullPath(Environment.ExpandEnvironmentVariables(cache));
    if (Directory.Exists(cache))
    {
        Console.WriteLine("Cache path found: " + cache);
        string backupPath = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "VRChat Cache Backup"));
        if (!Directory.Exists(backupPath))
        {
            Directory.CreateDirectory(backupPath);
        }
        Console.WriteLine("Backup path found: " + backupPath);

        if (File.Exists(Path.Combine(backupPath, "__info")))
        {
            Console.WriteLine("Outdated INFO file found, purging...");
            File.Delete(Path.Combine(backupPath, "__info"));
        }
        Console.WriteLine("Copying INFO file...");
        File.Copy(Path.Combine(cache, "__info"), Path.Combine(backupPath, "__info"));

        Stopwatch watch = new Stopwatch();
        watch.Start();
        foreach (string dir in Directory.GetDirectories(cache))
        {
            string dirName = new DirectoryInfo(dir).Name;
            if (Directory.Exists(Path.Combine(backupPath, dirName)))
            {
                continue;
            }
            Console.WriteLine($"Copying {dirName}...");
            CopyFilesRecursively(dir, Path.Combine(backupPath, dirName));
        }
        watch.Stop();

        Console.WriteLine($"Done! Elapsed time: {watch.ElapsedMilliseconds}ms");
        Console.WriteLine($"Waiting {sleepTime} second(s)...");
        Thread.Sleep(sleepTime * 1000);
    }
}