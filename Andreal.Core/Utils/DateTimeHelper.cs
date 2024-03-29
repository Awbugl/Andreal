﻿namespace Andreal.Core.Utils;

internal static class DateTimeHelper
{
    internal static string DateStringFromNow(this long unixTime)
    {
        var span = DateTime.UtcNow - DateTime.UnixEpoch.AddSeconds(unixTime);

        return span.TotalMinutes switch
               {
                   > 1 => $"{(int)Math.Floor(span.TotalMinutes)}分钟前",
                   _   => $"{(int)Math.Ceiling(span.TotalSeconds)}秒前"
               };
    }
}
