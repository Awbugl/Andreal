using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Andreal.Core.Common;
using Andreal.Window.Common;
using MessageBox = System.Windows.MessageBox;

#pragma warning disable CS4014

namespace Andreal.Window.UI;

internal partial class App
{
    private static Mutex? _mutex;

    protected override void OnStartup(StartupEventArgs e)
    {
        _mutex = new(true, "AndrealOnlyRunMutex");
        if (!_mutex.WaitOne(0, false) &&
            MessageBox.Show("已有在运行的Andreal，是否要打开新的实例？", "Andreal提示", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
            Environment.Exit(0);

        Current.DispatcherUnhandledException += (_, args) =>
        {
            ExceptionLogger.Log(args.Exception);
            args.Handled = true;
        };

        TaskScheduler.UnobservedTaskException += (_, args) =>
        {
            ExceptionLogger.Log(args.Exception.InnerException);
            args.SetObserved();
        };

        AppDomain.CurrentDomain.UnhandledException += (_, args) => ExceptionLogger.Log(args.ExceptionObject as Exception);

        ExceptionLogger.OnExceptionRecorded += exception =>
        {
            Program.Add(Program.Exceptions, new() { Time = DateTime.Now, Exception = exception });
            if (Program.Exceptions.Count > 100) Program.RemoveFirst(Program.Exceptions);
        };

        FindResource("Taskbar");
        base.OnStartup(e);

        var window = new SourceDownloader();
        window.Show();

        MainWindow = new MainWindow();
        MainWindow.Hide();
    }
}
