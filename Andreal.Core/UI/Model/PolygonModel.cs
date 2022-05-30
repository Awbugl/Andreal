using System.Drawing;

namespace Andreal.Core.UI.Model;

#pragma warning disable CA1416

internal class PolygonModel : IGraphicsModel
{
    private readonly System.Drawing.Color _color;
    private readonly Point[] _region;

    internal PolygonModel(System.Drawing.Color color, params Point[] region)
    {
        _color = color;
        _region = region;
    }

    void IGraphicsModel.Draw(Graphics g)
    {
        using var brash = new SolidBrush(_color);
        g.FillPolygon(brash, _region);
    }
}
