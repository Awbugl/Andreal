using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Andreal.Core.Utils;
using Andreal.Window.UI.UserControl;

namespace Andreal.Window.UI;

internal partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();
        var timer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(1) };
        timer.Tick += (_, _) => Status.Text = BotStatementHelper.Status;
        timer.Start();
    }

    internal enum WindowStatus
    {
        None,
        AccountManage,
        Setting,
        ReplySetting,
        MessageLog,
        ExceptionLog
    }

    private WindowStatus _status = WindowStatus.None;

    private void OnMinBtnClick(object sender, RoutedEventArgs e) => Hide();

    private void OnAccountManageClick(object sender, RoutedEventArgs e)
    {
        if (_status == WindowStatus.AccountManage) return;
        
        _status = WindowStatus.AccountManage;
        Label.Content = new Accounts();
    }

    private void OnSettingClick(object sender, RoutedEventArgs e)
    {
        if (_status == WindowStatus.Setting) return;
        
        _status = WindowStatus.Setting;
        Label.Content = new Setting();
    }

    private void OnMessagePushClick(object sender, MouseButtonEventArgs e)
    {
        if (_status == WindowStatus.MessageLog) return;
        
        _status = WindowStatus.MessageLog;
        Label.Content = new UserControl.MessageLog();
    }

    private void OnExceptionLogClick(object sender, MouseButtonEventArgs e)
    {
        if (_status == WindowStatus.ExceptionLog) return;
        
        _status = WindowStatus.ExceptionLog;
        Label.Content = new UserControl.ExceptionLog();
    }

    private void OnReplySettingClick(object sender, MouseButtonEventArgs e)
    {
        if (_status == WindowStatus.ReplySetting) return;
        
        _status = WindowStatus.ReplySetting;
        Label.Content = new ReplySetting();
    }
   
    private void OnMainWindowClosed(object? sender, EventArgs e) { Environment.Exit(0); }

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
