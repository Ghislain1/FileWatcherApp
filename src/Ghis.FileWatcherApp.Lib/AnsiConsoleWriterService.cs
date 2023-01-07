// <copyright company="Ghislain One Inc.">
//  Copyright (c) GhislainOne
//  This computer program includes confidential, proprietary
//  information and is a trade secret of GhislainOne. All use,
//  disclosure, or reproduction is prohibited unless authorized in
//  writing by an officer of Ghis. All Rights Reserved.
// </copyright>

namespace Ghis.FileWatcherApp.Lib
{
    using Microsoft.VisualBasic;
    using Spectre.Console;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class AnsiConsoleWriterService : IConsoleWriterService
    {
        private FileInfoDataModel? fileInfoDataModel;

        private Table? attributesTable;
        public AnsiConsoleWriterService()
        {
            this.attributesTable = new Table().Expand().BorderColor(Color.Grey);
            this.attributesTable.AddColumn(TableColumnName.CreationTime.ToString());
            this.attributesTable.AddColumn(TableColumnName.DirectoryName.ToString());
            this.attributesTable.AddColumn(TableColumnName.FileName.ToString());
            this.attributesTable.AddColumn(TableColumnName.LastAccess.ToString());
            this.attributesTable.AddColumn(TableColumnName.LastWrite.ToString());
            this.attributesTable.AddColumn(TableColumnName.Security.ToString());


            //this.attributesTable.AddColumn("[yellow]Source currency[/]");
            //this.attributesTable.AddColumn("[yellow]Destination currency[/]");
            //this.attributesTable.AddColumn("[yellow]Exchange rate[/]");

            AnsiConsole.MarkupLine("Press [yellow]CTRL+C[/] to exit");

            this.SetUpTableAsync();

        }
        private bool canRefresh;
        private bool continouslyUpdateTable;
        private async Task SetUpTableAsync()
        {
            int count = 1;
            this.continouslyUpdateTable = true;
            await AnsiConsole.Live(this.attributesTable)
            .AutoClear(false)
            .Overflow(VerticalOverflow.Ellipsis)
            .Cropping(VerticalOverflowCropping.Bottom)            
            .StartAsync(async ctx =>
            {
                // Continously update the table
                while (this.continouslyUpdateTable)
                {
                    // Add a new row
                    if (this.canRefresh)
                    {
                        // Refresh and wait for a while
                        this.AddNewRow();
                        ctx.Refresh();                      
                    }
                    
                
                    await Task.Delay(400);
                    count--;
                    if (count == 0)
                    {
                        ctx.Refresh();
                    }
                }
            });
        }

        private bool AddNewRow()
        {               
            if (fileInfoDataModel is null)
            {
                this.canRefresh = false;
                return false;
            }       
            
            this.attributesTable?.AddRow(this.fileInfoDataModel.CreationTime,
                this.fileInfoDataModel.DirectoryName,
                this.fileInfoDataModel.FileName,
                this.fileInfoDataModel.LastAccess,
                this.fileInfoDataModel . LastWrite,
                this.fileInfoDataModel.Security);

            return true;
        }

        private void SetUpColors()
        {
            if (this.fileInfoDataModel?.WatcherColorNameTypes is not WatcherColorNameTypes color)
            {
                return;
            }

            this.fileInfoDataModel.CreationTime = $"[{color.ToString().ToLower()}]{this.fileInfoDataModel.CreationTime}[/]";
            this.fileInfoDataModel.DirectoryName = $"[{color.ToString().ToLower()}]{this.fileInfoDataModel.DirectoryName}[/]";
            this.fileInfoDataModel.FileName = $"[{color.ToString().ToLower()}]{this.fileInfoDataModel.FileName}[/]";
            this.fileInfoDataModel.LastAccess = $"[{color.ToString().ToLower()}]{this.fileInfoDataModel.LastAccess}[/]";
            this.fileInfoDataModel.LastWrite = $"[{color.ToString().ToLower()}]{this.fileInfoDataModel.LastWrite}[/]";
            this.fileInfoDataModel.Security = $"[{color.ToString().ToLower()}]{this.fileInfoDataModel.Security}[/]";
            this.fileInfoDataModel.Message = $"[{color.ToString().ToLower()}]{this.fileInfoDataModel.Message}[/]";



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
                this.messageToPrints.Add(item); 
            }
           
        }

        public void PrintLn(FileInfoDataModel fileInfoDataModel)
        {
            if (fileInfoDataModel is null)
            {
                return;
            }
            this.canRefresh = false;
            if (fileInfoDataModel.WatcherColorNameTypes is not null && fileInfoDataModel.WatcherColorNameTypes.HasValue)
            {

                this.SetUpColors();
            }
            this.fileInfoDataModel = fileInfoDataModel;
            this.canRefresh = true;

            //  AnsiConsole.MarkupLine(this.fileInfoDataModel.Message);
        }

        public List<string> messageToPrints = new List<string>();
        public void PrintLn(string message)
        {


        }
    }

}
