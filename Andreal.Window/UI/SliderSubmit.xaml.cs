using System.Windows;
using System.Windows.Input;
using Konata.Core;
using Konata.Core.Interfaces.Api;

namespace Andreal.Window.UI;

internal partial class SliderSubmit
{
    private readonly Bot _bot;

    internal SliderSubmit(Bot bot, string sliderUrl)
    {
        InitializeComponent();
        _bot = bot;
        UrlBlock.Text = sliderUrl;
    }

    private void OnSubmit(object sender, RoutedEventArgs e)
    {
        _bot.SubmitSliderTicket(CodeBox.Text);
        Close();
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

    private void OnCopyButtonClick(object sender, RoutedEventArgs e) { Clipboard.SetText(UrlBlock.Text); }
}
