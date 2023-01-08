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
public class MyClassCS
{
    static void Main()
    {
        var pathFolder = @"C:\Users\Zoe\Documents\SubMain";
        var consoleWriterService = new StandardConsoleWriterService();    
        var  lockFileService  = new LockFileService();
        // var consoleWriterService = new AnsiConsoleWriterService();
        using (var fileWatcherService = new FileWatcherService(consoleWriterService, lockFileService))
        {
            fileWatcherService.StartWatch(pathFolder, true, "*.txt");

           // Console.WriteLine("\nPress \'q\' to exit.");
             //Console.WriteLine(" ");
            Console.Read();
            //Make an infinite loop till 'q' is pressed.  
             while (Console.Read() != 'q') ;
          
        }

    }




}