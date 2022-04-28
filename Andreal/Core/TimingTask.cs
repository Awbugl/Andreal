using System.Timers;
using Andreal.Data.Api;
using Andreal.Data.Json.Pjsk;
using Andreal.Model.Pjsk;
using Timer = System.Timers.Timer;

namespace Andreal.Core;

internal static class BackgroundTask
{
    private static ulong _timerCount;

    internal static void Init()
    {
        Timer timer = new(240000);
        timer.Elapsed += PjskUpdate;
        timer.Elapsed += Clean;
        timer.Elapsed += (_, _) => ++_timerCount;
        timer.AutoReset = true;
        timer.Enabled = true;
    }

    private static void Clean(object? source, ElapsedEventArgs e)
    {
        var time = DateTime.Now.AddMinutes(-2);

        foreach (var j in new DirectoryInfo(Path.TempImageRoot).GetFiles().Where(j => time > j.LastWriteTime))
            j.Delete();
    }

    private static async void PjskUpdate(object? source, ElapsedEventArgs e)
    {
        try
        {
            if (_timerCount % 15 != 0) return;

            var musics = new List<PjskMusics>();
            var musicMetas = new List<PjskMusicMetas>();
            var musicDifficulties = new List<PjskMusicDifficulties>();

            await Task.WhenAll(Task.Run(() => musics = PjskApi.PjskMusics().Result ?? musics),
                               Task.Run(() => musicMetas = PjskApi.PjskMusicMetas().Result ?? musicMetas),
                               Task.Run(() => musicDifficulties
                                            = PjskApi.PjskMusicDifficulties().Result ?? musicDifficulties));

            foreach (var item in musics)
                if (!SongInfo.CheckFull(item.ID))
                {
                    var metas = musicMetas.Where(i => i.MusicID == item.ID).ToList();
                    if (metas.Any())
                        SongInfo.Insert(item, metas, null);
                    else
                    {
                        var difficulties = musicDifficulties.Where(i => i.MusicID == item.ID).ToList();
                        if (difficulties.Any()) SongInfo.Insert(item, difficulties, null);
                    }
                }
        }
        catch (Exception ex)
        {
            Reporter.ExceptionReport(ex);
        }
    }
}
