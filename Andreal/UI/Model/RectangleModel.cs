using System.Drawing;

namespace Andreal.UI.Model;

#pragma warning disable CA1416

internal class RectangleModel : IGraphicsModel
{
    private readonly System.Drawing.Color _color;
    private readonly Rectangle _rect;

    internal RectangleModel(System.Drawing.Color color, Rectangle rect)
    {
        _color = color;
        _rect = rect;
    }

    void IGraphicsModel.Draw(Graphics g)
    {
        using var brash = new SolidBrush(_color);
        g.FillRectangle(brash, _rect);
    }
}
