// <copyright company="Ghislain One Inc.">
//  Copyright (c) GhislainOne
//  This computer program includes confidential, proprietary
//  information and is a trade secret of GhislainOne. All use,
//  disclosure, or reproduction is prohibited unless authorized in
//  writing by an officer of Ghis. All Rights Reserved.
// </copyright>

namespace Ghis.FileWatcherApp.Lib
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class StandardConsoleWriterService : IConsoleWriterService
    {
        public void PrintException(Exception? ex)
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

        public void PrintLn(string message) =>           Console.WriteLine(message);

        public void PrintLn(params string[]? messages)
        {
            foreach (var item in messages)
            {
                this.PrintLn(item);
            }
        }

        public   void PrintLn(FileInfoDataModel fileInfoDataModel)
        {
            if(fileInfoDataModel is null)
            {
                return ;
            }
            PrintLn(  fileInfoDataModel?.CreationTime, fileInfoDataModel?.LastAccess, fileInfoDataModel?.FileName);
        }

    }

}
