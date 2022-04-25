using Andreal.Message;
using Andreal.Model.Arcaea;
using Andreal.UI.Model;
using Path = Andreal.Core.Path;

namespace Andreal.UI.ImageGenerator;

#pragma warning disable CA1416

internal class ArcSongLevelListImageGenerator
{
    private static readonly System.Drawing.Color WhiteBackground = Color.FromArgb(180, 245, 245, 245);

    internal ArcSongLevelListImageGenerator((string, ArcaeaChart)[] list) { List = list; }

    private (string, ArcaeaChart)[] List { get; }

    internal async Task<ImageMessage> Generate()
    {
        var height = 160 + 200 * List.Length;

        var bg = new BackGround(1080, height);
        bg.Draw(new ImageModel(Path.ArcaeaConstListBg, 0, 0, 1080, height),
                new RectangleModel(WhiteBackground, new(40, 40, 1000, height - 80)));

        for (var i = 0; i < List.Length; ++i)
        {
            var y = 110 + i * 200;

            var (sid,info) = List[i];

            using var song = await ArcaeaCharts.GetSongImg(sid,info.RatingClass);

            var color = song.MainColor;

            bg.Draw(new RectangleModel(info.Side == (int)Side.Hikari
                                           ? Color.Light
                                           : Color.Conflict, new(212, y + 6, 140, 140)),
                    new ImageModel(Path.ArcaeaDivider, 40, 64 + i * 200, 1000, 32),
                    new ImageModel(song, 206, y, 140, 140),
                    new TextOnlyModel(info.GetSongName(22), Font.KazesawaRegular36, color, 380, y - 5),
                    new TextOnlyModel($"{info.DifficultyInfo.LongStr}  {info.ConstString}  (+{info.Note})", Font.Exo20,
                                      info.DifficultyInfo.Color, 390, y + 60),
                    new TextOnlyModel(info.SetFriendly, Font.Beatrice20, System.Drawing.Color.Black, 390, y + 100));
        }

        bg.Draw(new ImageModel(Path.ArcaeaDivider, 40, 64 + List.Length * 200, 1000, 32));

        return bg;
    }
}
