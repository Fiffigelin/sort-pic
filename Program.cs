using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

internal class Program
{
    private static void Main(string[] args)
    {
        string sourceDirectoryPath = @"C:\Path\To\Your\Pictures"; // Replace with your actual source path
        string destinationDirectoryPath = @"C:\Path\To\Destination\Folder"; // Replace with your desired destination path

        DirectoryInfo path = new DirectoryInfo(sourceDirectoryPath);

        try
        {
            var myFiles = (from file in path.GetFiles()
                           where file.Extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase) || file.Extension.Equals(".png", StringComparison.OrdinalIgnoreCase)
                           orderby file.LastWriteTime descending
                           select file).ToList();

            if (myFiles.Any())
            {
                Console.WriteLine("Matching files found:");
                CreateFolderAndSortPics(myFiles, destinationDirectoryPath);
            }
            else
            {
                Console.WriteLine("No matching files found.");
            }
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"Access error: {ex.Message}");
        }
        catch (IOException ex)
        {
            Console.WriteLine($"File error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        }
    }

    public static void CreateFolderAndSortPics(List<FileInfo> pictureList, string destinationDirectoryPath)
    {
        foreach (var file in pictureList)
        {
            DateTime creationDate = File.GetCreationTime(file.FullName);
            string destinationFolder = EnsureDirectoryStructure(destinationDirectoryPath, creationDate);

            string destinationPath = Path.Combine(destinationFolder, file.Name);
            File.Move(file.FullName, destinationPath);

            Console.WriteLine($"Moved {file.Name} to {destinationPath}");
        }
    }

    private static string EnsureDirectoryStructure(string baseFolder, DateTime creationDate)
    {
        string year = creationDate.ToString("yyyy");
        string month = creationDate.ToString("MM");

        string yearPath = Path.Combine(baseFolder, year);
        string monthPath = Path.Combine(yearPath, month);

        if (!Directory.Exists(yearPath))
        {
            Directory.CreateDirectory(yearPath);
        }
        if (!Directory.Exists(monthPath))
        {
            Directory.CreateDirectory(monthPath);
        }

        return monthPath;
    }
}