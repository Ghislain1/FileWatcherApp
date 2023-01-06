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

public class MyClassCS
{
    static void Main()
    {
        var pathFolder = @"C:\Users\Zoe\Documents\SubMain";
       // using var watcher = new FileSystemWatcher();
        var fileWatcherService = new FileWatcherService();
        fileWatcherService.StartWatch(pathFolder);

        Console.WriteLine("Press enter to exit.");
        Console.ReadLine();
    }

  

 
}