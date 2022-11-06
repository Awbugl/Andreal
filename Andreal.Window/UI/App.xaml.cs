using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Andreal.Core.Common;
using Andreal.Window.Common;
using Path = Andreal.Core.Common.Path;

#pragma warning disable CS4014

namespace Andreal.Window.UI;

internal partial class App
{
    protected override void OnStartup(StartupEventArgs e)
    {
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
