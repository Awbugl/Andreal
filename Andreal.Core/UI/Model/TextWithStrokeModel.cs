using System.Drawing;
using System.Drawing.Drawing2D;

namespace Andreal.Core.UI.Model;

#pragma warning disable CA1416

internal class TextWithStrokeModel : IGraphicsModel
{
    private static readonly System.Drawing.Color Defcolor = System.Drawing.Color.FromArgb(82, 82, 82);
    private readonly System.Drawing.Color _color;
    private readonly System.Drawing.Font _font;
    private readonly System.Drawing.Color? _penColor;
    private readonly int _posX, _posY, _penWidth;
    private readonly StringAlignment _stringAlignment;
    private readonly string _text;
    private readonly int? _width, _height;

    internal TextWithStrokeModel(string text, System.Drawing.Font font, System.Drawing.Color color, int posX, int posY,
                                 System.Drawing.Color? penColor = null, int penWidth = 3,
                                 StringAlignment stringAlignment = StringAlignment.Near, int? width = null,
                                 int? height = null)
    {
        _text = text;
        _color = color;
        _font = font;
        _posX = posX;
        _posY = posY;
        _penColor = penColor;
        _penWidth = penWidth;
        _stringAlignment = stringAlignment;
        _width = width;
        _height = height;
    }

    void IGraphicsModel.Draw(Graphics g)
    {
        var origin = new Point(_posX, _posY);
        using var s = new StringFormat { Alignment = _stringAlignment };
        using var path = new GraphicsPath();
        using var brush = new SolidBrush(_color);
        using var pen = new Pen(_penColor ?? Defcolor, _penWidth);
        path.AddString(_text, _font.FontFamily, (int)_font.Style, _font.SizeInPoints, _width == null || _height == null
                           ? new(origin, g.MeasureString(_text + "?", _font, origin, s))
                           : new RectangleF(_posX, _posY, (float)_width, (float)_width), s);
        g.DrawPath(pen, path);
        g.FillPath(brush, path);
    }
}
