namespace Ghis.FileWatcherApp.Lib
{
    using Spectre.Console;


    public class FileWatcherService : IFileWatcherService, IDisposable
    {
        private readonly IConsoleWriterService consoleWriterService;
        public FileWatcherService(IConsoleWriterService consoleWriterService)
        {
            this.consoleWriterService = consoleWriterService;
        }
        private FileSystemWatcher? fileSystemWatcher;

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

            this.consoleWriterService.PrintLn($"Changed: {e.FullPath}");
        }

        private void OnCreated(object sender, FileSystemEventArgs e) => this.consoleWriterService.PrintLn($"Created: {e.FullPath}");

        private void OnDeleted(object sender, FileSystemEventArgs e) => this.consoleWriterService.PrintLn($"Deleted: {e.FullPath}");

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            this.consoleWriterService.PrintLn($"Renamed:",$"    Old: {e.OldFullPath}",$"    New: {e.FullPath}");
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