using System.Windows.Forms;
using System.Windows.Input;
using Konata.Core;
using Konata.Core.Interfaces.Api;
using Microsoft.Web.WebView2.Core.DevToolsProtocolExtension;
using Newtonsoft.Json.Linq;

namespace Andreal.Window.UI;

internal partial class SliderVerify
{
    private readonly string _sliderUrl;
    private DevToolsProtocolHelper? _cdpHelper;
    private readonly Bot _bot;

    private string _ticketId = "";

    private string _ticket = "";

    internal SliderVerify(Bot bot, string sliderUrl)
    {
        _sliderUrl = sliderUrl;
        _bot = bot;
        InitializeComponent();
        InitializeAsync();
    }

    private DevToolsProtocolHelper CdpHelper
    {
        get { return _cdpHelper ??= WebBrowser.CoreWebView2.GetDevToolsProtocolHelper(); }
    }

    private async void InitializeAsync()
    {
        try
        {
            await WebBrowser.EnsureCoreWebView2Async();
            await CdpHelper.Network.EnableAsync();
        }
        catch
        {
           if (MessageBox.Show("未找到Webview运行时！是否下载Webview安装包？（安装后请重启Andreal）\n选择否将使用滑块验证助手", "提示", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
               System.Diagnostics.Process.Start("https://go.microsoft.com/fwlink/p/?LinkId=2124703");
           else
           {
               var window = new SliderSubmit(_bot, _sliderUrl);
               window.Show();
           }
           Close();
        }

        await
            CdpHelper.Network
                     .SetUserAgentOverrideAsync("Mozilla/5.0 (Linux; Android 10; PCT-AL10 Build/HUAWEIPCT-AL10; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/89.0.4389.72 MQQBrowser/6.2 TBS/046011 Mobile Safari/537.36 V1_AND_SQ_8.8.88_2770_YYB_D A_8088800 QQ/8.8.88.7830 NetType/WIFI WebP/0.3.0 Pixel/1080",
                                                platform: "Android");

        WebBrowser.CoreWebView2.Navigate(_sliderUrl);

        CdpHelper.Network.ResponseReceived += (_, args) =>
                                              {
                                                  if (args.Response.Url
                                                      == "https://t.captcha.qq.com/cap_union_new_verify")
                                                      _ticketId = args.RequestId;
                                              };

        CdpHelper.Network.LoadingFinished += async (_, args) =>
                                             {
                                                 if (args.RequestId != _ticketId) return;

                                                 _ticketId = args.RequestId;
                                                 _ticket
                                                     = JObject.Parse((await CdpHelper.Network
                                                                                     .GetResponseBodyAsync(_ticketId))
                                                                     .Body)["ticket"]!.ToString();

                                                 _bot.SubmitSliderTicket(_ticket);
                                                 Close();
                                             };
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
}
