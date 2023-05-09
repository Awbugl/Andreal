using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Andreal.Core.Common;
using Andreal.Core.Data.Api;
using Andreal.Window.Common;
using Konata.Core.Common;
using Newtonsoft.Json;
using Path = Andreal.Core.Common.Path;

namespace Andreal.Window.UI.UserControl;

internal partial class Setting
{
    private static readonly Regex Regex = new("[^0-9]+", RegexOptions.Compiled);

    internal Setting()
    {
        InitializeComponent();

        MasterQQ.Text = Program.Config.Master.ToString();

        if (Program.Config.Api.TryGetValue("unlimited", out var auavalue))
        {
            AUAUrl.Text = auavalue.Url;
            AUAToken.Text = auavalue.Token;
        }

        ProtocolComboBox.SelectedItem = Program.Config.Protocol;
        SliderComboBox.SelectedItem = Program.Config.SliderType;
        EnableProcess.IsChecked = Program.Config.EnableHandleMessage;
        AutoFriendRequest.IsChecked = Program.Config.Settings.FriendAdd;
        AutoGroupRequest.IsChecked = Program.Config.Settings.GroupAdd;
        WhiteList.Text = string.Join('\n', Program.Config.Settings.GroupInviterWhitelist);
    }

    private OicqProtocol CurrentProtocol { get; set; } = OicqProtocol.AndroidPhone;

    private void OnPreviewTextInput(object sender, TextCompositionEventArgs e) => e.Handled = Regex.IsMatch(e.Text);

    private void OnSaveButtonClick(object sender, RoutedEventArgs e)
    {
        if (!uint.TryParse(MasterQQ.Text, out var master))
        {
            MessageBox.Show("Bot管理员QQ号解析失败！");
            return;
        }

        List<uint> whitelist;

        try
        {
            whitelist = WhiteList.Text.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(uint.Parse)
                                 .ToList();
        }
        catch
        {
            MessageBox.Show("群白名单的读取失败！请按格式填写。（分行、QQ号）", "提示");
            return;
        }

        var config = new AndrealConfig
                     {
                         Master = master,
                         Protocol = CurrentProtocol,
                         Accounts = Program.Config.Accounts,
                         EnableHandleMessage = EnableProcess.IsChecked ?? true,
                         Settings = new()
                                    {
                                        FriendAdd = AutoFriendRequest.IsChecked ?? false,
                                        GroupAdd = AutoGroupRequest.IsChecked ?? false,
                                        GroupInviterWhitelist = whitelist
                                    }
                     };

        var auauri = AUAUrl.Text;
        var auatoken = AUAToken.Text;

        if (!string.IsNullOrWhiteSpace(auauri) && !string.IsNullOrWhiteSpace(auatoken))
        {
            config.Api["unlimited"] = new() { Url = auauri, Token = auatoken };
            UnofficialArcaeaAPI.Init(config);
        }

        Program.Config = config;
        File.WriteAllText(Path.Config, JsonConvert.SerializeObject(config, Formatting.Indented));
    }

    private void ComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (((ComboBox)sender).SelectedItem is OicqProtocol protocol) CurrentProtocol = protocol;
    }
}
