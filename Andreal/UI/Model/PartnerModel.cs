using System.Drawing;
using Andreal.Core;
using Andreal.Data.Json.Arcaea.PartnerPosInfoBase;
using Path = Andreal.Core.Path;

namespace Andreal.UI.Model;

#pragma warning disable CA1416

internal class PartnerModel : IGraphicsModel
{
    private readonly ImageModel _imageModel;

    internal PartnerModel(int partner, bool awakened, ImgVersion imgVersion)
    {
        var location = PartnerPosInfoBase.Get($"{partner}{(awakened ? "u" : "")}", imgVersion)!;
        _imageModel = new(Path.ArcaeaPartner(partner, awakened).Result, location.PositionX, location.PositionY,
                          location.Size, location.Size);
    }

    void IGraphicsModel.Draw(Graphics g) { (_imageModel as IGraphicsModel).Draw(g); }
}
