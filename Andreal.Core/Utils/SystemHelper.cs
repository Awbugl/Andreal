using System.Net;
using System.Runtime.CompilerServices;
using Andreal.Core.Common;
using Andreal.Core.Data.Api;

[assembly: InternalsVisibleTo("Andreal.Test")]

namespace Andreal.Core.Utils;

internal static class SystemHelper
{
    public static void Init(AndrealConfig andrealConfig)
    {
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        ServicePointManager.ServerCertificateValidationCallback = (
            _,
            _,
            _,
            _) => true;
        ServicePointManager.DefaultConnectionLimit = 512;
        ServicePointManager.Expect100Continue = false;
        ServicePointManager.UseNagleAlgorithm = false;
        ServicePointManager.ReusePort = true;
        ServicePointManager.CheckCertificateRevocationList = true;
        WebRequest.DefaultWebProxy = null;

        UnofficialArcaeaAPI.Init(andrealConfig);
        MessageInfo.Init(andrealConfig.Master);
        BackgroundTask.Init();
        ConfigWatcher.Init();
    }
}
