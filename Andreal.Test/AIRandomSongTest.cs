namespace Andreal.Test;

public class Tests
{
    [SetUp]
    public void Setup()
    {
        var config = JsonConvert.DeserializeObject<AndrealConfig>(File.ReadAllText(Core.Common.Path.Config))!;
        External.Initialize(config);
    }

    [Test]
    public void AIRandomSongTest()
    {
        var robotReply = GlobalConfig.RobotReply;
        var info = ArcaeaCharts.RandomSong(0, 15)!;
        var format = $"Ai酱：{robotReply.GetRandomAIReply(info.NameWithPackageAndConst, info.Artist)}";
        Console.WriteLine(format);
        Assert.Pass();
    }
}
