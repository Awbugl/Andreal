using System;
using System.Windows.Forms;
using System.Windows.Input;
using Konata.Core;
using Konata.Core.Interfaces.Api;
using Microsoft.Web.WebView2.Core.DevToolsProtocolExtension;
using Newtonsoft.Json.Linq;

namespace Andreal.Window.UI;

internal partial class SliderVerify : IDisposable
{
    private readonly Bot _bot;
    private readonly string _sliderUrl;
    private DevToolsProtocolHelper? _cdpHelper;

    private string _ticket = "";

    private string _ticketId = "";

    internal SliderVerify(Bot bot, string sliderUrl)
    {
        _sliderUrl = sliderUrl;
        _bot = bot;
        InitializeComponent();
        InitializeAsync();
    }

    private DevToolsProtocolHelper CdpHelper => _cdpHelper ??= WebBrowser.CoreWebView2.GetDevToolsProtocolHelper();

    private async void InitializeAsync()
    {
        try
        {
            await WebBrowser.EnsureCoreWebView2Async();
            await CdpHelper.Network.EnableAsync();

            await
                CdpHelper.Network
                         .SetUserAgentOverrideAsync("Mozilla/5.0 (Linux; Android 10; PCT-AL10 Build/HUAWEIPCT-AL10; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/89.0.4389.72 MQQBrowser/6.2 TBS/046011 Mobile Safari/537.36 V1_AND_SQ_8.8.88_2770_YYB_D A_8088800 QQ/8.8.88.7830 NetType/WIFI WebP/0.3.0 Pixel/1080",
                                                    platform: "Android");

            WebBrowser.CoreWebView2.Navigate(_sliderUrl);
        }
        catch
        {
            MessageBox.Show("未找到Webview运行时！\n将使用滑块验证助手进行扫码验证", "提示");
            var window = new SliderSubmit(_bot, _sliderUrl);
            window.ShowDialog();
            Close();
        }

        CdpHelper.Network.ResponseReceived += (_, args) =>
        {
            if (args.Response.Url == "https://t.captcha.qq.com/cap_union_new_verify") _ticketId = args.RequestId;
        };

        CdpHelper.Network.LoadingFinished += async (_, args) =>
        {
            if (args.RequestId != _ticketId) return;

            _ticketId = args.RequestId;
            _ticket = JObject.Parse((await CdpHelper.Network.GetResponseBodyAsync(_ticketId)).Body)["ticket"]!.ToString();

            _bot.SubmitSliderTicket(_ticket);
            
            Close();
        };
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

    public void Dispose()
    {
        WebBrowser?.Dispose();
        WebBrowser = null;
    }
}
