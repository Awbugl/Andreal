using Andreal.Message;
using Andreal.Model.Arcaea;
using Andreal.UI.Model;
using Path = Andreal.Core.Path;

namespace Andreal.UI.ImageGenerator;

internal class ArcRecord5ImageGenerator
{
    internal ArcRecord5ImageGenerator(IBest30Data b30data, bool isFloor)
    {
        B30data = b30data;
        IsFloor = isFloor;
    }

    private IBest30Data B30data { get; }
    private bool IsFloor { get; }

    internal async Task<ImageMessage> Generate()
    {
        var bg = new BackGround(Path.ArcaeaBest5Bg);

        bg.Draw(new TextWithStrokeModel(B30data.Best30Avg, Font.KazesawaLight48, Color.Black, 740, 90, Color.Black, 1),
                new TextWithStrokeModel(B30data.Recent10Avg, Font.KazesawaLight48, Color.Black, 740, 180, Color.Black,
                                        1));

        for (var i = 0; i < Math.Min(5, B30data.Best30List.Count); ++i)
        {
            var record = B30data.Best30List[i + (IsFloor
                                                ? Math.Max(0, B30data.Best30List.Count - 5)
                                                : 0)];

            using var song = await record.GetSongImage();

            bg.Draw(new ImageModel(song, 94, 304 + i * 340, 280, 280),
                    new TextWithStrokeModel(record.SongName(18), Font.KazesawaLight72, song.MainColor, 420,
                                            295 + i * 340, Color.Black, 1),
                    new TextWithStrokeModel(record.Score, Font.KazesawaLight56, Color.ArcGray, 426, 403 + i * 340,
                                            Color.Black, 1),
                    new TextWithStrokeModel($"{record.DifficultyInfo.LongStr} {record.Const:0.0}", Font.KazesawaLight56,
                                            record.DifficultyInfo.Color, 804, 403 + i * 340, Color.Black, 1),
                    new TextWithStrokeModel("F", Font.KazesawaLight56, Color.ArcGray, 420, 493 + i * 340, Color.Black,
                                            1),
                    new TextWithStrokeModel("L", Font.KazesawaLight56, Color.ArcGray, 633, 493 + i * 340, Color.Black,
                                            1),
                    new TextWithStrokeModel("PTT", Font.KazesawaLight56, Color.ArcGray, 814, 493 + i * 340, Color.Black,
                                            1),
                    new TextWithStrokeModel(record.Far, Font.KazesawaLight56, Color.ArcGray, 470, 493 + i * 340,
                                            Color.Black, 1),
                    new TextWithStrokeModel(record.Lost, Font.KazesawaLight56, Color.ArcGray, 683, 493 + i * 340,
                                            Color.Black, 1),
                    new TextWithStrokeModel(record.Rating, Font.KazesawaLight56, Color.ArcGray, 964, 493 + i * 340,
                                            Color.Black, 1));
        }

        return bg;
    }
}
