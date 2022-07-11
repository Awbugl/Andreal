using Andreal.Core.Data.Api;
using Andreal.Core.Model.Arcaea;
using Andreal.Core.Utils;

namespace Andreal.Core.Common;

[Serializable]
public class Path
{
    private static readonly string BaseDirectory = AppContext.BaseDirectory;
    public static readonly string ArcaeaBackgroundRoot = BaseDirectory + "/Andreal/Background/";
    private static readonly string ArcaeaImageRoot = BaseDirectory + "/Andreal/Arcaea/";
    private static readonly string ArcaeaSourceRoot = BaseDirectory + "/Andreal/Source/";
    private static readonly string AndreaOtherRoot = BaseDirectory + "/Andreal/Other/";
    private static readonly string PjskImageRoot = BaseDirectory + "/Andreal/Pjsk/";
    public static readonly string AndreaConfigRoot = BaseDirectory + "/Andreal/Config/";
    public static readonly string TempImageRoot = BaseDirectory + "/Andreal/TempImage/";

    public static readonly Path Database = new(AndreaConfigRoot + "Andreal.db");

    public static readonly Path Config = new(AndreaConfigRoot + "config.json");

    public static readonly Path PartnerConfig = new(AndreaConfigRoot + "positioninfo.json");

    public static readonly Path RobotReply = new(AndreaConfigRoot + "replytemplate.json");

    public static readonly Path PjskAlias = new(AndreaConfigRoot + "pjskalias.json");

    public static readonly Path ArcaeaConstListBg = new(ArcaeaSourceRoot + "ConstList.jpg");

    public static readonly Path ArcaeaDivider = new(ArcaeaSourceRoot + "Divider.png");

    public static readonly Path ArcaeaGlass = new(ArcaeaSourceRoot + "Glass.png");

    public static readonly Path ArcaeaBest30Bg = new(ArcaeaSourceRoot + "B30.png");

    public static readonly Path ArcaeaBest40Bg = new(ArcaeaSourceRoot + "B40.png");

    public static readonly Path ArcaeaBest5Bg = new(ArcaeaSourceRoot + "Floor.png");

    public static readonly Path ExceptionReport = new(BaseDirectory + "/Andreal/ExceptionReport.log");

    public static readonly Path ArcaeaBg1Mask = new(ArcaeaSourceRoot + "Mask.png");

    private readonly string _rawpath;

    static Path()
    {
        if (!Directory.Exists(ArcaeaBackgroundRoot)) Directory.CreateDirectory(ArcaeaBackgroundRoot);
        if (!Directory.Exists(ArcaeaSourceRoot)) Directory.CreateDirectory(ArcaeaSourceRoot);
        if (!Directory.Exists(TempImageRoot)) Directory.CreateDirectory(TempImageRoot);
        if (!Directory.Exists(AndreaOtherRoot)) Directory.CreateDirectory(AndreaOtherRoot);

        if (!Directory.Exists(AndreaConfigRoot))
        {
            Directory.CreateDirectory(AndreaConfigRoot);
            Directory.CreateDirectory(AndreaConfigRoot + "BotInfo/");
        }

        if (!Directory.Exists(PjskImageRoot))
        {
            Directory.CreateDirectory(PjskImageRoot);
            Directory.CreateDirectory(PjskImageRoot + "Song/");
            Directory.CreateDirectory(PjskImageRoot + "Chart/");
            Directory.CreateDirectory(PjskImageRoot + "EventBg/");
        }

        if (!Directory.Exists(ArcaeaImageRoot))
        {
            Directory.CreateDirectory(ArcaeaImageRoot);
            Directory.CreateDirectory(ArcaeaImageRoot + "Song/");
            Directory.CreateDirectory(ArcaeaImageRoot + "Char/");
            Directory.CreateDirectory(ArcaeaImageRoot + "Icon/");
            Directory.CreateDirectory(ArcaeaImageRoot + "Preview/");
        }
    }

    private Path(string rawpath) { _rawpath = rawpath; }

    private FileInfo? _fileInfo;

    public FileInfo FileInfo => _fileInfo ??= new(this);

    public static Path BotConfig(uint qqid) => new(AndreaConfigRoot + $"BotInfo/{qqid}.andreal.konata.botinfo");

    public static Path ArcaeaBackground(int version, ArcaeaChart chart) =>
        new(ArcaeaBackgroundRoot + $"V{version}_{ArcaeaTempSong(chart)}.png");

    public static Path ArcaeaBg3Mask(int side) => new(ArcaeaSourceRoot + $"RawV3Bg_{side}.png");

    public static async Task<Path> ArcaeaSong(ArcaeaChart chart)
    {
        var song = ArcaeaTempSong(chart);

        var pth = new Path($"{ArcaeaImageRoot}Song/{song}.jpg");

        if (pth.FileInfo.Exists)
        {
            if (pth.FileInfo.Length > 10240)
                return pth;
            else
                pth.FileInfo.Delete();
        }

        await ArcaeaUnlimitedApi.SongAssets(chart.SongID, chart.RatingClass, pth);

        return pth;
    }

    private static string ArcaeaTempSong(ArcaeaChart chart)
    {
        var song = chart switch
                   {
                       _ when chart.JacketOverride => $"{chart.SongID}_{chart.RatingClass}",
                       _ when chart.SongID == "melodyoflove" =>
                           $"melodyoflove{(DateTime.Now.Hour is > 19 or < 6 ? "_night" : "")}",
                       _ => chart.SongID
                   };
        return song;
    }

    public static Path ArcaeaRating(short potential)
    {
        var img = potential switch
                  {
                      >= 1300 => "7",
                      >= 1250 => "6",
                      >= 1200 => "5",
                      >= 1100 => "4",
                      >= 1000 => "3",
                      >= 700  => "2",
                      >= 350  => "1",
                      >= 0    => "0",
                      < 0     => "off"
                  };
        return new(ArcaeaSourceRoot + $"rating_{img}.png");
    }

    public static async Task<Path> ArcaeaPartner(int partner, bool awakened)
    {
        var pth = new Path(ArcaeaImageRoot + $"Char/{partner}{(awakened ? "u" : "")}.png");

        if (pth.FileInfo.Exists)
        {
            if (pth.FileInfo.Length > 10240)
                return pth;
            else
                pth.FileInfo.Delete();
        }

        await ArcaeaUnlimitedApi.CharAssets(partner, awakened, pth);

        return pth;
    }

    public static async Task<Path> ArcaeaPartnerIcon(int partner, bool awakened)
    {
        var pth = new Path(ArcaeaImageRoot + $"Icon/{partner}{(awakened ? "u" : "")}.png");

        if (pth.FileInfo.Exists)
        {
            if (pth.FileInfo.Length > 10240)
                return pth;
            else
                pth.FileInfo.Delete();
        }

        await ArcaeaUnlimitedApi.IconAssets(partner, awakened, pth);

        return pth;
    }

    public static async Task<Path> ArcaeaChartPreview(ArcaeaChart chart)
    {
        var pth = new Path(ArcaeaImageRoot + $"Preview/{chart.SongID}_{chart.RatingClass}.jpg");

        if (pth.FileInfo.Exists)
        {
            if (pth.FileInfo.Length > 10240)
                return pth;
            else
                pth.FileInfo.Delete();
        }

        await ArcaeaUnlimitedApi.PreviewAssets(chart.SongID, chart.RatingClass, pth);

        return pth;
    }

    public static Path ArcaeaCleartypeV1(sbyte cleartype) => new(ArcaeaSourceRoot + $"end_{cleartype}.png");

    public static Path ArcaeaCleartypeV3(sbyte cleartype) => new(ArcaeaSourceRoot + $"clear_{cleartype}.png");

    public static Path ArcaeaCleartypeV4(sbyte cleartype) => new(ArcaeaSourceRoot + $"clear_badge_{cleartype}.png");

    public static Path ArcaeaDifficultyForV1(int difficulty) => new(ArcaeaSourceRoot + $"con_{difficulty}.png");

    public static Path RandImageFileName() => new(TempImageRoot + $"{RandStringHelper.GetRandString()}.jpg");

    public static Path PjskSong(string sid) => new(PjskImageRoot + $"Song/{sid}.png");

    public static Path PjskChart(string sid, string dif) => new(PjskImageRoot + $"Chart/{sid}_{dif}.png");

    public static Path PjskEvent(string currentEventAssetbundleName) =>
        new(PjskImageRoot + $"EventBg/{currentEventAssetbundleName}.png");

    public static Path ArcaeaBg4(int difficulty, string package)
    {
        var backgroundImgV4 = difficulty == 3
            ? "Beyond"
            : package switch
              {
                  "base"                 => "Arcaea",
                  "core" or "yugamu"     => "Lost World",
                  "rei" or "prelude"     => "Spire of Convergence",
                  "mirai" or "nijuusei"  => "Outer Reaches",
                  "shiawase" or "zettai" => "Dormant Echoes",
                  "vs" or "extend"       => "Boundless Divide",
                  _                      => "Event"
              };
        return new(ArcaeaSourceRoot + $"V4Bg-{backgroundImgV4}.png");
    }

    public static implicit operator string(Path path) => path._rawpath;
}
