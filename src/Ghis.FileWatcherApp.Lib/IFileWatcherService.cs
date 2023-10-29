namespace Ghis.FileWatcherApp.Lib
{
    public interface IFileWatcherService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="filter"></param>
        /// <param name="includeSubdirectories"></param>
        /// <param name="filters"></param>
        /// <remarks>Use of multiple filters such as "*.txt|*.doc" is not supported.</remarks>
        void StartWatch(string fullPath,  bool includeSubdirectories = true,   string  filter=".*txt");

        void StartWatch(string fullPath, Action<WatcherChangeTypes,FileInfoDataModel> onFileInfoDataModelChanged, bool includeSubdirectories = true, string filter = ".*txt");
    }
}