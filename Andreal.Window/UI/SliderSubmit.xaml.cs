using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Konata.Core;
using Konata.Core.Interfaces.Api;
using QRCoder;

namespace Andreal.Window.UI;

internal partial class SliderSubmit
{
    private readonly Bot _bot;
    private readonly string _sliderUrl;

    internal SliderSubmit(Bot bot, string sliderUrl)
    {
        _bot = bot;
        _sliderUrl = sliderUrl;
        InitializeComponent();
        Dispatcher.Invoke(CreateQr);
    }

    private void CreateQr()
    {
        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(_sliderUrl, QRCodeGenerator.ECCLevel.L);
        using var qrCode = new QRCode(qrCodeData);
        using var qrCodeImage = qrCode.GetGraphic(4);

        Image.Source
            = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(qrCodeImage.GetHbitmap(), IntPtr.Zero,
                                                                           Int32Rect.Empty,
                                                                           BitmapSizeOptions.FromEmptyOptions());
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

    private void OnPasteButtonClick(object sender, RoutedEventArgs e) { CodeBox.Text = Clipboard.GetText(); }
}
