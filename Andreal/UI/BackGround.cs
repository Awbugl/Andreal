using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using Andreal.UI.Model;
using Path = Andreal.Core.Path;

namespace Andreal.UI;

#pragma warning disable CA1416

public class BackGround : Image, IDisposable
{
    private Graphics? _g;

    internal BackGround(Path path) : base(path) { }

    private BackGround(Bitmap bitmap) : base(bitmap) { }

    internal BackGround(int width, int height) : base(width, height) { }

    internal BackGround(Image origin, int width, int height) : base(origin, width, height) { }

    public new void Dispose()
    {
        if (Alreadydisposed) return;
        _g?.Dispose();
        base.Dispose();
    }

    ~BackGround()
    {
        _g?.Dispose();
        base.Dispose();
    }

    internal Graphics GraphicsFromBackGround()
    {
        if (_g is not null) return _g;
        _g = Graphics.FromImage(Bitmap);
        _g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        _g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        _g.CompositingQuality = CompositingQuality.HighQuality;
        _g.SmoothingMode = SmoothingMode.HighQuality;
        _g.TextRenderingHint = TextRenderingHint.AntiAlias;
        return _g;
    }

    internal new BackGround Cut(Rectangle rectangle) => new(Bitmap.Clone(rectangle, PixelFormat.Format32bppArgb));

    internal void FillColor(System.Drawing.Color color, int alpha = 120)
    {
        GraphicsFromBackGround().FillRectangle(new SolidBrush(System.Drawing.Color.FromArgb(alpha, color)), 0, 0,
                                               Width, Height);
    }

    internal void Draw(params IGraphicsModel[] graphicsModelCollection)
    {
        foreach (var i in graphicsModelCollection) i.Draw(GraphicsFromBackGround());
    }

    [DllImport(@"Andreal\Other\StackBlurHost.dll", CallingConvention = CallingConvention.StdCall)]
    private static extern void StackBlurHost(IntPtr imagePixelBuffer, int imageWidth, int imageHeight, int imageStride,
                                             int roundSize);

    internal BackGround BlurImage(int roundSize)
    {
        var bm = Bitmap.LockBits(new(0, 0, Bitmap.Width, Bitmap.Height), ImageLockMode.ReadWrite,
                                 PixelFormat.Format32bppArgb);
        StackBlurHost(bm.Scan0, bm.Width, bm.Height, bm.Stride, roundSize);
        Bitmap.UnlockBits(bm);
        return this;
    }
}
