using System.Drawing;

namespace Andreal.UI.Model;

#pragma warning disable CA1416

internal class LineModel : IGraphicsModel
{
    private readonly System.Drawing.Color _color;
    private readonly Point _end;
    private readonly int _penwidth;
    private readonly Point _start;

    internal LineModel(System.Drawing.Color color, int penwidth, Point start, Point end)
    {
        _color = color;
        _penwidth = penwidth;
        _start = start;
        _end = end;
    }

    void IGraphicsModel.Draw(Graphics g) { g.DrawLine(new(_color, _penwidth), _start, _end); }
}
