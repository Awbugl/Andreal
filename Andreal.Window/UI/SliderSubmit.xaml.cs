using System;
using System.Net.Http;
using System.Windows;
using System.Windows.Input;
using Konata.Core;
using Konata.Core.Interfaces.Api;
using Newtonsoft.Json.Linq;

namespace Andreal.Window.UI;

internal partial class SliderSubmit : IDisposable
{
    private readonly Bot _bot;
    private readonly string _sliderUrl;
    private HttpClient? _httpClient;

    internal SliderSubmit(Bot bot, string sliderUrl)
    {
        _bot = bot;
        _sliderUrl = sliderUrl.Replace("ssl.captcha.qq.com", "txhelper.glitch.me");
        _httpClient = new();
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "HttpClientFactory-Sample");
        InitializeComponent();
        Dispatcher.Invoke(GetCode);
    }

    private void GetCode()
    {
        var str = _httpClient!.GetStringAsync(_sliderUrl).Result;
        CodeBlock.Text = JObject.Parse(str)["code"]!.ToString();
    }

    private void OnSubmit(object sender, RoutedEventArgs e)
    {
        var str = _httpClient!.GetStringAsync(_sliderUrl).Result;
        var ticket = JObject.Parse(str)["ticket"]!.ToString();
        _bot.SubmitSliderTicket(ticket);
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

    public void Dispose()
    {
        _httpClient?.Dispose();
        _httpClient = null;
    }
}
