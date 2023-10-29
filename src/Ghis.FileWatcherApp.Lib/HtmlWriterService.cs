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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
enum PageLayout
{
    Accordion,
    Paragraph
}
public class HtmlWriterService : IWriterService
{
    private string watcherOutputPath;
    private SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1);
    public HtmlWriterService(string watcherOutputPath)
    {
        this.watcherOutputPath = watcherOutputPath;
    }
    public void PrintException(Exception? ex)
    {

    }

    private Task<bool> EnsureAccessAsync(FileInfo fileInfo)
    {
        return Task.Run(() =>
        {
            if (File.Exists(fileInfo.FullName))
            {
                fileInfo.Delete();
                GC.Collect();
                GC.WaitForPendingFinalizers();
                File.Delete(fileInfo.FullName);
                while (IsFileLocked(fileInfo))
                {
                    Thread.Sleep(1000);
                    GC.Collect();
                    GC.WaitForPendingFinalizers();              
                }
            }


            return true;

        });

    }

    private Task CopyAsync(string fileToCopy, string destToCopy)
    {
        return Task.Run(() =>
        {

            if (File.Exists(destToCopy))
            {
                File.Delete(destToCopy);
            }

            File.Copy(fileToCopy, destToCopy);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            while (IsFileLocked(new FileInfo(fileToCopy)))
            {
                Thread.Sleep(1000);
            }


        });

    }

    public async void PrintLn(FileInfoDataModel fileInfoDataModel)
    {
        var fileToCopy = fileInfoDataModel.FullName;
        var destToCopy = Path.Combine(this.watcherOutputPath, fileInfoDataModel.FileName);
        var fileInfo = new FileInfo(destToCopy);
        semaphoreSlim.WaitAsync();
        var accessible = await this.EnsureAccessAsync(fileInfo);
        semaphoreSlim.Release();
        if (accessible)
        {
            semaphoreSlim.WaitAsync();
            await CopyAsync(fileToCopy, destToCopy);
            semaphoreSlim.Release();
            this.UpdateHtmlOutputFiles();
        }


    }
    private void UpdateHtmlOutputFiles()
    {
        UpdateHtmlFileWithTextContent("Accordion.html", "Accordion", PageLayout.Accordion);
        UpdateHtmlFileWithTextContent("Paragraph.html", "Paragraph", PageLayout.Paragraph);

    }
    private void UpdateHtmlFileWithTextContent(string htmlFileName, string pageHeading, PageLayout pageLayout)
    {
        string content = string.Empty;

        DirectoryInfo directoryInfo = new DirectoryInfo(this.watcherOutputPath);
        FileInfo[] files = directoryInfo.GetFiles("*.txt");

        using (StreamWriter sw = new StreamWriter(Path.Combine(this.watcherOutputPath, htmlFileName)))
        {
            AddTopHtml(sw, pageHeading, pageLayout);

            foreach (FileInfo file in files)
            {
                while (IsFileLocked(file))
                {
                    Thread.Sleep(1000);
                }
                using (StreamReader sr = new StreamReader(file.OpenRead()))
                {
                    content = sr.ReadToEnd();
                }

                BuildHtmlBody(sw, content, Path.GetFileNameWithoutExtension(file.FullName));
            }
            AddBottomHtml(sw);
        }
    }
    private static void AddBottomHtml(StreamWriter sw)
    {
        sw.WriteLine("</div>");
        sw.WriteLine("</body>");
        sw.WriteLine("</html>");
    }
    private static void BuildHtmlBody(StreamWriter sw, string topicContent, string topicHeading)
    {
        sw.WriteLine($"<h3>{topicHeading}</h3>");
        sw.WriteLine("<div>");
        sw.WriteLine("<p>");
        sw.Write(topicContent);
        sw.WriteLine("</p>");
        sw.WriteLine("</div>");

    }
    private static void AddTopHtml(StreamWriter sw, string pageHeading, PageLayout pageLayout)
    {
        sw.WriteLine("<!doctype html>");
        sw.WriteLine(@"<html lang = ""en"">");
        sw.WriteLine("<head>");
        sw.WriteLine(@"<meta charset = ""utf-8"">");
        sw.WriteLine(@"<meta name = ""viewport"" content = ""width=device-width,intial-scale=1"">");
        sw.WriteLine($"<title>{pageHeading}</title>");


        if (pageLayout == PageLayout.Accordion)
        {
            sw.WriteLine(@"<link rel = ""stylesheet"" href = ""https://code.jquery.com/ui/1.12.1/themes/base/jquery-ui.css"" >");
            sw.WriteLine(@"<script src = ""https://code.jquery.com/jquery-1.12.4.js""></script>");
            sw.WriteLine(@"<script src = ""https://code.jquery.com/ui/1.12.1/jquery-ui.js""></script>");

            sw.WriteLine("<script>");
            sw.WriteLine("$(function(){");
            sw.WriteLine(@"$(""#accordion"").accordion();");
            sw.WriteLine("});");
            sw.WriteLine("</script>");
            sw.WriteLine("</head>");
            sw.WriteLine("<body>");
            sw.WriteLine($"<h1>{pageHeading}</h1> ");
            sw.WriteLine(@"<div id = ""accordion"">");
        }
        else if (pageLayout == PageLayout.Paragraph)
        {
            sw.WriteLine($"<h1>{pageHeading}</h1>");
            sw.WriteLine("<div>");
            sw.WriteLine("</head>");
            sw.WriteLine("<body>");
        }

    }

    private static bool IsFileLocked(FileInfo file)
    {
        FileStream stream = null;

        try
        {
            stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        }
        catch (IOException)
        {
            //the file is unavailable because it is:
            //still being written to
            //or being processed by another thread
            //or does not exist (has already been processed)
            var curreny = Thread.CurrentThread;
            return true;
        }
        finally
        {

            stream?.Close();
        }

        //file is not locked
        return false;
    }
}
