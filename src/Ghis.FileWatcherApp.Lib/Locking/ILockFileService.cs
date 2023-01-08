// <copyright company="Ghislain One Inc.">
//  Copyright (c) GhislainOne
//  This computer program includes confidential, proprietary
//  information and is a trade secret of GhislainOne. All use,
//  disclosure, or reproduction is prohibited unless authorized in
//  writing by an officer of Ghis. All Rights Reserved.
// </copyright>

namespace Ghis.FileWatcherApp.Lib.Locking
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Threading;
 

    public interface   ILockFileService 
    {
        /// <summary>
        ///     Acquire lock.
        /// </summary>
        /// <param name="releaseDate">Date after that lock is released</param>
        /// <returns>File lock. False if lock already exists.</returns>
     Task<bool> IsFileLockedAsync(DateTime releaseDate, FileInfo fileToLock);

     Task<bool> IsFileLockedAsync(DateTime releaseDate, string fileToLock);

     Task  CleanUpAsync( );
    }
}
