using System;
using System.Threading.Tasks;
using System.Windows;
using Andreal.Core.Common;
using Andreal.Window.Common;

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
                                            Program.Add(Program.Exceptions,
                                                        new() { Time = DateTime.Now, Exception = exception });
                                            if (Program.Exceptions.Count > 100) Program.RemoveFirst(Program.Exceptions);
                                        };

        FindResource("Taskbar");
        base.OnStartup(e);

        Dispatcher.InvokeAsync(Program.ProgramInit);

        MainWindow = new MainWindow();
        MainWindow.Show();
    }
}
