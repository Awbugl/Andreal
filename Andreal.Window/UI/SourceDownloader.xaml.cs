using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Net.Http.Handlers;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Andreal.Core.Common;
using Andreal.Window.Common;
using Newtonsoft.Json;
using static Andreal.Core.Common.Path;
using Application = System.Windows.Application;
using MessageBox = System.Windows.Forms.MessageBox;

namespace Andreal.Window.UI;

internal partial class SourceDownloader
{
    private HttpClient? _client;
    private readonly BackgroundWorker _worker = new();

    internal SourceDownloader()
    {
        InitializeComponent();
        ProgressBar.Maximum = 100;
        var processMessageHander = new ProgressMessageHandler(new HttpClientHandler());
        processMessageHander.HttpReceiveProgress += (_, e) => { _worker.ReportProgress(e.ProgressPercentage); };
        _client = new(processMessageHander);
        _client.BaseAddress = new("https://assets.awbugl.top/andreal/");
        _client.Timeout = TimeSpan.FromMinutes(5);
        _worker.WorkerReportsProgress = true;
        _worker.DoWork += Download;
        _worker.ProgressChanged += ProgressChanged;
        _worker.RunWorkerAsync();
        _worker.RunWorkerCompleted += WorkerCompleted;
    }

    private void WorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
    {
        if ((int)ProgressBar.Value != 100)
            Dispatcher.Invoke(() =>
            {
                MessageBox.Show("下载失败，请检查网络连接或重试。", "下载失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                Environment.Exit(0);
                return;
            });

        var mainWindow = Application.Current.MainWindow!;
        mainWindow.Dispatcher.InvokeAsync(Program.ProgramInit);
        mainWindow.Show();
        _client?.Dispose();
        _client = null!;
        Close();
    }

    private void ProgressChanged(object? sender, ProgressChangedEventArgs e)
    {
        var value = e.ProgressPercentage;
        Dispatcher.Invoke(() =>
        {
            ProgressBar.Value = value;
            Block.Text = value + "%";
        });
    }

    private static string GetPath(string id)
        => id switch
           {
               "config" => AndreaConfigRoot,
               "source" => ArcaeaSourceRoot,
               "fonts"  => ArcaeaFontRoot,
               _        => throw new ArgumentOutOfRangeException(nameof(id), id, null)
           };

    private void Download(object? sender, DoWorkEventArgs e)
    {
        try
        {
            Directory.CreateDirectory(AndreaConfigRoot + "BotInfo/");
            Directory.CreateDirectory(ArcaeaSourceRoot);
            Directory.CreateDirectory(ArcaeaFontRoot);

            var list = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(_client!.GetStringAsync("list.json").Result)!;

            Dispatcher.Invoke(() =>
            {
                Notice.Text = "正在下载：";
                ProgressBar.Visibility = Visibility.Visible;
            });

            foreach (var (key, ls) in list)
            {
                foreach (var i in ls)
                {
                    var path = GetPath(key) + i;
                    var requestUri = $"{key}/{i}";
                    Dispatcher.Invoke(() =>
                    {
                        TextBlock.Text = requestUri;
                        ProgressBar.Value = 0;
                    });
                    if (File.Exists(path)) continue;
                    var config = _client.GetByteArrayAsync(requestUri).Result;
                    File.WriteAllBytes(path, config);
                }
            }

            Dispatcher.Invoke(() => { ProgressBar.Value = 100; });
        }
        catch (Exception ex)
        {
            ExceptionLogger.Log(ex);
            Dispatcher.Invoke(() => { ProgressBar.Value = 0; });
        }
    }

    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        try
        {
            Focus();
            DragMove();
        }
        catch
        {
            //ignored
        }
    }
}
