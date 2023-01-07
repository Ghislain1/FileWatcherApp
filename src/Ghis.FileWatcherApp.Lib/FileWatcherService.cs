namespace Ghis.FileWatcherApp.Lib
{
    using System.Drawing;

    public class FileWatcherService : IFileWatcherService, IDisposable
    {
        private FileSystemWatcher fileSystemWatcher;
        public void StartWatch(string fullPath, bool includeSubdirectories = true, string filter = "*.txt")
        {
            Console.WriteLine($"StartWatch: {fullPath}");
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
            Console.WriteLine($"Changed: {e.FullPath}", Color.Yellow);
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            
            Console.WriteLine($"Created: {e.FullPath}", Color.Green);
        }

        private void OnDeleted(object sender, FileSystemEventArgs e) =>
            Console.WriteLine($"Deleted: {e.FullPath}", Color.Red);

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            Console.WriteLine($"Renamed:");
            Console.WriteLine($"    Old: {e.OldFullPath}", Color.Orange);
            Console.WriteLine($"    New: {e.FullPath}",Color.Orange);
        }

        private void OnError(object sender, ErrorEventArgs e) =>
            PrintException(e.GetException());

        private void PrintException(Exception? ex)
        {
            if (ex is null)
            {
                return;
            }
            
                Console.WriteLine($"Message: {ex.Message}");
                Console.WriteLine("Stacktrace:");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine();
                PrintException(ex.InnerException);
            
        }

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