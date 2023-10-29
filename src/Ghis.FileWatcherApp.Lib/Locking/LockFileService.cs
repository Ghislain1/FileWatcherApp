// <copyright company="Ghislain One Inc.">
//  Copyright (c) GhislainOne
//  This computer program includes confidential, proprietary
//  information and is a trade secret of GhislainOne. All use,
//  disclosure, or reproduction is prohibited unless authorized in
//  writing by an officer of Ghis. All Rights Reserved.
// </copyright>

namespace Ghis.FileWatcherApp.Lib.Locking;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class LockFileService : ILockFileService
{
    private const string Lock_Extension = "lock";
    private string ?lockFileFullPath;

    public async Task<bool> IsFileLockedAsync(DateTime releaseDate, FileInfo fileToLock)
    {
        var lockFileInfoDataModel = new LockFileInfoDataModel(fileToLock.FullName);
        this.lockFileFullPath = Path.ChangeExtension(fileToLock.FullName, Lock_Extension);
        if (File.Exists(this.lockFileFullPath) && await lockFileInfoDataModel.GetReleaseDate() > DateTime.UtcNow)
        {
            return false;
        }

        return await lockFileInfoDataModel.TrySetReleaseDate(releaseDate);
    }

    public async Task<bool> IsFileLockedAsync(DateTime releaseDate, string fileToLock)
    {
        return await this.IsFileLockedAsync(releaseDate, new FileInfo(fileToLock));
    }
    public async Task CleanUpAsync() => await Task.Run(() => CleanUp());
    private void CleanUp()
    {
        if (!File.Exists(lockFileFullPath))
        {
            return;
        }
        File.Delete(lockFileFullPath);
    }



}
