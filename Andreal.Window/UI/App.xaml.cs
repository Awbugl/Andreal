using System;
using System.Windows;
using Andreal.Core.Common;
using Andreal.Window.Common;

#pragma warning disable CS4014

namespace Andreal.Window.UI;

internal partial class App
{
    protected override void OnStartup(StartupEventArgs e)
    {
        DispatcherUnhandledException += (_, args) =>
                                        {
                                            Reporter.ExceptionReport(args.Exception);
                                            args.Handled = true;
                                        };
        
        AppDomain.CurrentDomain.UnhandledException+= (_, args) => Reporter.ExceptionReport(args.ExceptionObject as Exception);

        Reporter.OnExceptionRecorded += exception =>
                                        {
                                            Program.Add(Program.Exceptions,
                                                        new() { Time = DateTime.Now, Exception = exception });
                                            if (Program.Exceptions.Count > 100) Program.RemoveAt(Program.Exceptions);
                                        };

        FindResource("Taskbar");
        base.OnStartup(e);

        Dispatcher.InvokeAsync(Program.ProgramInit);

        MainWindow = new MainWindow();
        MainWindow.Show();
    }
}
