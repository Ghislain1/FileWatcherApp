// <copyright company="Ghislain One Inc.">
//  Copyright (c) GhislainOne
//  This computer program includes confidential, proprietary
//  information and is a trade secret of GhislainOne. All use,
//  disclosure, or reproduction is prohibited unless authorized in
//  writing by an officer of Ghis. All Rights Reserved.
// </copyright>

namespace Ghis.FileWatcherApp.Lib
{
    using Spectre.Console;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class AnsiConsoleWriterService : IConsoleWriterService
    {
        private Table? attributesTable;
        public AnsiConsoleWriterService()
        {
            AnsiConsole.Live(this.attributesTable)
                .AutoClear(false)   // Do not remove when done
                .Overflow(VerticalOverflow.Ellipsis) // Show ellipsis when overflowing
                .Cropping(VerticalOverflowCropping.Top) // Crop overflow at top
             .Start(ctx =>
             {
                 this.attributesTable.AddColumn("CreationTime");
                 ctx.Refresh();
                 Thread.Sleep(100);

                 this.attributesTable.AddColumn("DirectoryName");
                 ctx.Refresh();
                 Thread.Sleep(100);

                 this.attributesTable.AddColumn("FileName");
                 ctx.Refresh();
                 Thread.Sleep(100);

                 this.attributesTable.AddColumn("LastAccess");
                 ctx.Refresh();
                 Thread.Sleep(100);

                 this.attributesTable.AddColumn("LastWrite");
                 ctx.Refresh();
                 Thread.Sleep(100);

                 this.attributesTable.AddColumn("Security");
                 ctx.Refresh();
                 Thread.Sleep(100);

                 this.attributesTable.AddColumn("Size");
                 ctx.Refresh();
                 Thread.Sleep(100);



             });
        }
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
            this.PrintException(ex.InnerException);
        }
        public void PrintLn(params string[] messages)
        {
            foreach (var item in messages)
            {
                this.PrintLn(item);
            }
        }

        public void PrintLn(string message)
        {
           // var fileInfo = new FileInfo(e.FullPath);
            //this.attributesTable?.AddRow("CreationTime", fileInfo.CreationTime.ToString()!);
            //this.attributesTable?.AddRow("DirectoryName", fileInfo.DirectoryName!);
            //this.attributesTable?.AddRow("FileName", fileInfo.Name!);
            //this.attributesTable?.AddRow("LastAccess", fileInfo.LastAccessTime.ToString()!);
            //this.attributesTable?.AddRow("LastWrite", fileInfo.LastWriteTime.ToString()!);
            //this.attributesTable?.AddRow("Security", fileInfo.IsReadOnly.ToString());
            // Render the table to the console
            //  AnsiConsole.Write(this.attributesTable);
            AnsiConsole.Live(attributesTable).Start(i => i.Refresh());
        }
    }

}
