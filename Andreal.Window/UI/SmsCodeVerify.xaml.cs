using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using Konata.Core;
using Konata.Core.Interfaces.Api;

namespace Andreal.Window.UI;

internal partial class SmsCodeVerify
{
    private static readonly Regex Regex = new("[^0-9]+", RegexOptions.Compiled);

    private readonly Bot _bot;

    internal SmsCodeVerify(Bot bot, string phone)
    {
        InitializeComponent();
        _bot = bot;
        PhoneBlock.Text = phone;
    }

    private void OnSubmit(object sender, RoutedEventArgs e)
    {
        _bot.SubmitSmsCode(CodeBox.Text);
        Close();
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

    private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e) => e.Handled = Regex.IsMatch(e.Text);
}
