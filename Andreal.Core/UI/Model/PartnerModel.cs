using System.Drawing;
using Andreal.Core.Common;
using Andreal.Core.Data.Json.Arcaea.PartnerPosInfoBase;
using Path = Andreal.Core.Common.Path;

namespace Andreal.Core.UI.Model;

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
