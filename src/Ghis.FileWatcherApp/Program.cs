// <copyright company="Ghislain One Inc.">
//  Copyright (c) GhislainOne
//  This computer program includes confidential, proprietary
//  information and is a trade secret of GhislainOne. All use,
//  disclosure, or reproduction is prohibited unless authorized in
//  writing by an officer of Ghis. All Rights Reserved.
// </copyright>


namespace Ghis.FileWatcherApp;

using Ghis.FileWatcherApp.Lib;
using System.Runtime.CompilerServices;
using Ghis.FileWatcherApp.Lib.Locking;


public class Program
{
    private static string fileFilter = @"\.txt)|\.json\";
    private static string watcherPath = @"C:\Users\Zoe\Documents\SubMain";
    private static   string watcherInputPath = Path.Combine(watcherPath, "WatchInput");
    private static readonly string watcherOutputPath = Path.Combine(watcherPath, "HtmlOutput");
    static void Main()
    {
        var sucess = false;

        string? directoryPath;
        do
        {
            Console.Write($"Enter a Directory Path to watch > ");
            directoryPath = Console.ReadLine();
            if (string.IsNullOrEmpty(directoryPath))
            {
                // Set default
                directoryPath = watcherInputPath;
            }
            if (!string.IsNullOrWhiteSpace(directoryPath) && Directory.Exists(directoryPath))
            {
                Console.WriteLine($"You entered the Directory path: {directoryPath}");
                Console.WriteLine("");
                sucess = true;
            }
            else
            {
                Console.WriteLine("No correct Directory path entered. ");
                Console.WriteLine("");
            }

        } while (!sucess);

        watcherInputPath = directoryPath!;
        InitialiseApplication();
        RunFolderWatcher(watcherInputPath);

    }
    private static void InitialiseApplication()
    {
        EnsureDirectoryExists(watcherInputPath);
        EnsureDirectoryExists(watcherOutputPath);

    }

    private static void EnsureDirectoryExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    private static void OnFileInfoDataModelChanged(WatcherChangeTypes watcherChangeTypes, FileInfoDataModel fileInfoDataModel)
    {
        var writerService = new HtmlWriterService(watcherOutputPath);
        writerService.PrintLn(fileInfoDataModel);

        var consoleWriterService = new StandardConsoleWriterService();
        consoleWriterService.PrintLn(fileInfoDataModel);

    }
    private static void RunFolderWatcher(string directoryPath)
    {
        // var consoleWriterService = new StandardConsoleWriterService();

        var lockFileService = new LockFileService();
        using (var fileWatcherService = new FileWatcherService(lockFileService))
        {
            fileWatcherService.StartWatch(directoryPath, OnFileInfoDataModelChanged, true, fileFilter);

            Console.Read();
            //Make an infinite loop till 'q' is pressed.  
            while (Console.Read() != 'q') ;

        }



    }




    private static void OnChanged(object sender, FileSystemEventArgs e)
    {


    }







}