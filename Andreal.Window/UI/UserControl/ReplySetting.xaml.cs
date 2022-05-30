using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Andreal.Core.Common;
using Andreal.Core.Model.Arcaea;
using Newtonsoft.Json;
using MessageBox = System.Windows.Forms.MessageBox;
using Path = Andreal.Core.Common.Path;

namespace Andreal.Window.UI.UserControl;

internal partial class ReplySetting
{
    internal ReplySetting()
    {
        InitializeComponent();
        ComboBox.ItemsSource = typeof(RobotReply).GetProperties();
    }

    private PropertyInfo? CurrentProperty { get; set; }

    private void OnSaveButtonClick(object sender, RoutedEventArgs e)
    {
        if (CurrentProperty is null) return;

        CurrentProperty.SetValue(GlobalConfig.RobotReply, TextBox.Text);
        File.WriteAllText(Path.RobotReply, JsonConvert.SerializeObject(GlobalConfig.RobotReply, Formatting.Indented));
    }

    private void ComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (((ComboBox)sender).SelectedItem is PropertyInfo info)
        {
            CurrentProperty = info;
            TextBox.Text = CurrentProperty.GetValue(GlobalConfig.RobotReply) as string;
        }
    }

    private void OnClearTempImageButtonClick(object sender, RoutedEventArgs e)
    {
        foreach (var j in new DirectoryInfo(Path.TempImageRoot).GetFiles()) j.Delete();
        MessageBox.Show("发送图片缓存清除完成。");
    }

    private void OnClearBackgroundImageButtonClick(object sender, RoutedEventArgs e)
    {
        foreach (var j in new DirectoryInfo(Path.ArcaeaBackgroundRoot).GetFiles()) j.Delete();
        MessageBox.Show("查分背景缓存清除完成。");
    }

    private async void OnUpdateButtonClick(object sender, RoutedEventArgs e)
    {
        await Task.Run(ArcaeaCharts.Init);
        MessageBox.Show("Arc曲目列表已更新。");
    }
}
