using System;
using System.Diagnostics;
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

        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
                                                      { 
                                                          ExceptionLogger.Log(args.ExceptionObject as Exception);
                                                          try
                                                          {
                                                            
                                                              Process.Start(AppContext.BaseDirectory + @"\Andreal.Window.exe");
                                                          }
                                                          catch
                                                          {
                                                              // ignore
                                                          }
                                                      };

        ExceptionLogger.OnExceptionRecorded += exception =>
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
