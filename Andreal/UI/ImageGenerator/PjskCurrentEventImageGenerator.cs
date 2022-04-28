using System.Drawing;
using Andreal.Data.Api;
using Andreal.Data.Json.Pjsk;
using Andreal.Message;
using Andreal.UI.Model;
using Andreal.Utils;
using Path = Andreal.Core.Path;

namespace Andreal.UI.ImageGenerator;

#pragma warning disable CA1416

internal class PjskCurrentEventImageGenerator
{
    private readonly PjskCurrentEventItem _currentEvent;
    private readonly PjskRankings? _eventRankings;
    private readonly Dictionary<int, int> _predict;

    private readonly int[] _rank = { 100, 500, 1000, 2000, 5000, 10000, 50000, 100000 };

    public PjskCurrentEventImageGenerator(PjskCurrentEventItem? currentEvent, PjskRankings? eventRankings,
                                          Dictionary<int, int>? predict)
    {
        _currentEvent = currentEvent!;
        _eventRankings = eventRankings;
        _predict = predict!;
    }

    internal TextMessage TextMessage =>
        (_eventRankings is null
            ? ""
            : $"本期活动：\n{_currentEvent.Name} \n #{_eventRankings.Rank} {_eventRankings.Name} ({_eventRankings.Score}P)\n\n")
        + $"当前线：\n{string.Join("\n", _rank.Select(async i => $"Rank {i} : {(await PjskApi.PjskEventRanking(i, _currentEvent.EventID))!.Score}P"))}\n\n"
        + $"预测线：\n{string.Join("\n", _rank.Select(i => $"Rank {i} : {_predict[i]}P"))}";

    internal async Task<ImageMessage> Generate()
    {
        var bg = PjskEvent();

        if (_eventRankings is not null)
            bg.Draw(new TextWithStrokeModel(_eventRankings.Name, Font.HummingPro32, Color.White, 250, 345),
                    new TextWithStrokeModel($"#{_eventRankings.Rank} ({_eventRankings.Score}P)", Font.HummingPro28,
                                            Color.White, 250, 405));

        for (var index = 0; index < _rank.Length; index++)
        {
            bg.Draw(new
                        TextWithStrokeModel((await PjskApi.PjskEventRanking(_rank[index], _currentEvent.EventID))!.Score.ToString(),
                                            Font.HummingPro24, Color.White, 240, 550 + index * 50, width: 180,
                                            height: 25, stringAlignment: StringAlignment.Far));
            bg.Draw(new TextWithStrokeModel(_predict[_rank[index]].ToString(), Font.HummingPro24, Color.White, 590,
                                            550 + index * 50, width: 180, height: 25,
                                            stringAlignment: StringAlignment.Far));
        }

        return bg;
    }

    private BackGround PjskEvent()
    {
        var path = Path.PjskEvent(_currentEvent.AssetbundleName);
        return path.FileExists
            ? new(path)
            : GeneratePjskEvent(path);
    }

    private BackGround GeneratePjskEvent(Path path)
    {
        using var bitmap
            = new
                BackGround(WebHelper
                               .DownloadImage($"https://assets.pjsek.ai/file/pjsekai-assets/ondemand/event/{_currentEvent.AssetbundleName}/screen/bg/bg.png"));

        using var logo
            = WebHelper.GetImage($"https://assets.pjsek.ai/file/pjsekai-assets/ondemand/event/{_currentEvent.AssetbundleName}/logo/logo/logo.png");

        var bg = bitmap.Cut(new(0, 0, 1000, 1000)).Blur(40);
        bg.FillColor(Color.Black, 80);

        bg.Draw(new ImageModel(logo, 200, 50, 600, 300),
                new TextWithStrokeModel("Current Line", Font.HummingPro24, Color.White, 250, 480),
                new TextWithStrokeModel("Predict Line", Font.HummingPro24, Color.White, 600, 480),
                new LineModel(Color.White, 3, new(240, 530), new(785, 530)),
                new TextWithStrokeModel("100", Font.HummingPro24, Color.White, 80, 550, width: 70, height: 25,
                                        stringAlignment: StringAlignment.Far),
                new TextWithStrokeModel("500", Font.HummingPro24, Color.White, 80, 600, width: 70, height: 25,
                                        stringAlignment: StringAlignment.Far),
                new TextWithStrokeModel("1k", Font.HummingPro24, Color.White, 80, 650, width: 70, height: 25,
                                        stringAlignment: StringAlignment.Far),
                new TextWithStrokeModel("2k", Font.HummingPro24, Color.White, 80, 700, width: 70, height: 25,
                                        stringAlignment: StringAlignment.Far),
                new TextWithStrokeModel("5k", Font.HummingPro24, Color.White, 80, 750, width: 70, height: 25,
                                        stringAlignment: StringAlignment.Far),
                new TextWithStrokeModel("1w", Font.HummingPro24, Color.White, 80, 800, width: 70, height: 25,
                                        stringAlignment: StringAlignment.Far),
                new TextWithStrokeModel("5w", Font.HummingPro24, Color.White, 80, 850, width: 70, height: 25,
                                        stringAlignment: StringAlignment.Far),
                new TextWithStrokeModel("10w", Font.HummingPro24, Color.White, 80, 900, width: 70, height: 25,
                                        stringAlignment: StringAlignment.Far),
                new TextWithStrokeModel("Generated by Project Andreal", Font.KazesawaLight24, Color.White, 1535, 945));

        bg.SaveAsPng(path);
        return bg;
    }
}
