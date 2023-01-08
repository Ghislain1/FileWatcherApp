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
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class LockFileInfoDataModel
    {
        private readonly string lockFullPath;
        private const string Extension = "lock";
        public LockFileInfoDataModel(string path)
        {
            this.lockFullPath = Path.ChangeExtension(path, Extension); 
        }

        internal async Task<bool> TrySetReleaseDate(DateTime date)
        {
            try
            {
                using (var fs = new FileStream(lockFullPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
                {
                    using (var sr = new StreamWriter(fs, Encoding.UTF8))
                    {
                        await sr.WriteAsync(date.ToUniversalTime().Ticks.ToString());
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        internal async Task<DateTime> GetReleaseDate(string path = "")
        {
            try
            {
                using (var fs = new FileStream(string.IsNullOrWhiteSpace(path) ? lockFullPath : path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var sr = new StreamReader(fs, Encoding.UTF8))
                    {
                        string text = await sr.ReadToEndAsync();
                        long ticks = long.Parse(text);
                        return new DateTime(ticks, DateTimeKind.Utc);
                    }
                }
            }
            catch (Exception)
            {
                return DateTime.MaxValue;
            }
        }
    }

}
