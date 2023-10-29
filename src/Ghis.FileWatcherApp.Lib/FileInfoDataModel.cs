// <copyright company="Ghislain One Inc.">
//  Copyright (c) GhislainOne
//  This computer program includes confidential {  get; set; }   proprietary
//  information and is a trade secret of GhislainOne. All use {  get; set; }  
//  disclosure {  get; set; }   or reproduction is prohibited unless authorized in
//  writing by an officer of Ghis. All Rights Reserved.
// </copyright>

namespace Ghis.FileWatcherApp.Lib
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class FileInfoDataModel
    {
        public FileInfoDataModel(string fullPath, WatcherColorNameTypes? watcherColorNameTypes = null, string message = "", string caption = "")
        {
            var fileInfo = new FileInfo(fullPath);
            if (fileInfo is null)
            {
                return;
            }
            this.CreationTime = fileInfo.CreationTime.ToString();
            this.DirectoryName = fileInfo!.DirectoryName?? "No Directory Name";
            this.FullName = fileInfo.FullName;
            this.FileName = fileInfo.Name;

            this.LastAccess = fileInfo.LastAccessTime.ToString();
            this.LastWrite = fileInfo.LastWriteTime.ToString();
            this.Security = fileInfo!.GetAccessControl()?.ToString() ?? "No-Security found!";
            this.WatcherColorNameTypes = watcherColorNameTypes;
            this.Message = message;
            this.Caption = caption;

        }
        public string OldFullPath { get; set; }

        public string CreationTime { get; set; }
        public string DirectoryName { get; set; }
        public string FileName { get; set; }
        public string FullName { get; set; }
        public string LastAccess { get; set; }
        public string LastWrite { get; set; }
        public string Security { get; set; }
        public WatcherColorNameTypes? WatcherColorNameTypes { get; }
        public string Message { get; set; }
        public string Caption { get; }
    }
}
