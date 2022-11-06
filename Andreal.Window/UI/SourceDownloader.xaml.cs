﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Net.Http.Handlers;
using System.Windows.Forms;
using System.Windows.Input;
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
        _client.BaseAddress = new("https://server.awbugl.top/andreal/");
        _client.Timeout = TimeSpan.FromSeconds(30);
        _worker.WorkerReportsProgress = true;
        _worker.DoWork += Download;
        _worker.ProgressChanged += ProgressChanged;
        _worker.RunWorkerAsync();
        _worker.RunWorkerCompleted += WorkerCompleted;
    }

    private void WorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
    {
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
        ProgressBar.Value = value;
        Block.Text = value + "%";
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

            var list = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(_client!.GetStringAsync("list.json").Result)!;

            Dispatcher.Invoke(() => Notice.Text = "正在下载：");

            foreach (var (key,ls) in list)
            {
                foreach (var i in ls)
                {
                    var path = GetPath(key) + i;
                    var requestUri = $"{key}/{i}";
                    Dispatcher.Invoke(() => TextBlock.Text = requestUri);
                    if (File.Exists(path)) continue;
                    var config = _client.GetByteArrayAsync(requestUri).Result;
                    File.WriteAllBytes(path, config);
                }
            }
        }
        catch
        {
            MessageBox.Show("下载失败，请检查网络连接。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Environment.Exit(0);
        }
    }

    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        try
        {
            DragMove();
        }
        catch
        {
            //ignored
        }
    }
}
