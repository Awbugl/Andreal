namespace Andreal.UI;

[Serializable]
internal static class Color
{
    internal static readonly System.Drawing.Color White = System.Drawing.Color.White,
                                                  Black = System.Drawing.Color.Black,
                                                  Light = FromArgb(150, 100, 200, 225),
                                                  Conflict = FromArgb(150, 50, 20, 75),
                                                  PmColor = FromArgb(150, 180, 200), ArcGray = FromArgb(60, 60, 60),
                                                  ArcPurple = FromArgb(31, 30, 51), GnaqGray = FromArgb(110, 110, 110),
                                                  AzusaGray = FromArgb(90, 90, 90);

    internal static System.Drawing.Color FromArgb(int alpha, System.Drawing.Color baseColor) =>
        System.Drawing.Color.FromArgb(alpha, baseColor);

    internal static System.Drawing.Color FromArgb(int r, int g, int b) => System.Drawing.Color.FromArgb(r, g, b);

    internal static System.Drawing.Color FromArgb(int a, int r, int g, int b) =>
        System.Drawing.Color.FromArgb(a, r, g, b);
}
