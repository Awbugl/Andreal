using System.Drawing;
using System.Drawing.Drawing2D;
using Path = Andreal.Core.Common.Path;

namespace Andreal.Core.UI.Model;

#pragma warning disable CA1416

internal class PotentitalModel : IGraphicsModel
{
    private readonly int _posX, _posY, _size;
    private readonly short _potential;

    internal PotentitalModel(short potential, int positionX, int positionY, int size = 200)
    {
        _potential = potential;
        _posX = positionX;
        _posY = positionY;
        _size = size;
    }

    void IGraphicsModel.Draw(Graphics g)
    {
        using var temp = new BackGround(200, 200);
        using var gr = temp.GraphicsFromBackGround();
        temp.Draw(new ImageModel(Path.ArcaeaRating(_potential), 21, 21, 158));
        if (_potential < 0)
            temp.Draw(new TextWithStrokeModel("--", Font.Exo64, Color.White, 65, 52, Color.ArcPurple, 8));
        else
        {
            var ptt = Convert.ToString(_potential + 10000, 10).ToCharArray();
            var lx = 5 + (ptt[1] == '0'
                ? 0
                : ptt[1] == '1'
                    ? 23
                    : 37) + (ptt[2] == '1'
                ? 23
                : ptt[2] == '7'
                    ? 25
                    : 37);
            var rx = 20 + (ptt[3] == '1'
                ? 17
                : 30) + (ptt[4] == '1'
                ? 17
                : 30);
            var posx = 98 - (lx + rx) / 2;
            string l = ptt[1] == '0'
                       ? $"{ptt[2]}"
                       : $"{ptt[1]}{ptt[2]}", r = $".{ptt[3]}{ptt[4]}";
            using var s = new StringFormat { Alignment = StringAlignment.Near };
            using var path = new GraphicsPath();
            using var pen = new Pen(Color.ArcPurple, 8);
            path.AddString(l, Font.Exo60.FontFamily, 0, 60, new Rectangle(posx, 56, 150, 70), s);
            path.AddString(r, Font.Exo44.FontFamily, 0, 44, new Rectangle(posx + lx, 73, 150, 53), s);
            gr.DrawPath(pen, path);
            gr.FillPath(Brushes.White, path);
        }

        Image.ImageExtend.DrawImage(g, temp, _posX, _posY, _size, _size);
    }
}
