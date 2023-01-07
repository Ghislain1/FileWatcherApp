namespace Ghis.FileWatcherApp.Lib
{
    using Spectre.Console;
    using System.Runtime.InteropServices;
    using System.Threading.Channels;


    public class FileWatcherService : IFileWatcherService, IDisposable
    {
        private readonly IConsoleWriterService consoleWriterService;
        private FileSystemWatcher? fileSystemWatcher;
        public FileWatcherService(IConsoleWriterService consoleWriterService)
        {
            this.consoleWriterService = consoleWriterService;
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
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }
            var message = $"A file {e.Name} is opened in Word and the user saves some changes but does not close the file";
            var fileInfoDataModel = new FileInfoDataModel(e.FullPath,WatcherColorNameTypes.Orange, message);           
            this.consoleWriterService.PrintLn(fileInfoDataModel);
            // this.consoleWriterService.PrintLn($"Changed: {e.FullPath}");
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
 
            var fileInfoDataModel = new FileInfoDataModel(e.FullPath, WatcherColorNameTypes.Green);
            this.consoleWriterService.PrintLn(fileInfoDataModel);
           // this.consoleWriterService.PrintLn($"Created: {e.FullPath}");
        }

        private void OnDeleted(object sender, FileSystemEventArgs e)
        {
             
            var fileInfoDataModel = new FileInfoDataModel(e.FullPath, WatcherColorNameTypes.Red);
            this.consoleWriterService.PrintLn(fileInfoDataModel);
            //this.consoleWriterService.PrintLn($"Deleted: {e.FullPath}");
        }

            private void OnRenamed(object sender, RenamedEventArgs e)
        {
            var fileInfoDataModel = new FileInfoDataModel(e.FullPath, WatcherColorNameTypes.Yellow);
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
        }
    }
}