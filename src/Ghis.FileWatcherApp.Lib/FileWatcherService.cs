namespace Ghis.FileWatcherApp.Lib;

using Microsoft.VisualBasic;
using Spectre.Console;
using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Channels;
using Ghis.FileWatcherApp.Lib.Locking;
using System.Threading;


public class FileWatcherService : IFileWatcherService, IDisposable
{
    private readonly ILockFileService lockFileService;
    private string  fileFilter = @"*.json";
    private SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1);
    private FileSystemWatcher? fileSystemWatcher;
    private Action<WatcherChangeTypes, FileInfoDataModel>? onFileInfoDataModelChanged;
    public FileWatcherService(ILockFileService lockFileService)
    {
        this.lockFileService = lockFileService;
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
        fileSystemWatcher.Filter = "";// All
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
        // get the file's extension 
        string strFileExt = Path.GetExtension(e.FullPath);

        // filter file types 
        //if (Regex.IsMatch(strFileExt,this.fileFilter, RegexOptions.IgnoreCase))
        //{
        //    return;
        //}
        if (!this.fileFilter.Contains(strFileExt))
        {
            return;
        }

        await semaphoreSlim.WaitAsync();
      


        var isFileLock = await this.lockFileService.IsFileLockedAsync(DateTime.Now, e.FullPath);
        var message = string.Empty;
        if (isFileLock)
        {
            message = $"A file {e.Name} is opened in Word and the user saves some changes but does not close the file";
        }
        else
        {
            message = "A file {e.Name} is ready";
        }

        var fileInfoDataModel = new FileInfoDataModel(e.FullPath, WatcherColorNameTypes.Orange, message, "Changed:");

        this.onFileInfoDataModelChanged?.Invoke(e.ChangeType, fileInfoDataModel);
        semaphoreSlim.Release();

    }

    private void OnCreated(object sender, FileSystemEventArgs e)
    {
        var fileInfoDataModel = new FileInfoDataModel(e.FullPath, WatcherColorNameTypes.Green, "", "Created:");
        this.onFileInfoDataModelChanged?.Invoke(e.ChangeType, fileInfoDataModel);
    }

    private void OnDeleted(object sender, FileSystemEventArgs e)
    {

        var fileInfoDataModel = new FileInfoDataModel(e.FullPath, WatcherColorNameTypes.Red, "", "Deleted:");
        this.onFileInfoDataModelChanged?.Invoke(e.ChangeType, fileInfoDataModel);
    }

    private void OnRenamed(object sender, RenamedEventArgs e)
    {
        var fileInfoDataModel = new FileInfoDataModel(e.FullPath, WatcherColorNameTypes.Yellow, "", "Renamed:");
        fileInfoDataModel.OldFullPath = e.OldFullPath;
        // this.consoleWriterService.PrintLn($"Renamed:",$"Old: {e.OldFullPath}",$"New: {e.FullPath}");
        //  this.writerService.PrintLn(fileInfoDataModel);
        this.onFileInfoDataModelChanged?.Invoke(e.ChangeType, fileInfoDataModel);
    }

    private void OnError(object sender, ErrorEventArgs e)
    {
    }

    public void Dispose()
    {
        if (this.fileSystemWatcher is null)
        {
            return;
        }
        this.fileSystemWatcher.Dispose();
        this.lockFileService.CleanUpAsync();
    }

    public void StartWatch(string fullPath, Action<WatcherChangeTypes, FileInfoDataModel> onFileInfoDataModelChanged, bool includeSubdirectories = true, string filter = ".*txt")
    {
        this.onFileInfoDataModelChanged = onFileInfoDataModelChanged;
        this.StartWatch(fullPath, includeSubdirectories, filter);
    }
}
