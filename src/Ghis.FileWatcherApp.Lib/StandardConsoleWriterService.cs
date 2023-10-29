// <copyright company="Ghislain One Inc.">
//  Copyright (c) GhislainOne
//  This computer program includes confidential, proprietary
//  information and is a trade secret of GhislainOne. All use,
//  disclosure, or reproduction is prohibited unless authorized in
//  writing by an officer of Ghis. All Rights Reserved.
// </copyright>

namespace Ghis.FileWatcherApp.Lib;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class StandardConsoleWriterService : IWriterService
{
    private FileInfoDataModel? fileInfoDataModel;
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
    public void PrintLn(FileInfoDataModel? fileInfoDataModel)
    {
        if (fileInfoDataModel is null)
        {
            return;
        }
        this.fileInfoDataModel = fileInfoDataModel;
        Console.WriteLine($"{this.fileInfoDataModel.Caption,8}");
        this.PrintWithFormat(nameof(this.fileInfoDataModel.FileName), this.fileInfoDataModel.FileName);
        this.PrintWithFormat(nameof(this.fileInfoDataModel.LastWrite), this.fileInfoDataModel.LastWrite);
        this.PrintWithFormat(nameof(this.fileInfoDataModel.Security), this.fileInfoDataModel.Security);
        this.PrintWithFormat(nameof(this.fileInfoDataModel.LastAccess), this.fileInfoDataModel.LastAccess);
        this.PrintWithFormat(nameof(this.fileInfoDataModel.DirectoryName), this.fileInfoDataModel.DirectoryName);
        this.PrintWithFormat(nameof(this.fileInfoDataModel.Message), this.fileInfoDataModel.Message);

        Console.WriteLine();



    }
    private void PrintWithFormat(string key, string value)
    {
        // TODO: leave empty space before add rechtbuendy columns
        var keyValue = "    " + key + ": " + value;
        Console.WriteLine(keyValue.PadRight(5));



    }
   
}
