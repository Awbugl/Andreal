using System.Drawing;
using Andreal.Message;
using Image = Andreal.UI.Image;
using Path = Andreal.Core.Path;

namespace Andreal.Model.Arcaea;

[Serializable]
internal class SongInfo
{
    private static readonly Dictionary<string, Stream> SongImage = new();

    private SongInfo(Songdata songMetadata, sbyte difficulty)
    {
        Songdata = songMetadata;
        Difficulty = difficulty;
    }

    internal SongInfo(string sid, sbyte difficulty)
    {
        Songdata = Songdata.GetBySid(sid);
        Difficulty = difficulty;
    }

    private Songdata? Songdata { get; }

    internal sbyte Difficulty { get; }

    internal string SongName => Songdata!.Songname;

    internal string SongId => Songdata!.SongId;

    internal long Note => Songdata!.Notes[Difficulty];

    internal double Const => Songdata!.Consts[Difficulty];

    internal Side PartnerSide => Songdata!.PartnerSide;

    internal string PackageInfo => Songdata!.PackageInfo;

    internal string PackageId => Songdata!.Data.Set;

    internal DifficultyInfo DifficultyInfo => DifficultyInfo.GetByIndex(Difficulty);

    internal Color MainColor
    {
        get
        {
            using var img = GetSongImg();
            return img.Result.MainColor;
        }
    }

    internal string ConstString => $"[{DifficultyInfo.ShortStr} {Const:0.0}]";

    internal string NameWithPackageAndConst => $"{SongName}\n(Package: {PackageInfo})\n{ConstString}";

    internal string NameWithPackage => $"{SongName}\n(Package: {PackageInfo})";

    internal async Task<Image> GetSongImg()
    {
        var path = await Path.ArcaeaSong(Songdata!.SongId, Difficulty);

        try
        {
            if (!SongImage.TryGetValue(path, out var stream))
            {
                await using var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                var bytes = new byte[fileStream.Length];
                fileStream.Read(bytes, 0, bytes.Length);
                fileStream.Close();
                stream = new MemoryStream(bytes);
                SongImage.Add(path, stream);
            }

            var img = new Image(stream);
            if (img.Width == 512) return img;
            var newimg = new Image(img, 512, 512);
            newimg.SaveAsPng(path);
            img.Dispose();
            return newimg;
        }
        catch
        {
            File.Delete(path);
            throw new ArgumentException($"InvalidSongImage: {SongId}, deleted.");
        }
    }

    internal static IEnumerable<SongInfo> GetByConst(double dnum) => Convert(Songdata.GetByConst(dnum));

    internal string GetSongName(byte length) => Songdata!.GetSongName(length);

    internal async Task<MessageChain> FullConstString()
    {
        MessageChain msg = ImageMessage.FromPath(await Path.ArcaeaSong(Songdata!.SongId, 2));
        msg.Append(Songdata.Consts[3] <= 0
                       ? null
                       : ImageMessage.FromPath(await Path.ArcaeaSong(Songdata.SongId, 3)));
        msg.Append($"{SongName}\n(Package: {PackageInfo})");
        for (sbyte i = 0; i < 3; ++i) msg.Append("\n" + Songdata.ConstString(i));
        msg.Append(Songdata.Consts[3] <= 0
                       ? null
                       : "\n" + Songdata.ConstString(3));
        return msg;
    }

    internal static SongInfo? RandomSong(double lowerlimit, double upperlimit)
    {
        var item = Songdata.RandomSong(lowerlimit, upperlimit);
        return item.Item1 is null
            ? null
            : item!;
    }

    internal static SongInfo RandomSong() => new(Songdata.RandomSong(), 0);


    internal static async Task<(int, SongInfo[]?)> GetByAlias(string songname, sbyte difficulty)
    {
        var (status, ls) = await Songdata.GetByAlias(songname);
        return status switch
               {
                   0  => (0, ls!.Select(i => new SongInfo(i!, difficulty)).ToArray()),
                   -1 => (-1, null),
                   _  => (-2, ls!.Select(i => new SongInfo(i!, difficulty)).ToArray())
               };
    }

    internal bool CheckNull() => Songdata == null;

    public override string ToString() => ConstString;

    public static implicit operator SongInfo((Songdata, sbyte) valueTuple) => new(valueTuple.Item1, valueTuple.Item2);

    private static IEnumerable<SongInfo> Convert(IEnumerable<(Songdata, sbyte)> ls) =>
        ls.Select(i => new SongInfo(i.Item1, i.Item2));
}
