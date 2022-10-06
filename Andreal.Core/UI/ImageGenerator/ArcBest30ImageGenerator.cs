using Andreal.Core.Message;
using Andreal.Core.Model.Arcaea;
using Andreal.Core.UI.Model;
using Path = Andreal.Core.Common.Path;

namespace Andreal.Core.UI.ImageGenerator;

internal class ArcBest30ImageGenerator
{
    internal ArcBest30ImageGenerator(IBest30Data b30data, PlayerInfo info)
    {
        B30data = b30data;
        Info = info;
    }

    private IBest30Data B30data { get; }
    private PlayerInfo Info { get; }

    internal async Task<ImageMessage> Generate()
    {
        var bg = new BackGround(Path.ArcaeaBest30Bg);
        bg.Draw(new TextOnlyModel(Info.PlayerName, Font.Andrea72, Color.White, 345, 175),
                new TextOnlyModel($"ArcCode: {Info.PlayerCode}", Font.ExoLight28, Color.White, 370, 320),
                new TextOnlyModel($"Total Best 30: {B30data.Best30Avg}", Font.Andrea60, Color.White, 145, 480),
                new TextOnlyModel($"Recent Best 10: {B30data.Recent10Avg}", Font.Andrea60, Color.White, 1095, 480),
                new ImageModel(await Path.ArcaeaPartnerIcon(Info.Partner, Info.IsAwakened), 75, 130, 255),
                new PotentitalModel(Info.Potential, 175, 235));

        var len = Math.Min(B30data.Best30List.Count, 30);
        for (var i = 0; i < len; ++i)
        {
            var record = B30data.Best30List[i];
            int x = 75 + i % 2 * 950, y = 660 + i / 2 * 350;
            using var song = await record.GetSongImage();

            bg.Draw(
                    new PolygonModel(Color.White, new(x + 9, y), new(x + 891, y), new(x + 900, y + 9), new(x + 900, y + 291), new(x + 891, y + 300),
                                     new(x + 9, y + 300), new(x, y + 291), new(x, y + 9)),
                    new PolygonModel(record.DifficultyInfo.Color, new(x + 278, y + 22), new(x + 278, y + 70), new(x + 503, y + 70),
                                     new(x + 458, y + 22)), new ImageModel(song, x + 22, y + 22, 256, 256),
                    new TextOnlyModel($"[{record.Const:0.0}] {record.SongName(11)}", Font.Beatrice36, song.MainColor, x + 295, y + 80),
                    new TextOnlyModel(record.Score, Font.Exo44, song.MainColor, x + 290, y + 145),
                    new TextOnlyModel(record.Rating, Font.Exo26, System.Drawing.Color.White, x + 297, y + 24),
                    new TextOnlyModel($"#{i + 1}", Font.Beatrice26, System.Drawing.Color.Black, x + 800, y + 24),
                    new TextOnlyModel($"Pure: {record.Pure} (+{record.MaxPure})", Font.Beatrice20, System.Drawing.Color.FromArgb(105, 68, 100),
                                      x + 300, y + 235),
                    new TextOnlyModel($"Far: {record.Far}", Font.Beatrice20, System.Drawing.Color.FromArgb(216, 157, 49), x + 570, y + 235),
                    new TextOnlyModel($"Lost: {record.Lost}", Font.Beatrice20, System.Drawing.Color.FromArgb(159, 83, 109), x + 730, y + 235));
        }

        return bg;
    }
}
