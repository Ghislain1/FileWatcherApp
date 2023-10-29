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
    private static readonly string watcherPath = @"C:\Users\Zoe\Documents\SubMain";
    private static readonly string watcherInputPath = Path.Combine(watcherPath, "WatchInput");
    private static readonly string watcherOutputPath = Path.Combine(watcherPath, "HtmlOutput");
    static void Main()
    {
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
        using (var fileWatcherService = new FileWatcherService( lockFileService))
        {
            fileWatcherService.StartWatch(directoryPath, OnFileInfoDataModelChanged, true, "*.txt");
            
            Console.Read();
            //Make an infinite loop till 'q' is pressed.  
            while (Console.Read() != 'q') ;

        }

        //using (FileSystemWatcher watcher = new FileSystemWatcher())
        //{
        //    watcher.Path = directoryPath;

        //    watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;

        //    watcher.Filter = "*.txt";

        //    watcher.Created += OnChanged;

        //    watcher.EnableRaisingEvents = true;

        //    Console.WriteLine("Press 'q' to quit the application");

        //    while (Console.Read() != 'q') ;
        //}

    }




    private static void OnChanged(object sender, FileSystemEventArgs e)
    {
    

    }

  
   




}