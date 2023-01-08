namespace Ghis.FileWatcherApp.Lib
{
    using Microsoft.VisualBasic;
    using Spectre.Console;
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Text.RegularExpressions;
    using System.Threading.Channels;
    using Ghis.FileWatcherApp.Lib.Locking;


    public class FileWatcherService : IFileWatcherService, IDisposable
    {
        private readonly IConsoleWriterService consoleWriterService;
        private readonly ILockFileService lockFileService;
        private FileSystemWatcher? fileSystemWatcher;
        public FileWatcherService(IConsoleWriterService consoleWriterService, ILockFileService lockFileService)
        {
            this.consoleWriterService = consoleWriterService;
            this. lockFileService=   lockFileService;
        }


        public void StartWatch(string fullPath, bool includeSubdirectories = true, string filter = "*.txt")
        {

            this.fileSystemWatcher = new FileSystemWatcher(fullPath);
            fileSystemWatcher.NotifyFilter = NotifyFilters.Attributes
                           | NotifyFilters.CreationTime
                           | NotifyFilters.DirectoryName
                           | NotifyFilters.FileName
                           | NotifyFilters.LastAccess
                           | NotifyFilters.LastWrite
                           | NotifyFilters.Security
                           | NotifyFilters.Size;

            fileSystemWatcher.Changed += OnChanged;
            fileSystemWatcher.Created += OnCreated;
            fileSystemWatcher.Deleted += OnDeleted;
            fileSystemWatcher.Renamed += OnRenamed;
            fileSystemWatcher.Error += OnError;
            fileSystemWatcher.Filter = filter;
            fileSystemWatcher.IncludeSubdirectories = includeSubdirectories;
            fileSystemWatcher.EnableRaisingEvents = true;
            Console.WriteLine($"Monitoring ({fullPath}): ");
        }


 

        private async void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }
            string message = string.Empty;

          
            var isFileLock= await this.  lockFileService.IsFileLockedAsync(DateTime.Now, e.FullPath);

            if (isFileLock)
            {
                message = $"A file {e.Name} is opened in Word and the user saves some changes but does not close the file";
            }
            else
            {
                message = "A file {e.Name} is ready";
            }

            var fileInfoDataModel = new FileInfoDataModel(e.FullPath, WatcherColorNameTypes.Orange, message, "Changed:");
            this.consoleWriterService.PrintLn(fileInfoDataModel);


            // this.consoleWriterService.PrintLn($"Changed: {e.FullPath}");
        }
     
        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            var fileInfoDataModel = new FileInfoDataModel(e.FullPath, WatcherColorNameTypes.Green, "", "Created:");
            this.consoleWriterService.PrintLn(fileInfoDataModel);
            // this.consoleWriterService.PrintLn($"Created: {e.FullPath}");
        }

        private void OnDeleted(object sender, FileSystemEventArgs e)
        {

            var fileInfoDataModel = new FileInfoDataModel(e.FullPath, WatcherColorNameTypes.Red, "", "Deleted:");
            this.consoleWriterService.PrintLn(fileInfoDataModel);
            //this.consoleWriterService.PrintLn($"Deleted: {e.FullPath}");
        }

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            var fileInfoDataModel = new FileInfoDataModel(e.FullPath, WatcherColorNameTypes.Yellow, "", "Renamed:");
            fileInfoDataModel.OldFullPath = e.OldFullPath;
            // this.consoleWriterService.PrintLn($"Renamed:",$"Old: {e.OldFullPath}",$"New: {e.FullPath}");
            this.consoleWriterService.PrintLn(fileInfoDataModel);
        }

        private void OnError(object sender, ErrorEventArgs e) => this.consoleWriterService.PrintException(e.GetException());

        public void Dispose()
        {
            if (this.fileSystemWatcher is null)
            {
                return;
            }
            this.fileSystemWatcher.Dispose();
            this.lockFileService.CleanUpAsync();
        }
    }
}