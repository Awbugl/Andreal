using System.Drawing;
using System.Drawing.Text;
using Path = Andreal.Core.Common.Path;

namespace Andreal.Core.UI;

#pragma warning disable CA1416

[Serializable]
internal static class Font
{
    private static PrivateFontCollection _pfc;
    private static Dictionary<string, FontFamily> _fontFamily;

    static Font()
    {
        _pfc = new();
        foreach (var s in new DirectoryInfo(Path.ArcaeaFontRoot).GetFiles().Select(s => s.FullName)) _pfc.AddFontFile(s);
        _fontFamily = _pfc.Families.ToDictionary(i => i.Name, i => i);
        Exo64 = GetFont("Exo", 64);
        Exo60 = GetFont("Exo", 60);
        Exo44 = GetFont("Exo", 44);
        Exo40 = GetFont("Exo", 40);
        Exo36 = GetFont("Exo", 36);
        Exo32 = GetFont("Exo", 32);
        Exo26 = GetFont("Exo", 26);
        Exo24 = GetFont("Exo", 24);
        Exo20 = GetFont("Exo", 20);
        Andrea108 = GetFont("Andrea", 108);
        Andrea90 = GetFont("Andrea", 90);
        Andrea72 = GetFont("Andrea", 72);
        Andrea60 = GetFont("Andrea", 60);
        Andrea56 = GetFont("Andrea", 56);
        Andrea36 = GetFont("Andrea", 36);
        Andrea28 = GetFont("Andrea", 28);
        Andrea20 = GetFont("Andrea", 20);
        Beatrice36 = GetFont("Beatrice", 36);
        Beatrice26 = GetFont("Beatrice", 26);
        Beatrice24 = GetFont("Beatrice", 24);
        Beatrice20 = GetFont("Beatrice", 20);
        ExoLight42 = GetFont("Exo Andrea", 42);
        ExoLight36 = GetFont("Exo Andrea", 36);
        ExoLight28 = GetFont("Exo Andrea", 28);
        ExoLight24 = GetFont("Exo Andrea", 24);
        ExoLight20 = GetFont("Exo Andrea", 20);
        KazesawaLight72 = GetFont("Kazesawa Light", 72);
        KazesawaLight56 = GetFont("Kazesawa Light", 56);
        KazesawaLight48 = GetFont("Kazesawa Light", 48);
        KazesawaLight40 = GetFont("Kazesawa Light", 40);
        KazesawaLight32 = GetFont("Kazesawa Light", 32);
        KazesawaLight24 = GetFont("Kazesawa Light", 24);
        KazesawaRegular56 = GetFont("Kazesawa Regular", 56);
        KazesawaRegular27 = GetFont("Kazesawa Regular", 27);
    }

    private static System.Drawing.Font GetFont(string name, float emSize) => new(_fontFamily[name], emSize);

    internal static readonly System.Drawing.Font Exo64,
                                                 Exo60,
                                                 Exo44,
                                                 Exo40,
                                                 Exo36,
                                                 Exo32,
                                                 Exo26,
                                                 Exo24,
                                                 Exo20,
                                                 Andrea108,
                                                 Andrea90,
                                                 Andrea72,
                                                 Andrea60,
                                                 Andrea56,
                                                 Andrea36,
                                                 Andrea28,
                                                 Andrea20,
                                                 Beatrice36,
                                                 Beatrice26,
                                                 Beatrice24,
                                                 Beatrice20,
                                                 ExoLight42,
                                                 ExoLight36,
                                                 ExoLight28,
                                                 ExoLight24,
                                                 ExoLight20,
                                                 KazesawaLight72,
                                                 KazesawaLight56,
                                                 KazesawaLight48,
                                                 KazesawaLight40,
                                                 KazesawaLight32,
                                                 KazesawaLight24,
                                                 KazesawaRegular56,
                                                 KazesawaRegular27;
}
